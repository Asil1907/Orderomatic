public class DirectoryRule
{
 

    public string Name { get; set; } = "";
    public string Input { get; set; } = "";
    public string Output { get; set; } = "";

    public string? Convert { get; set; }

    public string? Export { get; set;  }

    public string? Error { get; set; }
    public List<string> Types { get; set; } = new();
}
