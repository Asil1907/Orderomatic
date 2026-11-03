public class Settings
{
    public string Log { get; set; } = "";

    public List<DirectoryRule> Directories { get; set; } = new();
}