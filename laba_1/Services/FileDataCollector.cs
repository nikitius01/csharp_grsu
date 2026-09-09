using System.Text;
using GeneticSearching.Models;

namespace GeneticSearching.Services;

public class FileDataCollector
{
    private readonly string _contentPath;

    public FileDataCollector()
    {
        _contentPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "content");
    }

    public FilePaths SelectFiles()
    {
        string[] sequenceFiles =
            Directory.GetFiles(_contentPath, "sequences*.txt");

        if (sequenceFiles.Length == 0)
        {
            Console.WriteLine("Not found any sequences files");
            return new FilePaths(_contentPath, string.Empty, string.Empty, -1);
        }

        Console.WriteLine("Choose sequences file:");

        for (int i = 0; i < sequenceFiles.Length; i++)
            Console.WriteLine($"{i + 1}) {Path.GetFileNameWithoutExtension(sequenceFiles[i])}");

        int fileId = ReadFileId(sequenceFiles.Length);
        string sequencesFilePath = sequenceFiles[fileId];

        string commandsFilePath =
            Path.Combine(_contentPath, $"commands.{fileId}.txt");

        if (!File.Exists(commandsFilePath))
        {
            Console.WriteLine("No commands file for this sequences file!");
            commandsFilePath = string.Empty;
        }

        return new FilePaths(
            _contentPath,
            sequencesFilePath,
            commandsFilePath,
            fileId);
    }

    public (List<Sequence> Sequences, List<Command> Commands) ReadData(
        FilePaths paths)
    {
        var sequences = new List<Sequence>();
        var commands = new List<Command>();

        if (!File.Exists(paths.SequencesFilePath))
        {
            Console.WriteLine(
                $"Sequences file not found: {paths.SequencesFilePath}");
            return (sequences, commands);
        }

        if (!File.Exists(paths.CommandsFilePath))
        {
            Console.WriteLine(
                $"Commands file not found: {paths.CommandsFilePath}");
            return (sequences, commands);
        }

        ReadSequences(paths.SequencesFilePath, sequences);
        ReadCommands(paths.CommandsFilePath, commands);

        return (sequences, commands);
    }

    private void ReadSequences(
        string filePath,
        List<Sequence> sequences)
    {
        foreach (string line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] tokens = line.Split(
                '	',
                StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length < 3)
            {
                Console.WriteLine(
                    $"Invalid sequence line skipped: {line}");
                continue;
            }

            sequences.Add(new Sequence
            {
                ProteinName = tokens[0],
                OrganismName = tokens[1],
                ProteinSequence = DecodeRLESequence(tokens[2])
            });
        }
    }

    private void ReadCommands(
        string filePath,
        List<Command> commands)
    {
        foreach (string line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] tokens = line.Split(
                '	',
                StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
            {
                Console.WriteLine(
                    $"Invalid command line skipped: {line}");
                continue;
            }

            var command = new Command
            {
                CommandName = tokens[0],
                CommandParameters = tokens.Skip(1).ToArray()
            };

            if (command.CommandName.Equals(
                    "search",
                    StringComparison.OrdinalIgnoreCase) &&
                command.CommandParameters.Length > 0)
            {
                command.CommandParameters[0] =
                    DecodeRLESequence(command.CommandParameters[0]);
            }

            commands.Add(command);
        }
    }

    private static string DecodeRLESequence(string proteinSequence)
    {
        if (string.IsNullOrEmpty(proteinSequence))
            return string.Empty;

        var result = new StringBuilder();
        int i = 0;

        while (i < proteinSequence.Length)
        {
            if (char.IsDigit(proteinSequence[i]))
            {
                int start = i;

                while (i < proteinSequence.Length &&
                       char.IsDigit(proteinSequence[i]))
                {
                    i++;
                }

                if (i >= proteinSequence.Length)
                    throw new FormatException(
                        "RLE string ends with number but no character follows.");

                int number = int.Parse(proteinSequence[start..i]);

                result.Append(proteinSequence[i], number);
                i++;
            }
            else
            {
                result.Append(proteinSequence[i]);
                i++;
            }
        }

        return result.ToString();
    }

    private static int ReadFileId(int filesCount)
    {
        while (true)
        {
            Console.Write(">>> ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int selectedFile) &&
                selectedFile >= 1 &&
                selectedFile <= filesCount)
            {
                return selectedFile - 1;
            }

            Console.WriteLine("Wrong input!");
        }
    }
}
