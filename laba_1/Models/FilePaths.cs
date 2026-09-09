namespace GeneticSearching.Models;

public class FilePaths
{
    public string ContentPath { get; }
    public string SequencesFilePath { get; }
    public string CommandsFilePath { get; }
    public int FileId { get; }

    public FilePaths(
        string contentPath,
        string sequencesFilePath,
        string commandsFilePath,
        int fileId)
    {
        ContentPath = contentPath;
        SequencesFilePath = sequencesFilePath;
        CommandsFilePath = commandsFilePath;
        FileId = fileId;
    }
}
