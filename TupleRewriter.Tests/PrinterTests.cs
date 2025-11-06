using TupleRewriter.AST;

namespace TupleRewriter.Tests;

public class PrinterTests
{
    [Test]
    public void Print_ComplexNestedStructure()
    {
        var tuple = new TupleLiteral(new List<Expr> { new Num("1"), new Num("2") });
        var newExpr = new NewExpr("Wrapper", new List<Expr> { tuple });
        var innerBlock = new Block(new List<Stmt> { new Return(newExpr) });
        var outerBlock = new Block(new List<Stmt>
        {
            new VarDecl("x", new Num("0")),
            innerBlock
        });

        var result = ASTPrinter.Print(outerBlock);

        var expected =
            "{\n" +
            "    var x = 0;\n" +
            "    {\n" +
            "        return new Wrapper((1, 2));\n" +
            "    }\n" +
            "}";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Print_EmptyBlock()
    {
        var block = new Block(new List<Stmt>());

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n}"));
    }

    [Test]
    public void Print_EmptyTuple()
    {
        var tuple = new TupleLiteral(new List<Expr>());
        var varDecl = new VarDecl("empty", tuple);
        var block = new Block(new List<Stmt> { varDecl });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    var empty = ();\n}"));
    }

    [Test]
    public void Print_MultipleStatements()
    {
        var statements = new List<Stmt>
        {
            new VarDecl("x", new Num("1")),
            new VarDecl("y", new Num("2")),
            new Return(new Id("x"))
        };
        var block = new Block(statements);

        var result = ASTPrinter.Print(block);

        var expected = "{\n    var x = 1;\n    var y = 2;\n    return x;\n}";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Print_NestedBlock()
    {
        var innerBlock = new Block(new List<Stmt> { new VarDecl("y", new Num("2")) });
        var outerBlock = new Block(new List<Stmt>
        {
            new VarDecl("x", new Num("1")),
            innerBlock
        });

        var result = ASTPrinter.Print(outerBlock);

        var expected = "{\n    var x = 1;\n    {\n        var y = 2;\n    }\n}";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Print_NestedTuple()
    {
        var inner = new TupleLiteral(new List<Expr> { new Num("2"), new Num("3") });
        var outer = new TupleLiteral(new List<Expr> { new Num("1"), inner });
        var varDecl = new VarDecl("nested", outer);
        var block = new Block(new List<Stmt> { varDecl });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    var nested = (1, (2, 3));\n}"));
    }

    [Test]
    public void Print_NewExpression()
    {
        var newExpr = new NewExpr("Point", new List<Expr> { new Num("10"), new Num("20") });
        var varDecl = new VarDecl("p", newExpr);
        var block = new Block(new List<Stmt> { varDecl });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    var p = new Point(10, 20);\n}"));
    }

    [Test]
    public void Print_NewExpressionWithNoArgs()
    {
        var newExpr = new NewExpr("Object", new List<Expr>());
        var varDecl = new VarDecl("obj", newExpr);
        var block = new Block(new List<Stmt> { varDecl });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    var obj = new Object();\n}"));
    }

    [Test]
    public void Print_ReturnStatement()
    {
        var returnStmt = new Return(new Num("42"));
        var block = new Block(new List<Stmt> { returnStmt });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    return 42;\n}"));
    }

    [Test]
    public void Print_ReturnTuple()
    {
        var tuple = new TupleLiteral(new List<Expr> { new Id("x"), new Id("y") });
        var returnStmt = new Return(tuple);
        var block = new Block(new List<Stmt> { returnStmt });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    return (x, y);\n}"));
    }

    [Test]
    public void Print_SimpleTupleLiteral()
    {
        var tuple = new TupleLiteral(new List<Expr> { new Num("1"), new Num("2") });
        var varDecl = new VarDecl("p", tuple);
        var block = new Block(new List<Stmt> { varDecl });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    var p = (1, 2);\n}"));
    }

    [Test]
    public void Print_SimpleVarDecl()
    {
        var varDecl = new VarDecl("x", new Num("42"));
        var block = new Block(new List<Stmt> { varDecl });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    var x = 42;\n}"));
    }

    [Test]
    public void Print_VarDeclWithIdentifier()
    {
        var varDecl = new VarDecl("y", new Id("x"));
        var block = new Block(new List<Stmt> { varDecl });

        var result = ASTPrinter.Print(block);

        Assert.That(result, Is.EqualTo("{\n    var y = x;\n}"));
    }
}