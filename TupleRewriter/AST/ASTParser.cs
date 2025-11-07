namespace TupleRewriter.AST;

public class ASTParser
{
    private readonly List<string> _tokens;
    private int i;

    public ASTParser(string source)
    {
        _tokens = Tokenize(source);
    }

    public Block Parse()
    {
        return ParseBlockStmt();
    }

    private string PeekToken()
    {
        if (i >= _tokens.Count)
            throw new ParseException("Unexpected end of input.");
        return _tokens[i];
    }

    private string GetToken(string? expected = null)
    {
        if (i >= _tokens.Count)
            throw new ParseException("Unexpected end of input.");
        if (expected is null)
            return _tokens[i++];
        if (_tokens[i] != expected)
            throw new ParseException($"Expected {expected}, got {_tokens[i]}.");
        return _tokens[i++];
    }

    private Expr ParseExpr()
    {
        var token = PeekToken();
        if (token == "new")
            return ParseNewExpr();
        if (char.IsLetter(token[0]) || token[0] == '_')
            return ParseIdExpr();
        if (char.IsNumber(token[0]))
            return ParseNumExpr();
        if (token == "(")
            return ParseTupleLiteralExpr();
        throw new ParseException($"Invalid expression {token}.");
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
        if (PeekToken() == ")")
        {
            GetToken(")");
            return new TupleLiteral(elements);
        }

        elements.Add(ParseExpr());
        while (PeekToken() != ")")
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

        if (PeekToken() == ")")
        {
            GetToken(")");
            return new NewExpr(typeName, args);
        }

        args.Add(ParseExpr());
        while (PeekToken() != ")")
        {
            GetToken(",");
            args.Add(ParseExpr());
        }

        GetToken(")");
        return new NewExpr(typeName, args);
    }

    private Stmt ParseStmt()
    {
        var token = PeekToken();
        if (token == "var")
            return ParseVarDeclStmt();
        if (token == "return")
            return ParseReturnStmt();
        if (token == "{")
            return ParseBlockStmt();
        throw new ParseException($"Invalid statement {token}.");
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
        while (PeekToken() != "}")
        {
            var statement = ParseStmt();
            statements.Add(statement);
            if (statement is not Block)
                GetToken(";");
        }

        GetToken("}");
        return new Block(statements);
    }

    private List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var i = 0;

        while (i < input.Length)
            if (char.IsWhiteSpace(input[i]))
            {
                i++;
            }
            else if (char.IsLetterOrDigit(input[i]) || input[i] == '_')
            {
                var token = "";
                token += input[i++];
                while ((i < input.Length && char.IsLetterOrDigit(input[i])) || input[i] == '_')
                    token += input[i++];
                tokens.Add(token);
            }
            else
            {
                tokens.Add(input[i++].ToString());
            }

        return tokens;
    }
}

public class ParseException : Exception
{
    public ParseException(string message) : base(message)
    {
    }
}