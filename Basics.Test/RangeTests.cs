using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Basics.Test
{
    public class RangeTests
    {
        [Theory]
        [InlineData(false, -11, -10)]
        [InlineData(false, 10, 11)]
        [InlineData(true, -3, 3)]
        [InlineData(true, -15, -5)]
        [InlineData(true, 5, 15)]
        [InlineData(true, -20, 20)]
        [InlineData(true, -10, -9)]
        [InlineData(true, 9, 10)]
        [InlineData(false, 0, 0)]
        public void Range_Collides(bool expected, float start, float end)
        {
            Assert.Equal(expected, new Ranges.Range(-10, 10).Collides(new Ranges.Range(start, end)));
        }
        
        
        [Theory]
        [InlineData(false, -7, -5)]
        [InlineData(true, -5, -3)]
        [InlineData(true, -12, -10)]
        [InlineData(true, -11, -9)]
        [InlineData(false, -10, -8)]
        [InlineData(true, -11, -4)]
        [InlineData(true, -6, -4)]
        [InlineData(true, -3, 0)]
        [InlineData(true, -6, 3)]
        [InlineData(false, -4, -5)]
        [InlineData(false, -6, -7)]
        [InlineData(true, -1000, 1000)]
        public void Ranges_Collides(bool expected, float start, float end)
        {
            var ranges = new Ranges();
            ranges.Add(-100, 90);
            ranges.Add(-5, 7);
            ranges.Add(5, 5);
            Assert.Equal(expected, ranges.Collides(start, end - start));
        }

        [Fact]
        public void Ranges_Collides_RightStraddle()
        {
            var ranges = new Ranges();
            ranges.Add(2, 3);
            ranges.Add(4, 6);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(2, ranges.ranges.First().start);
            Assert.Equal(10, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_RightOutside()
        {
            var ranges = new Ranges();
            ranges.Add(2, 3);
            ranges.Add(6, 4);
            Assert.Equal(2, ranges.ranges.Count);
        }

        [Fact]
        public void Ranges_Collides_LeftStraddle()
        {
            var ranges = new Ranges();
            ranges.Add(2, 3);
            ranges.Add(1, 2);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(1, ranges.ranges.First().start);
            Assert.Equal(5, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_LeftOutside()
        {
            var ranges = new Ranges();
            ranges.Add(2, 3);
            ranges.Add(-1, 1);
            Assert.Equal(2, ranges.ranges.Count);
        }

        [Fact]
        public void Ranges_Collides_Envelope()
        {
            var ranges = new Ranges();
            ranges.Add(-5, 10);
            ranges.Add(0, 1);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(-5, ranges.ranges.First().start);
            Assert.Equal(5, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_Enveloped()
        {
            var ranges = new Ranges();
            ranges.Add(0, 1);
            ranges.Add(-5, 10);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(-5, ranges.ranges.First().start);
            Assert.Equal(5, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_StraddleTwo()
        {
            var ranges = new Ranges();
            ranges.Add(0, 2);
            ranges.Add(5, 2);
            ranges.Add(1, 5);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(0, ranges.ranges.First().start);
            Assert.Equal(7, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_StraddleMany()
        {
            var ranges = new Ranges();
            ranges.Add(3, 2);
            ranges.Add(6, 2);
            ranges.Add(9, 2);
            ranges.Add(12, 2);
            ranges.Add(4, 9);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(3, ranges.ranges.First().start);
            Assert.Equal(14, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_FillGap()
        {
            var ranges = new Ranges();
            ranges.Add(0, 2);
            ranges.Add(5, 2);
            ranges.Add(2, 3);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(0, ranges.ranges.First().start);
            Assert.Equal(7, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_LeftTangent()
        {
            var ranges = new Ranges();
            ranges.Add(5, 2);
            ranges.Add(2, 3);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(2, ranges.ranges.First().start);
            Assert.Equal(7, ranges.ranges.First().end);
        }

        [Fact]
        public void Ranges_Collides_RightTangent()
        {
            var ranges = new Ranges();
            ranges.Add(2, 3);
            ranges.Add(5, 2);
            Assert.Equal(1, ranges.ranges.Count);
            Assert.Equal(2, ranges.ranges.First().start);
            Assert.Equal(7, ranges.ranges.First().end);
        }
    }
}
