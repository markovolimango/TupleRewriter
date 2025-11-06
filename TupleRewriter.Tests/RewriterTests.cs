using TupleRewriter.AST;

namespace TupleRewriter.Tests;

public class Tests
{
    [Test]
    public void Rewriter_RewritesAllTuples()
    {
        var root = new Block([
            new VarDecl(
                "x",
                new TupleLiteral([
                    new Num("1"),
                    new Num("2"),
                    new Num("3")
                ])
            ),
            new Block([
                new VarDecl(
                    "y",
                    new TupleLiteral([
                        new Num("4"),
                        new TupleLiteral([
                            new Num("5"),
                            new TupleLiteral([
                                new Num("6"),
                                new Num("7")
                            ]),
                            new Id("x")
                        ])
                    ])
                ),
                new Return(
                    new TupleLiteral([
                        new Id("x"),
                        new Id("y")
                    ])
                )
            ])
        ]);

        var typename = "Class";
        var newRoot = TupleLiteralRewriter.Rewrite(root, typename);

        Assert.That(newRoot.Statements.Count, Is.EqualTo(2));

        var xDecl = newRoot.Statements[0] as VarDecl;
        var xInit = xDecl.Init as NewExpr;

        Assert.That(xDecl.Init, Is.TypeOf<NewExpr>());
        Assert.That(xInit.TypeName, Is.EqualTo(typename));
        Assert.That(xInit.Args, Has.Count.EqualTo(3));

        Assert.That(newRoot.Statements[1], Is.TypeOf<Block>());
        Assert.That((newRoot.Statements[1] as Block).Statements, Has.Count.EqualTo(2));

        var yDecl = (newRoot.Statements[1] as Block).Statements[0] as VarDecl;
        var yInit = yDecl.Init as NewExpr;

        Assert.That(yDecl.Init, Is.TypeOf<NewExpr>());
        Assert.That(yInit.TypeName, Is.EqualTo(typename));

        var nested1 = yInit.Args[1];

        Assert.That(nested1, Is.TypeOf<NewExpr>());
        Assert.That((nested1 as NewExpr).Args[2], Is.EqualTo(new Id("x")));

        var nested2 = (nested1 as NewExpr).Args[1] as NewExpr;

        Assert.That(nested2.Args, Has.Count.EqualTo(2));
        Assert.That(nested2.Args[0], Is.EqualTo(new Num("6")));
    }

    [Test]
    public void Rewriter_DoesntChangeOtherExpressions()
    {
        var xInit = new NewExpr("Type", [new Num("1"), new Num("2"), new Num("3")]);
        var xDecl = new VarDecl("x", xInit);
        var yDecl = new VarDecl("y", new Num("4"));
        var zDecl = new VarDecl("z", new Id("y"));
        var ret = new Return(new NewExpr("Type", [new Id("x")]));

        var root = new Block([
            xDecl,
            yDecl,
            zDecl,
            ret
        ]);

        var typename = "Class";
        var newRoot = TupleLiteralRewriter.Rewrite(root, typename);

        Assert.That(newRoot.Statements, Has.Count.EqualTo(4));

        var newXDecl = newRoot.Statements[0] as VarDecl;
        var newXInit = newXDecl.Init as NewExpr;

        Assert.That(newXDecl.Name, Is.EqualTo("x"));
        Assert.That(newXDecl.Init, Is.TypeOf<NewExpr>());
        Assert.That(newXInit.Args[0], Is.EqualTo(xInit.Args[0]));
        Assert.That(newXInit.Args[1], Is.EqualTo(xInit.Args[1]));
        Assert.That(newXInit.Args[2], Is.EqualTo(xInit.Args[2]));

        Assert.That(newRoot.Statements[1], Is.EqualTo(yDecl));

        Assert.That(newRoot.Statements[2], Is.EqualTo(zDecl));

        Assert.That((newRoot.Statements[3] as Return).Value, Is.TypeOf<NewExpr>());
    }

    [Test]
    public void Rewriter_WorksOnEmptyBlock()
    {
        var root = new Block([]);
        var typename = "Class";
        var newRoot = TupleLiteralRewriter.Rewrite(root, typename);
        Assert.That(newRoot.Statements, Is.Empty);
    }
}