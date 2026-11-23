
using System.Diagnostics;
using System.Xml.Linq;

internal class Program
{
    private static Settings? currentSettings;
    private static List<FileSystemWatcher> currentWatchers = new();
    private static DateTime sistaOmladdning = DateTime.MinValue;
    private static FileSystemWatcher? configWatcher;

    private static int Main(string[] args)
    {
        //Sökvägen till inställningsfilen skapas från programmets mapp.
        var inställningsSökväg = Path.Combine(AppContext.BaseDirectory, "Inställningar.xml");

        if (!File.Exists(inställningsSökväg))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Hittar inte inställningar {inställningsSökväg}!");
            Console.ResetColor();
            return 1;
        }

        //Läs in inställningar för första gången
        currentSettings = LäsInställningar(inställningsSökväg);
        if (currentSettings == null)
        {
            return 1;
        }

        SorteraBefintligaFiler(currentSettings);

        //Starta bevakning av inmatningsmapparna
        currentWatchers = StartaBevakning(currentSettings);

        //Starta bevakning av själva konfigurationsfilen (Inställningar.xml)
        configWatcher = new FileSystemWatcher(AppContext.BaseDirectory)
        {
            Filter = "Inställningar.xml",
            EnableRaisingEvents = true
        };

        configWatcher.Changed += (sender, e) =>
        {
            //Undvik dubbelkörning om händelsen triggas flera gånger snabbt
            if ((DateTime.Now - sistaOmladdning).TotalMilliseconds < 500)
            {
                return;
            }
            sistaOmladdning = DateTime.Now;

            // Vänta en stund så att filen inte är låst av editorn
            Thread.Sleep(200);

            try
            {
                var nyaInställningar = LäsInställningar(inställningsSökväg);
                if (nyaInställningar == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Kunde inte läsa in nya inställningar (ogiltigt format). Behåller gamla.");
                    Console.ResetColor();
                    return;
                }

                //Stoppa och rensa gamla watchers
                foreach (var w in currentWatchers)
                {
                    w.EnableRaisingEvents = false;
                    w.Dispose();
                }
                currentWatchers.Clear();

                currentSettings = nyaInställningar;

                SorteraBefintligaFiler(currentSettings);

                //Starta nya watchers
                currentWatchers = StartaBevakning(currentSettings);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Inställningarna har laddats om!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Fel vid omladdning av inställningar: {ex.Message}");
                Console.ResetColor();
            }
        };

