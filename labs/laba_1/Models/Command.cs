namespace GeneticSearching.Models;

public class Command
{
    public string CommandName { get; set; } = string.Empty;
    public string[] CommandParameters { get; set; } = Array.Empty<string>();
}
