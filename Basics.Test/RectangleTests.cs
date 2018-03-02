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
    }
}
