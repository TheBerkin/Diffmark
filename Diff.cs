using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diffmark
{
    /// <summary>
    /// Diffs your marks.
    /// </summary>
    public sealed class Diff
    {
        private static readonly Dictionary<char, DiffRuleType> RuleMap = new Dictionary<char, DiffRuleType>()
        {
            {'*', DiffRuleType.ReplaceWord},
            {'-', DiffRuleType.Subtract},
            {'+', DiffRuleType.Add}  
        }; 

        private readonly string _patternString;
        private readonly Rule[] _rules;

        internal class Rule
        {
            public readonly string ConcatString;
            public readonly bool Prepend;
            public readonly DiffRuleType Type;
            public readonly int Factor;

            public Rule(string concatString, bool prepend, DiffRuleType type, int factor)
            {
                ConcatString = concatString;
                Prepend = prepend;
                Type = type;
                Factor = factor;
            }

            public static Rule Parse(string ruleString)
            {
                if (String.IsNullOrWhiteSpace(ruleString)) return null;
                DiffRuleType ruleType;
                ruleString = ruleString.Trim();
                char opChar;
                bool prepend = !RuleMap.TryGetValue(opChar = ruleString[0], out ruleType);
                if (prepend && !RuleMap.TryGetValue(opChar = ruleString[ruleString.Length - 1], out ruleType))
                {
                    prepend = false;
                    ruleType = DiffRuleType.Add;
                    opChar = '+';
                }

                int factor = 0;

                if (ruleType != DiffRuleType.Add)
                {
                    if (prepend)
                    {   
                        factor += ruleString.Reverse().TakeWhile(t => t == opChar).Count();
                    }
                    else
                    {
                        factor += ruleString.TakeWhile(t => t == opChar).Count();
                    }
                }
                else
                {
                    factor++;
                }

                return new Rule(prepend ? ruleString.TrimEnd(opChar) : ruleString.TrimStart(opChar), prepend, ruleType, factor);
            }
        }

        internal enum DiffRuleType
        {
            Add,
            Subtract,
            ReplaceWord
        }


        /// <summary>
        /// The pattern string for the diff.
        /// </summary>
        public string Pattern => _patternString;

        /// <summary>
        /// Creates a new Diffmark pattern for you to enjoy.
        /// </summary>
        /// <param name="patternString">The pattern string.</param>
        public Diff(string patternString)
        {
            if (String.IsNullOrEmpty(patternString))
                throw new ArgumentException("Pattern string cannot be null nor empty.");

            _patternString = patternString;
            _rules = SplitStatements(patternString).Select(Rule.Parse).ToArray();
        }

        public string Mark(string baseString)
        {
            foreach (var rule in _rules)
            {
                switch (rule.Type)
                {
                    case DiffRuleType.Add:
                        baseString = rule.Prepend
                            ? rule.ConcatString + baseString
                            : baseString + rule.ConcatString;
                        continue;
                    case DiffRuleType.Subtract:
                        baseString = Cut(baseString, rule.Factor, rule.Prepend);
                        baseString = rule.Prepend
                            ? rule.ConcatString + baseString
                            : baseString + rule.ConcatString;
                        continue;
                }
            }
            return baseString;
        }

        public static string Mark(string baseString, string pattern)
        {
            return new Diff(pattern).Mark(baseString);
        }

        internal static string Cut(string baseString, int factor, bool prepend)
        {
            if (factor > baseString.Length) return String.Empty;
            return prepend ? baseString.Substring(factor) : baseString.Substring(0, baseString.Length - factor);
        }

        internal static IEnumerable<string> SplitStatements(string pattern)
        {
            if (String.IsNullOrWhiteSpace(pattern)) yield break;
            int end = 0;
            int length = pattern.Length;
            var sb = new StringBuilder();
            while (end < length)
            {
                switch (pattern[end])
                {
                    case '\\':
                        end++;
                        sb.Append(Escape(pattern[end++]));
                        break;
                    case ';':
                        yield return sb.ToString();
                        sb.Clear();
                        end++;
                        break;
                    default:
                        sb.Append(pattern[end++]);
                        break;
                }
            }
            if (sb.Length > 0) yield return sb.ToString();
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
