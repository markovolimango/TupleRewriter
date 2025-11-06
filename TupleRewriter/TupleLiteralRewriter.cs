namespace TupleRewriter;

public static class TupleLiteralRewriter
{
    /// <summary>
    ///     Replace tuple literals with 'new <typeName>(...)'.
    /// </summary>
    public static Block Rewrite(Block root, string typeName)
    {
        List<Stmt> l = [];
        foreach (var statement in root.Statements)
            l.Add(RewriteStmt(statement, typeName));
        return new Block(l);
    }

    private static Stmt RewriteStmt(Stmt stmt, string typeName)
    {
        switch (stmt)
        {
            case VarDecl varDecl:
                return new VarDecl(varDecl.Name, RewriteExpr(varDecl.Init, typeName));
            case Return ret:
                return new Return(RewriteExpr(ret.Value, typeName));
            case Block block:
                List<Stmt> l = [];
                foreach (var statement in block.Statements)
                    l.Add(RewriteStmt(statement, typeName));
                return new Block(l);
            default:
                return stmt;
        }
    }

    private static Expr RewriteExpr(Expr expr, string typeName)
    {
        List<Expr> l = [];
        switch (expr)
        {
            case TupleLiteral tupleLiteral:
                foreach (var element in tupleLiteral.Elements)
                    l.Add(RewriteExpr(element, typeName));
                return new NewExpr(typeName, l);
            case NewExpr newExpr:
                foreach (var arg in newExpr.Args)
                    l.Add(RewriteExpr(arg, typeName));
                return new NewExpr(newExpr.TypeName, l);
            default:
                return expr;
        }
    }
}