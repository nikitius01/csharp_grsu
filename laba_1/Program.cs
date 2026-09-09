using System.Text;

namespace GeneticSearching;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var application = new Application();
        application.Run();
    }
}
