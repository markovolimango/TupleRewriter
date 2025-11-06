using TupleRewriter.AST;

namespace TupleRewriter.Tests;

public class ParserTests
{
    [Test]
    public void Parse_ComplexExample()
    {
        var source = @"{
            var origin = (0, 0);
            var point = new Point(10, 20);
            var pair = (point, origin);
            return pair;
        }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(4));
        Assert.That(block.Statements[0], Is.TypeOf<VarDecl>());
        Assert.That(((VarDecl)block.Statements[1]).Name, Is.EqualTo("point"));
        Assert.That(((VarDecl)block.Statements[2]).Init, Is.TypeOf<TupleLiteral>());
        Assert.That(block.Statements[3], Is.TypeOf<Return>());
    }

    [Test]
    public void Parse_EmptyBlock()
    {
        var source = "{ }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements, Is.Empty);
    }

    [Test]
    public void Parse_EmptyTuple()
    {
        var source = "{ var empty = (); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var tuple = varDecl.Init as TupleLiteral;
        Assert.That(tuple, Is.Not.Null);
        Assert.That(tuple.Elements, Is.Empty);
    }

    [Test]
    public void Parse_ExcessiveWhitespace()
    {
        var source = @"
        {
            var   x   =   (   1   ,   2   )   ;
        }
        ";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(1));
    }

    [Test]
    public void Parse_InvalidExpression_ThrowsException()
    {
        var source = "{ var x = ,; }";
        var parser = new ASTParser(source);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Test]
    public void Parse_MissingEquals_ThrowsException()
    {
        var source = "{ var x 42; }";
        var parser = new ASTParser(source);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    // ===== ERROR CASES =====

    [Test]
    public void Parse_MissingSemicolon_ThrowsException()
    {
        var source = "{ var x = 1 }";
        var parser = new ASTParser(source);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Test]
    public void Parse_MultipleStatements()
    {
        var source = @"{
            var x = 1;
            var y = 2;
            return x;
        }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(3));
        Assert.That(block.Statements[0], Is.TypeOf<VarDecl>());
        Assert.That(block.Statements[1], Is.TypeOf<VarDecl>());
        Assert.That(block.Statements[2], Is.TypeOf<Return>());
    }

    [Test]
    public void Parse_NestedBlock()
    {
        var source = @"{
            var x = 1;
            {
                var y = 2;
            }
        }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(2));
        Assert.That(block.Statements[0], Is.TypeOf<VarDecl>());
        var innerBlock = block.Statements[1] as Block;
        Assert.That(innerBlock, Is.Not.Null);
        Assert.That(innerBlock.Statements.Count, Is.EqualTo(1));
    }

    [Test]
    public void Parse_NestedTuple()
    {
        var source = "{ var nested = (1, (2, 3)); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var outer = varDecl.Init as TupleLiteral;
        Assert.That(outer.Elements.Count, Is.EqualTo(2));
        Assert.That(outer.Elements[0], Is.TypeOf<Num>());

        var inner = outer.Elements[1] as TupleLiteral;
        Assert.That(inner, Is.Not.Null);
        Assert.That(inner.Elements.Count, Is.EqualTo(2));
    }

    [Test]
    public void Parse_NewExpression()
    {
        var source = "{ var p = new Point(1, 2); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var newExpr = varDecl.Init as NewExpr;
        Assert.That(newExpr, Is.Not.Null);
        Assert.That(newExpr.TypeName, Is.EqualTo("Point"));
        Assert.That(newExpr.Args.Count, Is.EqualTo(2));
    }

    [Test]
    public void Parse_NewExpressionWithNoArgs()
    {
        var source = "{ var obj = new Object(); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var newExpr = varDecl.Init as NewExpr;
        Assert.That(newExpr.TypeName, Is.EqualTo("Object"));
        Assert.That(newExpr.Args, Is.Empty);
    }

    [Test]
    public void Parse_NewExpressionWithTupleArg()
    {
        var source = "{ var w = new Wrapper((1, 2)); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var newExpr = varDecl.Init as NewExpr;
        Assert.That(newExpr.Args.Count, Is.EqualTo(1));
        Assert.That(newExpr.Args[0], Is.TypeOf<TupleLiteral>());
    }

    [Test]
    public void Parse_NoWhitespace()
    {
        var source = "{var x=(1,2);}";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(1));
    }

    [Test]
    public void Parse_ReturnStatement()
    {
        var source = "{ return 42; }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(1));
        var returnStmt = block.Statements[0] as Return;
        Assert.That(returnStmt, Is.Not.Null);
        Assert.That(returnStmt.Value, Is.TypeOf<Num>());
    }

    [Test]
    public void Parse_ReturnTuple()
    {
        var source = "{ return (x, y); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var returnStmt = block.Statements[0] as Return;
        var tuple = returnStmt.Value as TupleLiteral;
        Assert.That(tuple, Is.Not.Null);
        Assert.That(tuple.Elements.Count, Is.EqualTo(2));
    }

    [Test]
    public void Parse_SimpleTupleLiteral()
    {
        var source = "{ var p = (1, 2); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var tuple = varDecl.Init as TupleLiteral;
        Assert.That(tuple, Is.Not.Null);
        Assert.That(tuple.Elements.Count, Is.EqualTo(2));
        Assert.That(tuple.Elements[0], Is.TypeOf<Num>());
        Assert.That(tuple.Elements[1], Is.TypeOf<Num>());
    }

    [Test]
    public void Parse_SimpleVarDecl()
    {
        var source = "{ var x = 42; }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(1));
        var varDecl = block.Statements[0] as VarDecl;
        Assert.That(varDecl, Is.Not.Null);
        Assert.That(varDecl.Name, Is.EqualTo("x"));
        Assert.That(varDecl.Init, Is.TypeOf<Num>());
        Assert.That(((Num)varDecl.Init).Text, Is.EqualTo("42"));
    }

    [Test]
    public void Parse_TupleWithMixedTypes()
    {
        var source = "{ var mixed = (x, 42, y); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var tuple = varDecl.Init as TupleLiteral;
        Assert.That(tuple.Elements.Count, Is.EqualTo(3));
        Assert.That(tuple.Elements[0], Is.TypeOf<Id>());
        Assert.That(tuple.Elements[1], Is.TypeOf<Num>());
        Assert.That(tuple.Elements[2], Is.TypeOf<Id>());
    }

    [Test]
    public void Parse_UnclosedBlock_ThrowsException()
    {
        var source = "{ var x = 1;";
        var parser = new ASTParser(source);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Test]
    public void Parse_UnclosedTuple_ThrowsException()
    {
        var source = "{ var x = (1, 2; }";
        var parser = new ASTParser(source);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Test]
    public void Parse_UnderscoresInIdentifiers()
    {
        var source = "{ var _my_var = other_var; }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        Assert.That(varDecl.Name, Is.EqualTo("_my_var"));
        var id = varDecl.Init as Id;
        Assert.That(id.Name, Is.EqualTo("other_var"));
    }

    [Test]
    public void Parse_VarDeclWithIdentifier()
    {
        var source = "{ var y = x; }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        Assert.That(varDecl.Name, Is.EqualTo("y"));
        Assert.That(varDecl.Init, Is.TypeOf<Id>());
        Assert.That(((Id)varDecl.Init).Name, Is.EqualTo("x"));
    }

    [Test]
    public void Parse_DeeplyNestedBlocks()
    {
        var source = "{ { { { var x = 1; } } } }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var level1 = block.Statements[0] as Block;
        var level2 = level1.Statements[0] as Block;
        var level3 = level2.Statements[0] as Block;
        Assert.That(level3.Statements.Count, Is.EqualTo(1));
    }

    [Test]
    public void Parse_DeeplyNestedTuples()
    {
        var source = "{ var deep = (1, (2, (3, (4, 5)))); }";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var tuple = varDecl.Init as TupleLiteral;
        Assert.That(tuple.Elements.Count, Is.EqualTo(2));
    }

    [Test]
    public void Parse_ManyStatements()
    {
        var statements = string.Join("\n",
            Enumerable.Range(1, 50).Select(i => $"var x{i} = {i};"));
        var source = $"{{ {statements} }}";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        Assert.That(block.Statements.Count, Is.EqualTo(50));
    }

    [Test]
    public void Parse_VeryLongTuple()
    {
        var elements = string.Join(", ", Enumerable.Range(1, 100));
        var source = $"{{ var big = ({elements}); }}";
        var parser = new ASTParser(source);

        var block = parser.Parse();

        var varDecl = block.Statements[0] as VarDecl;
        var tuple = varDecl.Init as TupleLiteral;
        Assert.That(tuple.Elements.Count, Is.EqualTo(100));
    }
}