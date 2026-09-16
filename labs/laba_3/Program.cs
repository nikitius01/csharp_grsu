using System.Text;

namespace laba_3;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            var inputPath = args.Length > 0 ? args[0] : GetContentPath("input.txt");
            var stopWordsPath = args.Length > 1 ? args[1] : GetContentPath("stopwords_ru.txt");

            var application = new ConsoleApplication();
            application.Run(inputPath, stopWordsPath);
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

    private static string GetContentPath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "content", fileName);

    private static void PrintError(string message) =>
        Console.Error.WriteLine($"Ошибка: {message}");
}
