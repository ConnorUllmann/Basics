using System;
using System.Collections.Generic;
using System.Text;

namespace Basics
{
    public class ConsoleTile : ISolidTile
    {
        public bool Solid { get; set; }
        public int Value { get; set; }
        public string DisplayValue { get; set; }
        
        private static ConsoleColor[] Colors = new ConsoleColor[]
        {
            ConsoleColor.Magenta,
            ConsoleColor.Cyan,
            ConsoleColor.Yellow,
            ConsoleColor.Red,
            ConsoleColor.Green
        };

        public void Print(string suffix = null)
        {
            if (!string.IsNullOrEmpty(DisplayValue) && DisplayValue[0] == '#' && DisplayValue[1] != '*') DisplayValue = $"{DisplayValue[0]} ";
            if (!string.IsNullOrEmpty(DisplayValue) && DisplayValue[0] != '#' && DisplayValue[1] == '*') DisplayValue = $" {DisplayValue[1]}";

            var colorIndex0 = Solid ? 0 : (string.IsNullOrEmpty(DisplayValue) ? 1 : (DisplayValue[0] == '#' ? 3 : 4));
            var colorIndex1 = Solid ? 0 : (string.IsNullOrEmpty(DisplayValue) ? 1 : (DisplayValue[1] == '*' ? 2 : 4));

            Utils.Log((Solid ? "[" : " ") + (string.IsNullOrEmpty(DisplayValue) ? ' ' : DisplayValue[0]), false, Colors[colorIndex0]);
            Utils.Log((string.IsNullOrEmpty(DisplayValue) ? ' ' : DisplayValue[1]) + (Solid ? "]" : " ") + (suffix != null ? suffix : ""), false, Colors[colorIndex1]);
        }
        
    }

    public class ConsoleGrid : Grid<ConsoleTile>
    {
        public ConsoleGrid(int _width, int _height) : base(_width, _height) { }
        public ConsoleGrid(List<List<ConsoleTile>> _tiles) : base(_tiles) { }
        public void Print() => ForEachY(x => { x.ForEach(tile => { tile.Print(" "); }); Utils.Log("\n"); });
    }
}
