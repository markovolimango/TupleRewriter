namespace TupleRewriter;

public class ASTParser
{
    private readonly List<string> tokens;
    private int i;

    public ASTParser(string source)
    {
        tokens = Tokenize(source);
    }

    public Block Parse()
    {
        return ParseBlockStmt();
    }

    private string GetToken(string? expected = null)
    {
        if (expected is null)
            return tokens[i++];
        if (tokens[i] != expected)
            throw new Exception("Expected " + expected);
        return tokens[i++];
    }

    private Expr ParseExpr()
    {
        if (tokens[i] == "new")
            return ParseNewExpr();
        if (char.IsLetter(tokens[i][0]) || tokens[i][0] == '_')
            return ParseIdExpr();
        if (char.IsNumber(tokens[i][0]))
            return ParseNumExpr();
        if (tokens[i] == "(")
            return ParseTupleLiteralExpr();
        throw new Exception("Not a valid expression");
    }

    private Id ParseIdExpr()
    {
        var id = GetToken();
        return new Id(id);
    }

    private Num ParseNumExpr()
    {
        var num = GetToken();
        return new Num(num);
    }

    private TupleLiteral ParseTupleLiteralExpr()
    {
        GetToken("(");
        var elements = new List<Expr>();
        if (tokens[i] == ")")
        {
            GetToken(")");
            return new TupleLiteral(elements);
        }

        elements.Add(ParseExpr());
        while (tokens[i] != ")")
        {
            GetToken(",");
            elements.Add(ParseExpr());
        }

        GetToken(")");
        return new TupleLiteral(elements);
    }

    private NewExpr ParseNewExpr()
    {
        GetToken("new");
        var typeName = GetToken();
        GetToken("(");
        var args = new List<Expr>();

        if (tokens[i] == ")")
        {
            GetToken(")");
            return new NewExpr(typeName, args);
        }

        args.Add(ParseExpr());
        while (tokens[i] != ")")
        {
            GetToken(",");
            args.Add(ParseExpr());
        }

        GetToken(")");
        return new NewExpr(typeName, args);
    }

    private Stmt ParseStmt()
    {
        if (tokens[i] == "var")
            return ParseVarDeclStmt();
        if (tokens[i] == "return")
            return ParseReturnStmt();
        if (tokens[i] == "{")
            return ParseBlockStmt();
        throw new Exception("Not a valid statement");
    }

    private VarDecl ParseVarDeclStmt()
    {
        GetToken("var");
        var name = GetToken();
        GetToken("=");
        var init = ParseExpr();
        return new VarDecl(name, init);
    }

    private Return ParseReturnStmt()
    {
        GetToken("return");
        var value = ParseExpr();
        return new Return(value);
    }

    private Block ParseBlockStmt()
    {
        GetToken("{");
        var statements = new List<Stmt>();
        while (tokens[i] != "}")
        {
            statements.Add(ParseStmt());
            GetToken(";");
        }

        GetToken("}");
        return new Block(statements);
    }

    private List<string> Tokenize(string source)
    {
        var tokens = new List<string>();
        var i = 0;

        while (i < source.Length)
            if (char.IsWhiteSpace(source[i]))
            {
                i++;
            }
            else if (char.IsLetterOrDigit(source[i]) || source[i] == '_')
            {
                var token = "";
                token += source[i++];
                while ((i < source.Length && char.IsLetterOrDigit(source[i])) || source[i] == '_')
                    token += source[i++];
                tokens.Add(token);
            }
            else
            {
                tokens.Add(source[i++].ToString());
            }

        return tokens;
    }
}