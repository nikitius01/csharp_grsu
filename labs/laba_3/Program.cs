using System.Text;
using laba_3.Models;
using laba_3.Services;
using TextModel = laba_3.Models.Text;

namespace laba_3;

public static class Program
{
    private static readonly TextParser Parser = new();

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            var inputPath = args.Length > 0 ? args[0] : GetContentPath("input.txt");
            var stopWordsPath = args.Length > 1 ? args[1] : GetContentPath("stopwords_ru.txt");

            Run(inputPath, stopWordsPath);
        }
        catch (FileNotFoundException exception)
        {
            PrintError($"Файл не найден: {exception.FileName ?? exception.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            PrintError("Нет доступа к указанному файлу.");
        }
        catch (IOException exception)
        {
            PrintError($"Ошибка при работе с файлом: {exception.Message}");
        }
        catch (Exception exception)
        {
            PrintError(exception.Message);
        }
    }

    private static void Run(string inputPath, string stopWordsPath)
    {
        var text = Parser.ParseFile(inputPath);
        Console.WriteLine($"Загружено предложений: {text.Sentences.Count}");

        while (true)
        {
            PrintMenu();
            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            if (choice is null or "0")
                return;

            Execute(choice, text, stopWordsPath);
            Console.WriteLine();
        }
    }

    private static void Execute(string choice, TextModel text, string stopWordsPath)
    {
        switch (choice)
        {
            case "1":
                PrintSentences(text.OrderByWordCount());
                break;
            case "2":
                PrintSentences(text.OrderByLength());
                break;
            case "3":
                FindWordsInQuestions(text);
                break;
            case "4":
                RemoveWordsStartingWithConsonant(text);
                break;
            case "5":
                ReplaceWords(text);
                break;
            case "6":
                RemoveStopWords(text, stopWordsPath);
                break;
            case "7":
                ExportToXml(text);
                break;
            case "8":
                Console.WriteLine(text);
                break;
            default:
                Console.WriteLine("Неизвестный пункт меню.");
                break;
        }
    }

    private static void FindWordsInQuestions(TextModel text)
    {
        var length = ReadPositiveInt("Длина слова: ");
        var words = text.FindWordsInQuestions(length);

        Console.WriteLine(words.Count == 0
            ? "Слова не найдены."
            : string.Join(", ", words.Select(word => word.Value)));
    }

    private static void RemoveWordsStartingWithConsonant(TextModel text)
    {
        var length = ReadPositiveInt("Длина слова: ");
        var removedCount = text.RemoveWordsStartingWithConsonant(length);

        Console.WriteLine($"Удалено слов: {removedCount}");
        Console.WriteLine(text);
    }

    private static void ReplaceWords(TextModel text)
    {
        if (text.Sentences.Count == 0)
        {
            Console.WriteLine("В тексте нет предложений.");
            return;
        }

        var sentenceNumber = ReadIntInRange(
            $"Номер предложения (1-{text.Sentences.Count}): ",
            1,
            text.Sentences.Count);
        var length = ReadPositiveInt("Длина заменяемого слова: ");

        Console.Write("Новая подстрока: ");
        var replacement = Console.ReadLine() ?? string.Empty;

        var replacementCount = text.ReplaceWordsInSentence(sentenceNumber, length, replacement);
        Console.WriteLine($"Заменено слов: {replacementCount}");
        Console.WriteLine(text.Sentences[sentenceNumber - 1]);
    }

    private static void RemoveStopWords(TextModel text, string stopWordsPath)
    {
        var removedCount = text.RemoveStopWords(File.ReadLines(stopWordsPath));

        Console.WriteLine($"Удалено стоп-слов: {removedCount}");
        Console.WriteLine(text);
    }

    private static void ExportToXml(TextModel text)
    {
        Console.Write("Путь к XML (по умолчанию text.xml): ");
        var outputPath = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(outputPath))
            outputPath = "text.xml";

        text.ExportToXml(outputPath);
        Console.WriteLine($"XML сохранён: {Path.GetFullPath(outputPath)}");
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out var value) && value > 0)
                return value;

            Console.WriteLine("Введите положительное целое число.");
        }
    }

    private static int ReadIntInRange(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out var value) && value >= minimum && value <= maximum)
                return value;

            Console.WriteLine($"Введите число от {minimum} до {maximum}.");
        }
    }

    private static void PrintSentences(IEnumerable<Sentence> sentences)
    {
        var number = 1;
        foreach (var sentence in sentences)
            Console.WriteLine($"{number++}. {sentence}");
    }

    private static void PrintMenu()
    {
        Console.WriteLine("1. Предложения по количеству слов");
        Console.WriteLine("2. Предложения по длине");
        Console.WriteLine("3. Слова заданной длины в вопросительных предложениях");
        Console.WriteLine("4. Удалить слова, начинающиеся с согласной");
        Console.WriteLine("5. Заменить слова в выбранном предложении");
        Console.WriteLine("6. Удалить стоп-слова");
        Console.WriteLine("7. Экспортировать текст в XML");
        Console.WriteLine("8. Показать текущий текст");
        Console.WriteLine("0. Выход");
    }

    private static string GetContentPath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "content", fileName);

    private static void PrintError(string message) =>
        Console.Error.WriteLine($"Ошибка: {message}");
}