        Task.Delay(Timeout.Infinite).Wait();
        return 0;
    }

    //Läser inställningar från XML-filen och kontrollerar att de är giltiga.
    private static Settings? LäsInställningar(string sökväg)
    {
        XDocument xml;

        try
        {
            xml = XDocument.Load(sökväg);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Reading Xml Failed: {e.Message}");
            Console.ResetColor();
            return null;
        }

        var settings = new Settings();

        var root = xml.Root;
        if (root == null || root.Name != "Settings")
        {
            Console.WriteLine("Roten i Inställningar.xml måste heta <Settings>");
            return null;
        }

        //Hantera bakåtsnedstreck för plattformsoberoende loggsökväg
        var log = root.Element("Log")?.Value.Trim().Replace('\\', Path.DirectorySeparatorChar);
        if (string.IsNullOrWhiteSpace(log))
        {
            Console.WriteLine("<Log> saknas i Inställningar.xml");
            return null;
        }
        settings.Log = log;

        var directoryElements = root.Elements("Directory").ToList();
        if (directoryElements.Count == 0)
        {
            Console.WriteLine("<Directory> saknas i Inställningar.xml");
            return null;
        }

        foreach (var directoryElement in directoryElements)
        {
            var rule = new DirectoryRule();

            var name = directoryElement.Element("Name")?.Value.Trim();

            //Hantera bakåtsnedstreck för alla sökvägar
            var input = directoryElement.Element("Input")?.Value.Trim().Replace('\\', Path.DirectorySeparatorChar);
            var output = directoryElement.Element("Output")?.Value.Trim().Replace('\\', Path.DirectorySeparatorChar);
            var convert = directoryElement.Element("Convert")?.Value.Trim().Replace('\\', Path.DirectorySeparatorChar);
            var export = directoryElement.Element("Export")?.Value.Trim().Replace('\\', Path.DirectorySeparatorChar);
            var error = directoryElement.Element("Error")?.Value.Trim().Replace('\\', Path.DirectorySeparatorChar);

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(input) ||
                string.IsNullOrWhiteSpace(output))
            {
                Console.WriteLine("<Name>, <Input> eller <Output> saknas i en <Directory>");
                return null;
            }

            //En regel som konverterar måste veta var CSV-filer och trasiga order ska hamna
            if (!string.IsNullOrWhiteSpace(convert) &&
                (string.IsNullOrWhiteSpace(export) || string.IsNullOrWhiteSpace(error)))
            {
                Console.WriteLine($"<Export> och <Error> krävs när <Convert> finns i <Directory> {name}");
                return null;
            }

            rule.Name = name;
            rule.Input = input;
            rule.Output = output;
            rule.Convert = convert;
            rule.Export = export;
            rule.Error = error;

            foreach (var typeElement in directoryElement.Elements("Type"))
            {
                rule.Types.Add(typeElement.Value.Trim());
            }

            if (rule.Types.Count == 0)
            {
                Console.WriteLine($"<Type> saknas i <Directory> {rule.Name}");
                return null;
            }

            settings.Directories.Add(rule);
        }

        try
        {
            foreach (var rule in settings.Directories)
            {
                if (!Directory.Exists(rule.Input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Inmatningsmappen hittades inte: {rule.Input}");
                    Console.ResetColor();
                    return null;
                }

                if (!Directory.Exists(rule.Output))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Utmatningsmappen hittades inte: {rule.Output}");
                    Console.ResetColor();
                    return null;
                }

                //Bara regler som konverterar har Convert, Export och Error
                if (!string.IsNullOrWhiteSpace(rule.Convert))
                {
                    if (!File.Exists(rule.Convert))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Konverteringsprogrammet hittades inte: {rule.Convert}");
                        Console.ResetColor();
                        return null;
                    }

                    if (!Directory.Exists(rule.Export))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Exportmappen hittades inte: {rule.Export}");
                        Console.ResetColor();
                        return null;
                    }

                    if (!Directory.Exists(rule.Error))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Felmappen hittades inte: {rule.Error}");
                        Console.ResetColor();
                        return null;
                    }

                    //Export och Error får inte vara samma mapp som Input, annars bevakar programmet sina egna filer
                    if (SammaMapp(rule.Export!, rule.Input) || SammaMapp(rule.Error!, rule.Input))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Export- och felmappen får inte vara samma som Input i {rule.Name}");
                        Console.ResetColor();
                        return null;
                    }
                }
            }
            var logMapp = Path.GetDirectoryName(settings.Log);
            if (!string.IsNullOrWhiteSpace(logMapp) && !Directory.Exists(logMapp))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Loggfilens mapp hittades inte: {logMapp}");
                Console.ResetColor();
                return null;
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fel vid kontroll av mappar: {e.Message}");
            Console.ResetColor();
            return null;
        }

        return settings;
    }

    //Jämför två mappsökvägar efter att de gjorts fullständiga, så att till exempel "C:\Order\" och "c:/order" räknas som samma.
    private static bool SammaMapp(string a, string b)
    {
        var fullA = Path.TrimEndingDirectorySeparator(Path.GetFullPath(a));
        var fullB = Path.TrimEndingDirectorySeparator(Path.GetFullPath(b));
        return string.Equals(fullA, fullB, StringComparison.OrdinalIgnoreCase);
    }

    //Bevakar inmatningsmapparna efter nya filer.
    private static List<FileSystemWatcher> StartaBevakning(Settings settings)
    {
        var watchers = new List<FileSystemWatcher>();

        foreach (var rule in settings.Directories)
        {
            var watcher = new FileSystemWatcher(rule.Input);
            watcher.Created += (sender, e) => HanteraFil(e.FullPath, rule, settings.Log);
            watcher.EnableRaisingEvents = true;
            watchers.Add(watcher);
        }
        return watchers;
    }

    //Sorterar filer som redan finns i inmatningsmapparna.
    private static void SorteraBefintligaFiler(Settings settings)
    {
        foreach (var rule in settings.Directories)
        {
            if (Directory.Exists(rule.Input))
            {
                var filer = Directory.GetFiles(rule.Input);
                foreach (var fil in filer)
                {
                    HanteraFil(fil, rule, settings.Log);
                }
            }
        }
    }

    //Flyttar en fil till rätt utmatningsmapp och skriver till loggfilen.
    private static void HanteraFil(string fil, DirectoryRule rule, string logPath)
    {
        try
        {
            string extension = Path.GetExtension(fil);
            if (!rule.Types.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            //Order går via Blackbox i stället för att bara flyttas
            if (!string.IsNullOrWhiteSpace(rule.Convert))
            {
                KonverteraOrder(fil, rule, logPath);
                return;
            }

            FlyttaFil(fil, rule.Output);
            Logga(logPath, $"Flyttade {Path.GetFileName(fil)} till {rule.Name}");
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fel vid hantering av fil {fil}: {e.Message}");
            Console.ResetColor();
        }
    }

    //Konverterar en order med Blackbox och flyttar den till Behandlade eller Fel.
    private static void KonverteraOrder(string fil, DirectoryRule rule, string logPath)
    {
        VäntaPåFil(fil);

        string filnamn = Path.GetFileName(fil);
        string csvNamn = Path.ChangeExtension(filnamn, ".csv");
        string csvSökväg = Path.Combine(rule.Export!, csvNamn);

        //ArgumentList hanterar mellanslag i sökvägarna åt oss
        var startInfo = new ProcessStartInfo(rule.Convert!)
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add(fil);
        startInfo.ArgumentList.Add(csvSökväg);

        int returkod;
        string felmeddelande;

        using (var process = Process.Start(startInfo)!)
        {
            //Läs felutskriften parallellt, annars kan Blackbox fastna om den skriver mycket
            var felLäsning = process.StandardError.ReadToEndAsync();

            if (process.WaitForExit(10000))
            {
                returkod = process.ExitCode;
                felmeddelande = felLäsning.Result.Trim();
            }
            else
            {
                process.Kill(true);
                returkod = -1;
                felmeddelande = "Blackbox svarade inte inom 10 sekunder";
            }
        }

        if (returkod == 0)
        {
            FlyttaFil(fil, rule.Output);
            Logga(logPath, $"Konverterade {filnamn} till {csvNamn}");
        }
        else
        {
            //En halvfärdig CSV-fil ska inte ligga kvar och se korrekt ut
            if (File.Exists(csvSökväg))
            {
                File.Delete(csvSökväg);
            }

            FlyttaFil(fil, rule.Error!);
            Logga(logPath, $"Kunde inte konvertera {filnamn}: {felmeddelande}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Order {filnamn} flyttades till Fel: {felmeddelande}");
            Console.ResetColor();
        }
    }

    //Väntar tills ingen annan använder filen, till exempel medan den fortfarande kopieras in.
    private static void VäntaPåFil(string fil)
    {
        for (int i = 0; i < 10; i++)
        {
            try
            {
                using var ström = File.Open(fil, FileMode.Open, FileAccess.Read, FileShare.None);
                return;
            }
            catch (IOException)
            {
                if (i == 9) throw;
                Thread.Sleep(100);
            }
        }
    }

    //Flyttar en fil till en mapp och försöker igen om filen är upptagen.
    private static void FlyttaFil(string fil, string mapp)
    {
        string mål = Path.Combine(mapp, Path.GetFileName(fil));

        for (int i = 0; i < 10; i++)
        {
            try
            {
                File.Move(fil, mål, true);
                return;
            }
            catch (IOException)
            {
                if (i == 9) throw;
                Thread.Sleep(100);
            }
        }
    }

    //Skriver en rad med tidsstämpel till loggfilen.
    private static void Logga(string logPath, string meddelande)
    {
        File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {meddelande}{Environment.NewLine}");
    }
}