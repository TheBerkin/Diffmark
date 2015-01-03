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
        private static readonly Dictionary<DM, DiffRuleType> RuleMap = new Dictionary<DM, DiffRuleType>()
        {
            {DM.ReplaceWord, DiffRuleType.ReplaceWord},
            {DM.Subtract, DiffRuleType.Subtract},
            {DM.Add, DiffRuleType.Add}
        };

        private readonly string _patternString;
        private readonly Rule[] _rules;

        internal class Rule
        {
            public readonly string ConcatString;
            public readonly bool Prepend;
            public readonly DiffRuleType Type;
            public readonly int Factor;

            private Rule(string concatString, bool prepend, DiffRuleType type, int factor)
            {
                ConcatString = concatString;
                Prepend = prepend;
                Type = type;
                Factor = factor;
            }

            public static Rule Parse(Token[] tokens)
            {
                DiffRuleType ruleType;
                DM op;
                bool prepend = !RuleMap.TryGetValue(op = tokens[0].Type, out ruleType);
                if (prepend && !RuleMap.TryGetValue(op = tokens[tokens.Length - 1].Type, out ruleType))
                {
                    prepend = false;
                    ruleType = DiffRuleType.Add;
                }

                int factor = 0;

                if (ruleType != DiffRuleType.Add)
                {
                    if (prepend)
                    {
                        factor += tokens.Reverse().TakeWhile(t => t.Type == op).Count();
                    }
                    else
                    {
                        factor += tokens.TakeWhile(t => t.Type == op).Count();
                    }
                }
                else
                {
                    factor++;
                }

                var sb = new StringBuilder();

                foreach (var token in 
                    tokens.SkipWhile(t => RuleMap.ContainsKey(t.Type))
                        .Reverse()
                        .SkipWhile(t => RuleMap.ContainsKey(t.Type))
                        .Reverse())
                {
                    sb.Append(token.Value);
                }

                return new Rule(sb.ToString(), prepend, ruleType, factor);
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
            _rules = Lexer.Lex(patternString).Select(tokens => Rule.Parse(tokens.ToArray())).ToArray();
        }

        /// <summary>
        /// Applies the pattern to a string.
        /// </summary>
        /// <param name="baseString">The string to apply the pattern to.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Transforms one string to another.
        /// </summary>
        /// <param name="baseString">The base string to transform.</param>
        /// <param name="pattern">The Diffmark pattern to apply to the string.</param>
        /// <returns></returns>
        public static string Mark(string baseString, string pattern)
        {
            return new Diff(pattern).Mark(baseString);
        }

        internal static string Cut(string baseString, int factor, bool prepend)
        {
            if (factor > baseString.Length) return String.Empty;
            return prepend ? baseString.Substring(factor) : baseString.Substring(0, baseString.Length - factor);
        }
    }
}
