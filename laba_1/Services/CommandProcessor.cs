using GeneticSearching.Models;

namespace GeneticSearching.Services;

public class CommandProcessor
{
    private readonly FileWriter _fileWriter;
    private int _commandId = 1;

    public CommandProcessor(FileWriter fileWriter)
    {
        _fileWriter = fileWriter;
    }

    public void Execute(
        List<Sequence> sequences,
        List<Command> commands)
    {
        foreach (Command command in commands)
        {
            switch (command.CommandName.ToLowerInvariant())
            {
                case "search":
                    ExecuteSearch(sequences, command);
                    break;

                case "diff":
                    ExecuteDiff(sequences, command);
                    break;

                case "mode":
                    ExecuteMode(sequences, command);
                    break;

                default:
                    _fileWriter.WriteLine(
                        $"Unknown command: {command.CommandName}");
                    break;
            }

            _fileWriter.WriteSeparator();
        }
    }

    private void ExecuteSearch(
        List<Sequence> sequences,
        Command command)
    {
        _fileWriter.WriteCommand(
            _commandId++,
            command.CommandName,
            command.CommandParameters);

        if (command.CommandParameters.Length == 0)
        {
            _fileWriter.WriteLine("NOT FOUND");
            return;
        }

        string searchSequence = command.CommandParameters[0];

        var matches = sequences.Where(sequence =>
            sequence.ProteinSequence.Contains(searchSequence));

        _fileWriter.WriteLine("organism\t\t\t\t\tprotein");

        if (matches.Any())
        {
            foreach (Sequence sequence in matches)
            {
                _fileWriter.WriteLine(
                    $"{sequence.OrganismName}\t\t{sequence.ProteinName}");
            }
        }
        else
        {
            _fileWriter.WriteLine("NOT FOUND");
        }
    }

    private void ExecuteDiff(
        List<Sequence> sequences,
        Command command)
    {
        _fileWriter.WriteCommand(
            _commandId++,
            command.CommandName,
            command.CommandParameters);

        _fileWriter.WriteLine("amino-acids difference:");

        if (command.CommandParameters.Length < 2)
        {
            _fileWriter.WriteLine("MISSING: command parameters");
            return;
        }

        Sequence? firstProtein = sequences.FirstOrDefault(sequence =>
            sequence.ProteinName.Contains(command.CommandParameters[0]));

        Sequence? secondProtein = sequences.FirstOrDefault(sequence =>
            sequence.ProteinName.Contains(command.CommandParameters[1]));

        if (firstProtein != null && secondProtein != null)
        {
            Sequence longerSequence = firstProtein;
            Sequence shorterSequence = secondProtein;

            if (longerSequence.ProteinSequence.Length <
                secondProtein.ProteinSequence.Length)
            {
                longerSequence = secondProtein;
                shorterSequence = firstProtein;
            }

            int difference = 0;

            for (int i = 0;
                 i < shorterSequence.ProteinSequence.Length;
                 i++)
            {
                if (shorterSequence.ProteinSequence[i] !=
                    longerSequence.ProteinSequence[i])
                {
                    difference++;
                }
            }

            difference +=
                longerSequence.ProteinSequence.Length -
                shorterSequence.ProteinSequence.Length;

            _fileWriter.WriteLine(difference.ToString());
        }
        else
        {
            if (firstProtein == null && secondProtein == null)
            {
                _fileWriter.WriteLine(
                    $"MISSING:\t{command.CommandParameters[0]}\t" +
                    $"{command.CommandParameters[1]}");
            }
            else if (firstProtein == null)
            {
                _fileWriter.WriteLine(
                    $"MISSING:\t{command.CommandParameters[0]}");
            }
            else
            {
                _fileWriter.WriteLine(
                    $"MISSING:\t{command.CommandParameters[1]}");
            }
        }
    }

    private void ExecuteMode(
        List<Sequence> sequences,
        Command command)
    {
        _fileWriter.WriteCommand(
            _commandId++,
            command.CommandName,
            command.CommandParameters);

        _fileWriter.WriteLine("amino-acid occurs:");

        if (command.CommandParameters.Length == 0)
        {
            _fileWriter.WriteLine("MISSING");
            return;
        }

        Sequence? sequence = sequences.FirstOrDefault(item =>
            item.ProteinName.Contains(command.CommandParameters[0]));

        if (sequence != null)
        {
            var result = sequence.ProteinSequence
                .GroupBy(character => character)
                .Select(group => new
                {
                    Key = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Key)
                .First();

            _fileWriter.WriteLine(
                $"{result.Key}          {result.Count}");
        }
        else
        {
            _fileWriter.WriteLine(
                $"MISSING: {command.CommandParameters[0]}");
        }
    }
}
