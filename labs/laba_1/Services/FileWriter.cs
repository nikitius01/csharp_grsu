using System.Text;

namespace GeneticSearching.Services;

public class FileWriter
{
    private readonly StringBuilder _content = new();

    public void WriteHeader()
    {
        _content.Append(
            "Yakimovich Nikita\n" +
            "Genetic Searching\n" +
            new string('-', 74) +
            "\n");
    }

    public void WriteCommand(
        int commandId,
        string commandName,
        string[] parameters)
    {
        string parameterText = string.Join("   ", parameters);

        _content.Append(
            $"{commandId:D3}   {commandName}   {parameterText}\n");
    }

    public void WriteLine(string text)
    {
        _content.Append(text).Append('\n');
    }

    public void WriteSeparator()
    {
        _content.Append('-', 74).Append('\n');
    }

    public void GenerateFile(string directory, int fileId)
    {
        string outputPath =
            Path.Combine(directory, $"genedata.{fileId}.txt");

        File.WriteAllText(outputPath, _content.ToString());
        Console.WriteLine($"File Saved: {outputPath}");
    }
}
