using System;

namespace Diffmark
{
    internal class Token
    {
        public readonly DM Type;
        public readonly string Value;

        public Token(DM type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    internal enum DM
    {
        Add,
        Subtract,
        ReplaceWord,
        Escape,
        Text,
        Delimiter
    }
}