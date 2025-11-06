namespace TupleRewriter;

public abstract record Stmt;

public sealed record VarDecl(string Name, Expr Init) : Stmt;

public sealed record Return(Expr Value) : Stmt;

public sealed record Block(IReadOnlyList<Stmt> Statements) : Stmt;