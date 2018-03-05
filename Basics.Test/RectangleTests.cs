using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Basics.Test
{
    public class RectangleTests
    {
        [Theory]
        [InlineData(-3, 0, -4, -4, 8, 8, true)]
        [InlineData(0, -3, -4, -4, 8, 8, true)]
        [InlineData(3, 0, -4, -4, 8, 8, true)]
        [InlineData(0, 3, -4, -4, 8, 8, true)]
        [InlineData(-5, 0, -4, -4, 8, 8, false)]
        [InlineData(0, -5, -4, -4, 8, 8, false)]
        [InlineData(5, 0, -4, -4, 8, 8, false)]
        [InlineData(0, 5, -4, -4, 8, 8, false)]
        [InlineData(-1, 5, -1, -3, 1, 10, true)]
        [InlineData(5, -1, -3, -1, 10, 1, true)]
        [InlineData(0, 0, 0, 0, 0, 0, false)]
        [InlineData(0, 0, -1, -1, 2, 2, true)]
        [InlineData(-5, -5, -5, -5, 5, 5, true)]
        [InlineData(0, 0, -5, -5, 5, 5, false)]
        [InlineData(3, 3, 0, 0, 5, 5, true)]
        [InlineData(0, 0, 0, 0, 5, 5, true)]
        [InlineData(5, 5, 0, 0, 5, 5, false)]
        public void RectangleCollide_ReturnSucceed(float _x, float _y, float _rx, float _ry, float _rw, float _rh, bool _inside = true)
            => Assert.Equal(_inside, Rectangle.Collide(_x, _y, _rx, _ry, _rw, _rh));

        [Theory]
        [InlineData(true, -40, 10, 30, 30)]
        [InlineData(true, 110, -90, 30, 30)]
        [InlineData(true, 35, -40, 30, 30)]
        [InlineData(true, 35, -90, 30, 30)]
        [InlineData(true, 110, -165, 30, 30)]
        [InlineData(false, 10, 10, 30, 30)]
        [InlineData(false, 210, -90, 30, 80)]
        public void RectangleCollidesAny_ReturnSucceed(bool _expected, float _x, float _y, float _w, float _h)
        {
            var rectangles = new List<Rectangle>
            {
                new Rectangle(50, -100, 100, 100),
                new Rectangle(100, -150, 100, 100),
                new Rectangle(-50, 0, 50, 50)
            };
            Assert.Equal(_expected, new Rectangle(_x, _y, _w, _h).CollidesAny(rectangles));
        }

        [Fact]
        public void RectanglesCollidesAny_ReturnSucceed_Same()
        {
            var rectangles = new List<Rectangle>
            {
                new Rectangle(50, -100, 100, 100),
                new Rectangle(100, -150, 100, 100),
                new Rectangle(-50, 0, 50, 50)
            };
            Assert.Equal(true, rectangles.CollidesAny(rectangles));
        }

        [Fact]
        public void RectanglesCollidesAny_ReturnSucceed_SingleCollisionOfMultiple()
        {
            var a = new List<Rectangle>
            {
                new Rectangle(50, -100, 100, 100),
                new Rectangle(100, -150, 100, 100),
                new Rectangle(-50, 0, 50, 50)
            };
            var b = new List<Rectangle>
            {
                new Rectangle(10, 10, 30, 30),
                new Rectangle(210, -140, 30, 80),
                new Rectangle(110, -165, 30, 30)
            };
            Assert.Equal(true, a.CollidesAny(b));
            Assert.Equal(true, b.CollidesAny(a));
        }

        [Fact]
        public void RectanglesCollidesAny_ReturnSucceed_NoCollisionsOfMultiple()
        {
            var a = new List<Rectangle>
            {
                new Rectangle(50, -100, 100, 100),
                new Rectangle(100, -150, 100, 100),
                new Rectangle(-50, 0, 50, 50)
            };
            var b = new List<Rectangle>
            {
                new Rectangle(10, 10, 30, 30),
                new Rectangle(210, -140, 30, 80),
                new Rectangle(110, -215, 30, 30)
            };
            Assert.Equal(false, a.CollidesAny(b));
            Assert.Equal(false, b.CollidesAny(a));
        }
    }
}
