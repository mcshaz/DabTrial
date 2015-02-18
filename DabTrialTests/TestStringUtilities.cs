using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DabTrial.Utilities;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
namespace DabTrialTests
{
    [TestClass]
    public class TestStringUtilities
    {
        [TestMethod]
        public void ToSeparateWords()
        {
            Assert.AreEqual("Test This Camel Case", "TestThisCamelCase".ToSeparateWords());
            Assert.AreEqual("Test TLA Containing Camel Case", "TestTLAContainingCamelCase".ToSeparateWords());
            Assert.AreEqual("Ends With TLA", "EndsWithTLA".ToSeparateWords());
            Assert.AreEqual("Ends With A", "EndsWithA".ToSeparateWords());
            Assert.AreEqual("AA", "AA".ToSeparateWords());
            Assert.AreEqual("Aa", "Aa".ToSeparateWords());
            Assert.AreEqual("a A", "aA".ToSeparateWords());
            Assert.AreEqual("a", "a".ToSeparateWords());
            Assert.AreEqual("A", "A".ToSeparateWords());
        }
        static string AndyRosePascal(string instr)
        {
            return new string(instr.SelectMany((c, i) => i != 0 && char.IsUpper(c) && !char.IsUpper(instr[i - 1]) ? new char[] { ' ', c } : new char[] { c }).ToArray());
        }

        static IEnumerable<string> DanTaoSolution(string text)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(text))
            {
                while (reader.Peek() != -1)
                {
                    char c = (char)reader.Read();
                    if (char.IsUpper(c) && sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Length = 0;
                    }

                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
                yield return sb.ToString();
        }
        [TestMethod]
        public void PerformanceTestToSeparateWords()
        {
            const int reps = 1000000;
            const string pattern = "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))";
            const string testString = "TestTLAContainingCamelCase";
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for(int i=0;i<reps;i++ )
            {
                Regex.Replace(testString, pattern, "$1 ");
            }
            Console.WriteLine("static regex:{0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            Regex testRegex = new Regex(pattern);
            for (int i = 0; i < reps; i++)
            {
                testRegex.Replace(testString, "$1 ");
            }
            Console.WriteLine("Regex instance:{0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            testRegex = new Regex(pattern, RegexOptions.Compiled);
            for (int i = 0; i < reps; i++)
            {
                Regex.Replace(testString, pattern, "$1 ");
            }
            Console.WriteLine("compiled regex:{0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            for (int i = 0; i < reps; i++)
            {
                StringExtensions.ToSeparateWords(testString);
            }
            Console.WriteLine("code (char array):{0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            for (int i = 0; i < reps; i++)
            {
                AndyRosePascal(testString);
            }
            Console.WriteLine("AndyRose:{0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            for (int i = 0; i < reps; i++)
            {
                string outstr = string.Join("",DanTaoSolution(testString));
            }
            Console.WriteLine("DanTao:{0}ms", sw.ElapsedMilliseconds);
        }
    }
}
