namespace TupleRewriter;

public class ASTParser
{
    private readonly List<string> tokens;

    public ASTParser(string source)
    {
        tokens = Tokenize(source);
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
                token += source[i];
                while (char.IsLetterOrDigit(source[i]) || source[i] == '_')
                    token += source[i++];
                tokens.Add(token);
            }

        Console.WriteLine(tokens.ToString());
        return tokens;
    }
}