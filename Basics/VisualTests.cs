using System;
using System.Collections.Generic;
using System.Text;
using Basics.Sorting;
using System.Numerics;

namespace Basics
{
    public static class VisualTests
    {
        public static void Sort()
        {
            SortTests.TestOrdersOfMagnitudeForSortFunctions(new Dictionary<string, SortTests.Sort>()
            {
                ["Merge"] = SortFunctions.MergeSort,
                ["Quick"] = SortFunctions.QuickSort,
                ["Insertion"] = SortFunctions.InsertionSort,
                ["Heap"] = SortFunctions.HeapSort,
                ["Bubble"] = SortFunctions.BubbleSort
            });
            Wait();
        }

        public static void ExtractBatch()
        {
            var totalSize = 17;
            var batchSize = 7;

            var colorBefore = ConsoleColor.Red;
            var colorArrow = ConsoleColor.Yellow;
            var colorAfter = ConsoleColor.Green;

            var a = new List<string>();
            for (var i = 0; i < totalSize; i++)
                a.Add(i.ToString());

            Utils.Log(string.Join(", ", a));
            int count = 0;
            while (a.Count > 0)
            {
                Utils.Log(string.Join(", ", a.ExtractBatch(batchSize)), false, colorBefore);
                Utils.Log("   =>   ", false, colorArrow);
                Utils.Log(string.Join(", ", a), true, colorAfter);
                count++;
            }
            Wait();
        }

        public static void Pathfinding()
        {
            while (true)
            {
                PrintPaths();
                Wait();
            }
        }

        private static void PrintPaths()
        {
            var random = new Random();
            var objects = new List<List<ConsoleTile>>();
            var width = 15;
            var height = 11;
            for (var x = 0; x < width; x++)
            {
                objects.Add(new List<ConsoleTile>());
                for (var y = 0; y < height; y++)
                {
                    var tile = new ConsoleTile();
                    objects[x].Add(tile);
                    tile.Solid = random.Next(4) == 0;
                    tile.Value = tile.Solid ? 11 : 0;
                }
            }

            var grid = new ConsoleGrid(objects);
            grid.ForEachYX(tile => tile.DisplayValue = tile.Solid ? "11" : "  ");
            List<Vector2> objs = new List<Vector2>();
            for (var i = 0; i < 4; i+=2)
            {
                Vector2 a, b;
                do
                {
                    a = new Vector2(random.Next(0, width), random.Next(0, height));
                    b = new Vector2(random.Next(0, width), random.Next(0, height));
                } while (Utils.Distance(a, b) <= 10);
                objs.Add(a);
                objs.Add(b);
                var second = i == 2;
                var color = second ? ConsoleColor.Yellow : ConsoleColor.Red;
                Utils.Log($"({a.X}, {a.Y})", false, color);
                Utils.Log($" to ", false, ConsoleColor.DarkGray);
                Utils.Log($"({b.X}, {b.Y})     ", second, color);
            }
            objs.ForEach(c =>
            {
                var tile = grid.Get(c.X, c.Y);
                tile.Solid = false;
                tile.DisplayValue = "oo";
            });

            var tiles = Path.FindPath(grid, objs[2].X, objs[2].Y, objs[3].X, objs[3].Y, true);
            if (tiles != null)
                foreach (var tile in tiles)
                    tile.DisplayValue = Utils.PrependToLength(tile.DisplayValue ?? tile.Value.ToString(), 2)[0] + "*";

            tiles = Path.FindPath(grid, objs[0].X, objs[0].Y, objs[1].X, objs[1].Y, false);
            if (tiles != null)
                foreach (var tile in tiles)
                    tile.DisplayValue = "#" + Utils.PrependToLength(tile.DisplayValue ?? tile.Value.ToString(), 2)[1];

            grid.Print();
        }

        private static void Wait() => Console.ReadLine();
    }
}
