using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Basics.Test
{
    public class GridTests
    {
        private class Tile : ISolidTile, IPosition
        {
            public float X { get; set; }
            public float Y { get; set; }

            public bool Solid { get; set; }

            public Tile(int i, int j)
            {
                X = i;
                Y = j;
            }
        }

        [Fact]
        public void Grid_Clear()
        {
            var grid = new Grid<Tile>(5, 5, (i, j) => new Tile(i, j));
            grid.Clear();
            grid.ForEachXY(Assert.Null);
        }

        [Theory]
        [InlineData(-1, 0, false)]
        [InlineData(0, -1, false)]
        [InlineData(0, 0, true)]
        [InlineData(3, 2, true)]
        [InlineData(4, 4, true)]
        [InlineData(5, 4, false)]
        [InlineData(4, 5, false)]
        public void Grid_Inside(int x, int y, bool expected)
        {
            var grid = new Grid<Tile>(5, 5, (i, j) => new Tile(i, j));
            var actual = grid.Inside(x, y);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Grid_GetAndSet_Single_InBounds()
        {
            var grid = new Grid<Tile>(5, 5);
            (int x, int y) = (3, 2);
            var expected = new Tile(x, y);
            grid.Set(expected, x, y);
            var actual = grid.Get(x, y);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1, 2)]
        [InlineData(2, -1)]
        [InlineData(5, 3)]
        [InlineData(3, 5)]
        public void Grid_GetAndSet_Single_OutOfBounds(int x, int y)
        {
            var grid = new Grid<Tile>(5, 5);
            var expected = new Tile(x, y);
            grid.Set(expected, x, y);
            var actual = grid.Get(x, y);
            Assert.Null(actual);
            grid.ForEachXY(Assert.Null);
        }

        [Theory]
        [InlineData(3, 4, 6, 10, 2)]
        [InlineData(-2, -1, 3, 4, 3)]
        [InlineData(1, 1, 3, 2, 6)]
        [InlineData(0, -1, 1, 1, 0)]
        [InlineData(-1, 0, 1, 1, 0)]
        [InlineData(0, 5, 1, 1, 0)]
        [InlineData(5, 0, 1, 1, 0)]
        public void Grid_GetAndSet_Rectangle_InBoundsAndOutOfBounds(int x, int y, int w, int h, int count)
        {
            var grid = new Grid<Tile>(5, 5);
            var expected = new Tile(x, y);
            grid.Set(expected, x, y, w, h);
            var actual = grid.Get(x, y, w, h);
            Assert.Equal(count, actual?.Count);
            Assert.True(actual.TrueForAll(o => expected == o));
        }

        [Theory]
        [InlineData(1, 1, 4)]
        [InlineData(0, 1, 3)]
        [InlineData(1, 0, 3)]
        [InlineData(0, 0, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(2, 3, 1)]
        [InlineData(3, 2, 1)]
        [InlineData(-1, 0, 1)]
        [InlineData(0, -1, 1)]
        [InlineData(-1, -1, 0)]
        [InlineData(3, 3, 0)]
        public void Grid_GetNeighborsCardinal(int x, int y, int count)
        {
            var grid = new Grid<Tile>(3, 3, (i, j) => new Tile(i, j));
            var tiles = grid.GetNeighborsCardinal(x, y);
            Assert.Equal(count, tiles?.Count);
            Assert.True(tiles.TrueForAll(t => t.Distance(x, y) == 1));
        }

        [Theory]
        [InlineData(1, 1, 8)]
        [InlineData(0, 1, 5)]
        [InlineData(1, 0, 5)]
        [InlineData(0, 0, 3)]
        [InlineData(2, 2, 3)]
        [InlineData(2, 3, 2)]
        [InlineData(3, 2, 2)]
        [InlineData(-1, 0, 2)]
        [InlineData(0, -1, 2)]
        [InlineData(-1, -1, 1)]
        [InlineData(3, 3, 1)]
        [InlineData(-10, -10, 0)]
        [InlineData(10, 10, 0)]
        public void Grid_GetNeighborsSquare(int x, int y, int count)
        {
            var grid = new Grid<Tile>(3, 3, (i, j) => new Tile(i, j));
            var tiles = grid.GetNeighborsSquare(x, y);
            Assert.Equal(count, tiles?.Count);
            Assert.True(tiles.TrueForAll(t => t.Distance(x, y) == 1 || Math.Round(t.Distance(x, y), 3) == Math.Round(Math.Sqrt(2), 3)));
        }

        [Fact]
        public void Grid_ForEachXY()
        {
            var grid = new Grid<Tile>(2, 2, (i, j) => new Tile(i, j));
            var xs = new List<float>();
            var ys = new List<float>();
            grid.ForEachXY(t => { xs.Add(t.X); ys.Add(t.Y); });
            Assert.True(xs.SequenceEqual(new List<float> { 0, 0, 1, 1 }));
            Assert.True(ys.SequenceEqual(new List<float> { 0, 1, 0, 1 }));
        }

        [Fact]
        public void Grid_ForEachYX()
        {
            var grid = new Grid<Tile>(2, 2, (i, j) => new Tile(i, j));
            var xs = new List<float>();
            var ys = new List<float>();
            grid.ForEachYX(t => { xs.Add(t.X); ys.Add(t.Y); });
            Assert.True(xs.SequenceEqual(new List<float> { 0, 1, 0, 1 }));
            Assert.True(ys.SequenceEqual(new List<float> { 0, 0, 1, 1 }));
        }

        [Fact]
        public void Grid_Constructor_2DList()
        {
            var grid = new Grid<Tile>(new List<List<Tile>>
            {
                new List<Tile> { null, null, null },
                new List<Tile> { null, null, null}
            });
            Assert.Equal(2, grid.Width);
            Assert.Equal(3, grid.Height);
        }
    }
}
