using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diffmark
{
    internal static class Lexer
    {
        private static IEnumerable<Token> GetTokens(string patternString)
        {
            patternString = patternString.Trim();
            var text = new StringBuilder();
            Token nextToken = null;
            for (int i = 0; i < patternString.Length; i++)
            {
                switch (patternString[i])
                {
                    case '\\':
                        nextToken = new Token(DM.Escape, Escape(patternString[++i]).ToString());
                        break;
                    case '+':
                        nextToken = new Token(DM.Add, "+");
                        break;
                    case '-':
                        nextToken = new Token(DM.Subtract, "-");
                        break;
                    case '*':
                        nextToken = new Token(DM.ReplaceWord, "*");
                        break;
                    case ';':
                        nextToken = new Token(DM.Delimiter, ";");
                        break;
                    case ' ':
                        if (text.Length > 0) goto default;
                        continue;
                    default:
                        text.Append(patternString[i]);
                        continue;
                }
                if (text.Length > 0)
                {
                    yield return new Token(DM.Text, text.ToString().Trim());
                    text.Clear();
                }
                yield return nextToken;
                nextToken = null;
            }
            if (text.Length > 0)
            {
                yield return new Token(DM.Text, text.ToString().Trim());
            }
        }

        public static IEnumerable<IEnumerable<Token>> Lex(string patternString)
        {
            var tokens = GetTokens(patternString);
            var list = new List<Token>();
            foreach (var token in tokens)
            {
                if (token.Type == DM.Delimiter && list.Any())
                {
                    yield return list.ToArray();
                    list.Clear();
                }
                else
                {
                    list.Add(token);
                }
            }
            if (list.Any()) yield return list.ToArray();
        }

        internal static char Escape(char escapeChar)
        {
            switch (escapeChar)
            {
                case 'n':
                    return '\n';
                case 's':
                    return ' ';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                default:
                    return escapeChar;
            }
        }
    }
}