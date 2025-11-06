namespace TupleRewriter;

public static class Program
{
    public static void Main(string[] args)
    {
        var ast = new Block(new Stmt[]
        {
            new VarDecl(
                "x",
                new TupleLiteral(
                    [
                        new Num("1"),
                        new TupleLiteral(
                        [
                            new NewExpr("int", [new Num("17")]),
                            new Num("67")
                        ]),
                        new Num("3")
                    ]
                )
            ),
            new Return(
                new Id("x")
            )
        });

        Console.WriteLine(AstPrinter.PrintStmt(ast));
        ast = TupleLiteralRewriter.Rewrite(ast, "Class");
        Console.WriteLine(AstPrinter.PrintStmt(ast));
    }
}