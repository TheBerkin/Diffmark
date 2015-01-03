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
        public void Marking(string baseString, string pattern, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, Diff.Mark(baseString, pattern));
        }
    }
}
