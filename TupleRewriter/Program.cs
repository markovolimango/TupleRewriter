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
            var newRoot = TupleLiteralRewriter.Rewrite(root, "Class");
            File.WriteAllText(filename, ASTPrinter.Print(newRoot));
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid input file.");
        }
    }
}