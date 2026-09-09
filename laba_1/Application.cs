using GeneticSearching.Models;
using GeneticSearching.Services;

namespace GeneticSearching;

public class Application
{
    private readonly FileDataCollector _dataCollector;
    private readonly FileWriter _fileWriter;
    private readonly CommandProcessor _commandProcessor;

    public Application()
    {
        _dataCollector = new FileDataCollector();
        _fileWriter = new FileWriter();
        _commandProcessor = new CommandProcessor(_fileWriter);
    }

    public void Run()
    {
        FilePaths files = _dataCollector.SelectFiles();

        if (files.FileId < 0 ||
            string.IsNullOrEmpty(files.SequencesFilePath) ||
            string.IsNullOrEmpty(files.CommandsFilePath))
        {
            return;
        }

        (List<Sequence> sequences, List<Command> commands) =
            _dataCollector.ReadData(files);

        _fileWriter.WriteHeader();
        _commandProcessor.Execute(sequences, commands);
        _fileWriter.GenerateFile(files.ContentPath, files.FileId);
    }
}
