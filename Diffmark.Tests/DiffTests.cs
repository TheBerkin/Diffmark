using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Diffmark.Tests
{
    [TestFixture]
    public class DiffTests
    {
        [TestCase("foo", "+bar", "foobar")]
        [TestCase("bar", "foo+", "foobar")]
        [TestCase("escape", "-ing", "escaping")]
        [TestCase("danger", "b-", "banger")]
        [TestCase("toilet", "out---", "outlet")]
        [TestCase("electricity", "-----ocute", "electrocute")]
        [TestCase("love", @"I\s+ ; +\sC#!", "I love C#!")]
        [TestCase("text", @"\-example\s+", "-example text")]
        [TestCase("The quick brown rabbit jumps over the lazy dog.", @"fox****", "The quick brown fox jumps over the lazy dog.")]
        [TestCase("Unit tests are awful.", @"*wonderful", "Unit tests are wonderful.")]
        public void Marking(string baseString, string pattern, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, Diff.Mark(baseString, pattern));
        }

        [TestCase(" xxx  bbb  ccc ", 1, "aaa", " aaa  bbb  ccc ")]
        [TestCase(" aaa  xxx  ccc ", 2, "bbb", " aaa  bbb  ccc ")]
        [TestCase(" aaa  bbb  xxx ", 3, "ccc", " aaa  bbb  ccc ")]
        [TestCase(" xxx  xxx  xxx ", 4, "yyy", " xxx  xxx  xxx ")]
        public void ReplaceWordPrepend(string baseString, int factor, string replacement, string expected)
        {
            Assert.AreEqual(expected, Diff.ReplaceWord(baseString, replacement, factor, true));
        }

        [TestCase(" xxx  bbb  ccc ", 3, "aaa", " aaa  bbb  ccc ")]
        [TestCase(" aaa  xxx  ccc ", 2, "bbb", " aaa  bbb  ccc ")]
        [TestCase(" aaa  bbb  xxx ", 1, "ccc", " aaa  bbb  ccc ")]
        [TestCase(" xxx  xxx  xxx ", 4, "yyy", " xxx  xxx  xxx ")]
        public void ReplaceWordAppend(string baseString, int factor, string replacement, string expected)
        {
            Assert.AreEqual(expected, Diff.ReplaceWord(baseString, replacement, factor, false));
        }

        [TestCase("qabcdef", "abcdg", "abcd")]
        public void LongestCommonSubstring(string a, string b, string converged)
        {
            int ia, ib, l;
            Assert.IsTrue(Diff.LongestCommonSubstring(a, b, out ia, out ib, out l));
            Assert.AreEqual(converged, a.Substring(ia, l));
            Assert.AreEqual(converged, b.Substring(ib, l));
        }

        [TestCase("punch", "punching", "ing")]
        [TestCase("alight", "flight", "f-")]
        [TestCase("sun", "sub", "-b")]
        [TestCase("wind", "unwind", "un+")]
        [TestCase("embolus", "emboli", "--i")]
        [TestCase("dance", "dancing", "-ing")]
        [TestCase("black", "slack", "s-")]
        [TestCase("fall", "winter", "*winter")]
        [TestCase("fathom", "unfathomable", "un+;able")]
        [TestCase("unreachable", "teacher", "t---;----er")]
        [TestCase("dislike", "liked", "|---;d")]
        [TestCase("aaacaaaa", "baaac", "b+;----")]
        public void Deriving(string before, string after, string expectedPattern)
        {
            var derived = Diff.Derive(before, after);
            Assert.AreEqual(derived, expectedPattern);
            Assert.AreEqual(Diff.Mark(before, derived), after);
        }
    }
}
