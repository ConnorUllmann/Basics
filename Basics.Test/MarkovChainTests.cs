using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace Basics.Test
{
    public class MarkovChainTests
    {
        /// <summary>
        /// Adds names one at a time, counting consecutive duplicates.
        /// If a name is different from the previous, then a new line is created for it with a new count.
        /// Call .ToList() in order to get the corresponding text list of lines when printing "name" a 
        /// "count" number of times in one line.
        /// </summary>
        public class Sorter
        {
            public readonly List<(string name, int count)> lines = new List<(string, int)>();

            public void Add(params string[] names)
            {
                foreach (var name in names)
                    Add(name);
            }
            public void Add(string name)
            {
                if (string.IsNullOrEmpty(name))
                    return;

                if(lines.Count == 0 || lines.Last().name != name)
                {
                    lines.Add((name, 1));
                    return;
                }

                var lastLine = lines.Last();
                lines[lines.Count - 1] = (lastLine.name, lastLine.count +1);
            }

            public List<string> ToList()
            {
                var printLines = new List<string>();
                foreach(var line in lines)
                {
                    var printLine = string.Empty;
                    line.count.Repetitions(() => printLine += line.name);
                    printLines.Add(printLine);
                }
                return printLines;
            }
        }

        [Fact]
        public void AddLinkToString_Test()
        {
            var sorter = new Sorter();
            sorter.Add("A", "B", "", null, "A", "A", "B", "B", "B", "C");
            Assert.True(sorter.ToList().SequenceEqual(new List<string> { "A", "B", "AA", "BBB", "C" }));
        }

        [Fact]
        public void MarkovChain_Test_PrintToFile()
        {
            var states = new Dictionary<string, int>();
            var sorter = new Sorter();
            var dictionary = new Dictionary<string, (Action action, IDictionary<string, float> weightByName)>
            {
                ["A"] = (() => { states["A"]++; sorter.Add("A"); }, new Dictionary<string, float>
                {
                    ["A"] = 2,
                    ["B"] = 1
                }),
                ["B"] = (() => { states["B"]++; sorter.Add("B"); }, new Dictionary<string, float>
                {
                    ["A"] = 2,
                    ["B"] = 1,
                    ["C"] = 3
                }),
                ["C"] = (() => { states["C"]++; sorter.Add("C"); }, new Dictionary<string, float>
                {
                    ["B"] = 1,
                    ["C"] = 1
                })
            };
            var chain = MarkovChain.FromDictionary("A", dictionary);

            foreach (var key in dictionary.Keys)
                states.Add(key, 0);

            1000000.Repetitions(chain.Update);
            
            var debugLines = new List<string>();
            var sum = states.Values.Sum();
            foreach (var kv in states)
                debugLines.Add($"{kv.Key}: {kv.Value} = {Math.Round(100f * kv.Value / sum)}%");
            debugLines.Add(string.Empty);

            var printLines = sorter.ToList();
            printLines.InsertRange(0, debugLines);
            //var path = "C:/Users/Connor/Desktop/Development/Unity/Engine/markov-chain.txt";
            //System.IO.File.WriteAllLines(path, printLines);
        }
    }
}
