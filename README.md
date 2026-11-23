# Orderomatic

<img src="logo.png" alt="Orderomatic" width="120">

Automatisk inläsning och validering av kundorder i XML-filer, med konvertering till CSV, felhantering och loggning – byggt som en vidareutveckling av Sortomatic i C# och .NET 8.

## Om projektet

Jag har byggt ihop tidigare skoljobb till ett sammanhängande flöde där sorteringsprogrammet och Blackbox samarbetar. Sorteringsprogrammet håller koll på inkorgsmappen och sorterar vanliga filer som vanligt, men när det dyker upp en orderfil i XML-format skickas den vidare till Blackbox för kontroll och omvandling till CSV. Om allt ser bra ut sparas ordern som behandlad, medan trasiga filer hamnar i en felmapp och loggas utan att resten av programmet stannar. Nästa steg är att koppla på lagerhanteringen, ett tredje skoljobb, så att behandlade order också påverkar lagersaldot. Genom att koppla ihop de här separata delarna har jag fått ett mer komplett flöde som liknar hur system hänger ihop i praktiken.

## Bakgrund

Ett tillverkande företag tar emot kundorder som XML-filer i en inkorgsmapp. Idag behöver någon manuellt hålla koll på mappen, öppna eller kontrollera filerna och föra över informationen till ett format som passar vidare hantering eller export. Det är tidskrävande och gör flödet skört: en felaktig eller ofullständig XML-fil kan fastna, missas eller ställa till det om den behandlas som en giltig order utan kontroll.

Orderomatic löser det genom att automatiskt bevaka inkorgsmappar och låta ett separat konverteringsprogram (Blackbox) läsa varje orderfil innan den flyttas vidare. Giltiga order omvandlas till CSV för vidare export och arkiveras i en mapp för behandlade order, medan trasiga order sorteras till en separat felmapp och loggas utan att stoppa resten av flödet.

## Så fungerar det

```
Order\Input  ──►  Blackbox  ──►  returkod 0             ──►  Behandlade  +  CSV i Export
                              └►  annan kod / tidsgräns  ──►  Fel          +  rad i loggen
```

1. När en orderfil hamnar i Input-mappen upptäcker Orderomatic filen och väntar tills skrivningen är klar innan den hanteras. Filer som redan ligger i mappen när programmet startar hanteras också.
2. Orderomatic startar Blackbox med två argument: XML-filen och sökvägen där CSV-filen ska skapas i Export-mappen.
3. Vid lyckad konvertering (returkod 0) flyttas XML-filen till mappen för behandlade order, och den färdiga CSV-filen ligger kvar i Export-mappen.
4. Om Blackbox returnerar något annat än 0, eller inte blir klar inom 10 sekunder, flyttas XML-filen i stället till felmappen. En eventuell CSV-fil tas bort, felmeddelandet från Blackbox skrivs till loggen och visas i konsolen, och programmet fortsätter köra.

Vanliga filer (till exempel bilder och dokument) sorteras fortfarande efter filändelse, precis som i Sortomatic som projektet bygger på.

## Projektets delar

| Projekt | Ansvar |
|---|---|
| `Orderomatic` | Bevakar konfigurerade mappar, väntar på att filer är färdigskrivna, startar Blackbox med tidsgräns, flyttar filer baserat på returkod, hanterar vanlig filsortering och loggar händelser |
| `Blackbox` | Läser en order i XML, skriver orderraderna till en CSV-fil och returnerar 0 vid lyckad hantering eller 1 vid fel. Kan också köras fristående: `dotnet run --project Blackbox -- <xml-fil> <csv-fil>` |
| `Exempel/` | Exempelorder att testa med: två giltiga orderfiler och `trasig.xml`, som triggar felhanteringen |

## Krav

