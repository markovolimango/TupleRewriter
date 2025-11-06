namespace TupleRewriter;

public static class AstPrinter
{
    private static string PrintElements(IReadOnlyList<Expr> elements)
    {
        if (elements.Count == 0)
            return "()";

        var res = "(";
        res += PrintExpr(elements[0]);
        foreach (var element in elements.Skip(1))
            res += $", {PrintExpr(element)}";
        return res + ")";
    }

    private static string PrintExpr(Expr expr)
    {
        switch (expr)
        {
            case Id id:
                return id.Name;
            case Num num:
                return num.Text;
            case TupleLiteral tupleLiteral:
                return PrintElements(tupleLiteral.Elements);
            case NewExpr newExpr:
                return $"new {newExpr.TypeName}{PrintElements(newExpr.Args)}";
            default:
                throw new NotImplementedException();
        }
    }

    public static string PrintStmt(Stmt stmt, int indent = 0)
    {
        var ind = new string(' ', indent * 4);
        switch (stmt)
        {
            case VarDecl varDecl:
                return $"{ind}var {varDecl.Name} = {PrintExpr(varDecl.Init)};";
            case Return ret:
                return $"{ind}return {PrintExpr(ret.Value)};";
            case Block block:
                var res = $"{ind}{{\n";
                foreach (var blockStmt in block.Statements)
                    res += $"{PrintStmt(blockStmt, indent + 1)}\n";
                return res + "}";
            default:
                throw new NotImplementedException();
        }
    }
}