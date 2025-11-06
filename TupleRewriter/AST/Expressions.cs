namespace TupleRewriter.AST;

public abstract record Expr;

public sealed record Id(string Name) : Expr;

public sealed record Num(string Text) : Expr;

public sealed record TupleLiteral(IReadOnlyList<Expr> Elements) : Expr;

public sealed record NewExpr(string TypeName, IReadOnlyList<Expr> Args) : Expr;