* [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Kom igång

1. Bygg båda projekten:

   ```
   dotnet build Orderomatic.slnx
   ```

2. Skapa mapparna för order, till exempel `Order\Input`, `Order\Behandlade`, `Order\Export` och `Order\Fel`, samt mapparna för eventuell vanlig filsortering. Alla mappar måste finnas innan programmet startas.
3. Fyll i `Orderomatic/Inställningar.xml` (se nedan) och bygg igen så att filen kopieras till programmets mapp.
4. Starta programmet:

   ```
   dotnet run --project Orderomatic
   ```

5. Kopiera in en fil från `Exempel/` i Input-mappen. En giltig order flyttas till mappen för behandlade order och ger en CSV-fil i Export. `trasig.xml` flyttas till Fel, och felet visas i konsolen och i loggen. Programmet fortsätter köra tills du avslutar det med Ctrl+C.

## Inställningar

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Settings>
    <Log>C:\Order\log.txt</Log>

    <Directory>
        <Name>Kundorder</Name>
        <Input>C:\Order\Input</Input>
        <Output>C:\Order\Behandlade</Output>
        <Export>C:\Order\Export</Export>
        <Error>C:\Order\Fel</Error>
        <Convert>C:\Orderomatic\Blackbox\bin\Debug\net8.0\Blackbox.exe</Convert>
        <Type>.xml</Type>
    </Directory>

    <Directory>
        <Name>Bilder</Name>
        <Input>C:\Sortering\Inkorg</Input>
        <Output>C:\Sortering\Bilder</Output>
        <Type>.jpg</Type>
        <Type>.png</Type>
    </Directory>
</Settings>
```

| Element | Beskrivning |
|---|---|
| `<Settings>` | Rotelementet. Allt annat ligger inuti det. |
| `<Log>` | Sökväg till loggfilen. Filen skapas om den inte finns, men mappen måste redan finnas. |
| `<Directory>` | En regel. Det kan finnas en eller flera. |
| `<Name>` | Ett läsbart namn på regeln som används i utskrifter och i loggen. |
| `<Input>` | Mappen som bevakas för nya filer. |
| `<Output>` | För order: mappen för behandlade XML-filer. För vanlig sortering: mappen som filerna flyttas till. |
| `<Type>` | En filändelse som regeln hanterar, till exempel `.xml` eller `.jpg`. Det kan finnas flera. Stora och små bokstäver spelar ingen roll. |
| `<Convert>` | Valfri. Hela sökvägen till `Blackbox.exe`. Gör regeln till en orderregel. |
| `<Export>` | Krävs om `<Convert>` finns. Mappen där den färdiga CSV-filen placeras. |
| `<Error>` | Krävs om `<Convert>` finns. Mappen dit orderfiler flyttas när konverteringen misslyckas. |

Vid uppstart kontrolleras att alla element finns, att alla mappar och `Blackbox.exe` finns, och att Export- och felmappen inte är samma mapp som Input. Annars avslutas programmet med ett felmeddelande. `Inställningar.xml` laddas om automatiskt när filen ändras under körning; är den nya filen ogiltig behålls de gamla inställningarna.

> **Obs:** Programmet läser filen som ligger bredvid programmet (i `bin`-mappen). Det är den filen du ska ändra medan programmet körs.

## Designval

- **Returkoder mellan programmen:** Att Orderomatic och Blackbox kommunicerar via returkoder gör gränssnittet tydligt och löst kopplat. Som separata program isoleras XML-tolkning och konvertering i en egen process: om en felaktig eller oväntad fil orsakar ett fel i Blackbox påverkas inte bevakningen i Orderomatic, och Blackbox kan bytas ut eller testas fristående.
- **En felmapp i stället för att stoppa:** I ett löpande orderflöde får en enskild korrupt kundfil inte blockera efterföljande order. Genom att flytta trasiga filer till en felmapp och logga felet kan verksamheten fortsätta ta emot giltiga order medan felsökning sker i lugn takt.
- **Ingen halvfärdig CSV:** Export-mappen ska bara innehålla färdiga order, annars riskerar nästa steg i kedjan att läsa in ofullständig data. Det finns två skydd: Blackbox läser in hela XML-filen innan något skrivs, så en trasig XML ger ingen CSV alls. Och om konverteringen ändå misslyckas tar Orderomatic bort en eventuell CSV-fil innan ordern flyttas till Fel.
- **Vänta på att filen är klar:** När en fil dyker upp i en bevakad mapp kan ett externt system eller en kopiering fortfarande skriva till den. Orderomatic försöker öppna filen exklusivt och väntar tills det går, så att Blackbox inte läser en halv XML-fil.
- **Tidsgräns:** En tidsgräns på 10 sekunder skyddar flödet mot att Blackbox hänger sig. Om tiden går ut avbryts processen och filen hanteras som misslyckad i stället för att frysa hela orderkön.
- **Inget är hårdkodat:** Mappar, sökvägen till Blackbox och regler läses från `Inställningar.xml` i stället för att hårdkodas. Samma byggda program kan därför köras i olika miljöer och ändras utan omkompilering.

## Loggen

Loggen skrivs till filen som anges i `<Log>`. Varje rad har en tidsstämpel och beskriver vad som hänt med en fil, så att man kan spåra varje order:

```
2026-11-23 13:10:50 - Konverterade 1.xml till 1.csv
2026-11-23 13:10:52 - Flyttade bild.jpg till Bilder
2026-11-23 13:10:52 - Kunde inte konvertera trasig.xml: Något gick fel: Unexpected end of file has occurred. The following elements are not closed: Order. Line 544, position 1.
2026-11-23 13:10:52 - Konverterade 2.xml till 2.csv
```

## Vidareutveckling

- Räkna om ordervärdet till SEK med ett valutakurs-API, och hantera saknade kurser eller timeout utan att ordern stoppas
- Dra orderraderna från ett lagersaldo eller kontrollera artiklar och lagersaldo innan ordern exporteras
- Lägga till enhetstester för konvertering, felhantering och inställningar, och köra dem i en CI-pipeline (till exempel på Linux)

## Användning av AI

Jag har använt Claude som stöd för kodexempel, felsökning och dokumentation.
