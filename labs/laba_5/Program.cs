using System.Globalization;
using System.Text;
using laba_5.Interfaces;
using laba_5.Models;

namespace laba_5;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            var (minimumSugar, maximumSugar) = ReadSugarRange(args);
            var gift = CreateGift();

            Console.WriteLine(gift.Name);
            Console.WriteLine("Состав подарка:");
            PrintItems(gift);
            Console.WriteLine($"Общий вес: {gift.TotalWeightGrams:g} г");

            Console.WriteLine("\nКонфеты по возрастанию веса:");
            PrintItems(gift.GetCandiesSortedByWeight());

            Console.WriteLine($"\nКонфеты с содержанием сахара от {minimumSugar:g}% до {maximumSugar:g}%:");
            var matchingCandies = gift.FindCandiesBySugar(minimumSugar, maximumSugar);
            if (matchingCandies.Count == 0)
                Console.WriteLine("Подходящие конфеты не найдены.");
            else
                PrintItems(matchingCandies);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Ошибка: {exception.Message}");
        }
    }

    private static Gift CreateGift()
    {
        var gift = new Gift("Новогодний подарок");

        gift.Add(new ChocolateCandy("Алёнка", 45, 52, "сливочная", 70));
        gift.Add(new ChocolateCandy("Белочка", 25, 58, "ореховая", 55));
        gift.Add(new CaramelCandy("Барбарис", 15, 72, "барбарис"));
        gift.Add(new CaramelCandy("Дюшес", 14, 68, "груша"));
        gift.Add(new Marshmallow("Ванильный зефир", 40, 48, "ваниль"));

        return gift;
    }

    private static (double Minimum, double Maximum) ReadSugarRange(string[] args)
    {
        if (args.Length == 0)
            return (50, 70);

        if (args.Length != 2 ||
            !double.TryParse(args[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var minimum) ||
            !double.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var maximum))
        {
            throw new ArgumentException(
                "Укажите две границы содержания сахара, например: 50 70.");
        }

        return (minimum, maximum);
    }

    private static void PrintItems(IEnumerable<IGiftItem> items)
    {
        foreach (var item in items)
            Console.WriteLine($"- {item}");
    }
}
