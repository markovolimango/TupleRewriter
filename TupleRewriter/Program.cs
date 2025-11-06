using TupleRewriter.AST;

namespace TupleRewriter;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.Write("Enter filename: ");
        var filename = Console.ReadLine();
        if (!File.Exists(filename))
        {
            Console.WriteLine("File not found.");
            return;
        }

        var parser = new ASTParser(File.ReadAllText(filename));

        try
        {
            var root = parser.Parse();

            Console.Write("Enter type name: ");
            var typename = Console.ReadLine();
            var newRoot = TupleLiteralRewriter.Rewrite(root, typename);
            File.WriteAllText(filename, ASTPrinter.Print(newRoot));
            Console.WriteLine("Tuples successfully rewritten.");
        }
        catch (ParseException e)
        {
            Console.WriteLine($"Invalid input file: {e.Message}");
        }
    }
}