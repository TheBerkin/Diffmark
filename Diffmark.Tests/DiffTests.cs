using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Diffmark.Tests
{
    [TestFixture]
    public class DiffTests
    {
        private static string Combine(string[] parts)
        {
            if (!parts.Any()) return String.Empty;
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb
                    .Append("'")
                    .Append(parts[i])
                    .Append("'");
            }
            return sb.ToString();
        }

        [Test]
        public void StatementSplit()
        {
            var split = Diff.SplitStatements(@"abc;def;ghi").ToArray();
            Console.WriteLine("Parts: \{Combine(split)}\n");
            Assert.AreEqual(3, split.Length, "Expected 3 statements.");
            Assert.AreEqual(split[0], "abc");
            Assert.AreEqual(split[1], "def");
            Assert.AreEqual(split[2], "ghi");
        }

        [Test]
        public void EscapedSplit()
        {   
            var split = Diff.SplitStatements(@"abc;def\;;\;ghi;\;jkl\;;\;\;\;").ToArray();
            Console.WriteLine("Parts: \{Combine(split)}\n");
            Assert.AreEqual(5, split.Length, "Expected 4 statements.");
            Assert.AreEqual(split[0], @"abc");
            Assert.AreEqual(split[1], @"def;");
            Assert.AreEqual(split[2], @";ghi");
            Assert.AreEqual(split[3], @";jkl;");
            Assert.AreEqual(split[4], @";;;");
        }

        [Test]
        public void PrependAddRule()
        {
            var rule = Diff.Rule.Parse("hello+");
            Assert.IsTrue(rule.Prepend, "Rule is appending.");
            Assert.AreEqual(1, rule.Factor, "Factor should be 1.");
            Assert.AreEqual("hello", rule.ConcatString, "Wrong concat string.");
            Assert.AreEqual(Diff.DiffRuleType.Add, rule.Type, "Wrong rule type.");
        }

        [Test]
        public void AppendAddRule()
        {
            var rule = Diff.Rule.Parse("+hello");
            Assert.IsFalse(rule.Prepend, "Rule is prepending.");
            Assert.AreEqual(1, rule.Factor, "Factor should be 1.");
            Assert.AreEqual("hello", rule.ConcatString, "Wrong concat string.");
            Assert.AreEqual(Diff.DiffRuleType.Add, rule.Type, "Wrong rule type.");
        }

        [Test]
        public void AppendImplicitAddRule()
        {
            var rule = Diff.Rule.Parse("hello");
            Assert.IsFalse(rule.Prepend, "Rule is prepending.");
            Assert.AreEqual(1, rule.Factor, "Factor should be 1.");
            Assert.AreEqual("hello", rule.ConcatString, "Wrong concat string.");
            Assert.AreEqual(Diff.DiffRuleType.Add, rule.Type, "Wrong rule type.");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void PrependDeleteRule(int expectedFactor)
        {
            var rule = Diff.Rule.Parse("hello" + new string('-', expectedFactor));
            Assert.IsTrue(rule.Prepend, "Rule is appending.");
            Assert.AreEqual(expectedFactor, rule.Factor, "Factor should be \{expectedFactor}.");
            Assert.AreEqual("hello", rule.ConcatString, "Wrong concat string.");
            Assert.AreEqual(Diff.DiffRuleType.Subtract, rule.Type, "Wrong rule type.");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void AppendDeleteRule(int expectedFactor)
        {
            var rule = Diff.Rule.Parse(new string('-', expectedFactor) + "hello");
            Assert.IsFalse(rule.Prepend, "Rule is prepending.");
            Assert.AreEqual(expectedFactor, rule.Factor, "Factor should be \{expectedFactor}.");
            Assert.AreEqual("hello", rule.ConcatString, "Wrong concat string.");
            Assert.AreEqual(Diff.DiffRuleType.Subtract, rule.Type, "Wrong rule type.");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void PrependReplaceWordRule(int expectedFactor)
        {
            var rule = Diff.Rule.Parse("hello" + new string('*', expectedFactor));
            Assert.IsTrue(rule.Prepend, "Rule is appending.");
            Assert.AreEqual(expectedFactor, rule.Factor, "Factor should be \{expectedFactor}.");
            Assert.AreEqual("hello", rule.ConcatString, "Wrong concat string.");
            Assert.AreEqual(Diff.DiffRuleType.ReplaceWord, rule.Type, "Wrong rule type.");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void AppendReplaceWordRule(int expectedFactor)
        {
            var rule = Diff.Rule.Parse(new string('*', expectedFactor) + "hello");
            Assert.IsFalse(rule.Prepend, "Rule is prepending.");
            Assert.AreEqual(expectedFactor, rule.Factor, "Factor should be \{expectedFactor}.");
            Assert.AreEqual("hello", rule.ConcatString, "Wrong concat string.");
            Assert.AreEqual(Diff.DiffRuleType.ReplaceWord, rule.Type, "Wrong rule type.");
        }

        [TestCase("foo", "+bar", "foobar")]
        [TestCase("bar", "foo+", "foobar")]
        [TestCase("escape", "-ing", "escaping")]
        [TestCase("danger", "b-", "banger")]
        [TestCase("toilet", "out---", "outlet")]
        [TestCase("electricity", "-----ocute", "electrocute")]
        [TestCase("love", "I + ; + C#!", "I love C#!")]
        public void Marking(string baseString, string pattern, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, Diff.Mark(baseString, pattern));
        }
    }
}
