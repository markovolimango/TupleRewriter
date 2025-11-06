namespace TupleRewriter;

public static class Program
{
    public static void Main(string[] args)
    {
        var parser = new ASTParser(File.ReadAllText("../../../example.txt"));

        try
        {
            var root = parser.Parse();
            Console.WriteLine(AstPrinter.PrintStmt(root));
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid input file.");
        }
    }
}