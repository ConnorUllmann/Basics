using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Basics
{
    public struct Rectangle : IPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W;
        public float H;
        
        public Rectangle(Rectangle _r) : this(_r.X, _r.Y, _r.W, _r.H) { }
        public Rectangle(float _x, float _y, float _w, float _h)
        {
            X = _x;
            Y = _y;
            W = _w;
            H = _h;
        }

        /// <summary>
        /// Area of the rectangle.
        /// </summary>
        public float Area => W * H;

        /// <summary>
        /// Checks if the given point is inside this rectangle.
        /// </summary>
        /// <param name="px">x-position of point</param>
        /// <param name="py">y-position of point</param>
        /// <returns>Whether the point is inside this rectangle</returns>
        public bool Collides(float px, float py) => Collide(px, py, X, Y, W, H);

        /// <summary>
        /// Checks if the given point is inside the given rectangle.
        /// </summary>
        /// <param name="px">x-position of point</param>
        /// <param name="py">y-position of point</param>
        /// <param name="rx">x-position of rectangle</param>
        /// <param name="ry">y-position of rectangle</param>
        /// <param name="rw">width of rectangle</param>
        /// <param name="rh">height of rectangle</param>
        /// <returns>Whether the point is inside the rectangle</returns>
        public static bool Collide(float px, float py, float rx, float ry, float rw, float rh)
            => px >= rx &&
               py >= ry &&
               px < rx + rw &&
               py < ry + rh;
        
        /// <summary>
        /// Checks if this rectangle collides with any of the given rectangles.
        /// </summary>
        /// <param name="_rectangles">rectangles to check collisions against</param>
        /// <returns>Whether this rectangle collides with any in the list</returns>
        public bool CollidesAny(IEnumerable<Rectangle> _rectangles) => _rectangles.Any(Collides);

        /// <summary>
        /// Checks if this rectangle collides with another rectangle.
        /// </summary>
        /// <param name="r">rectangle to check collisions against</param>
        /// <returns>Whether this rectangle collides with another</returns>
        public bool Collides(Rectangle r) => Collide(X, Y, W, H, r.X, r.Y, r.W, r.H);

        /// <summary>
        /// Checks if this rectangle collides with another rectangle.
        /// </summary>
        /// <param name="rx">x-position of rectangle</param>
        /// <param name="ry">y-position of rectangle</param>
        /// <param name="rw">width of rectangle</param>
        /// <param name="rh">height of rectangle</param>
        /// <returns>Whether this rectangle collides with another</returns>
        public bool Collides(float rx, float ry, float rw, float rh) => Collide(X, Y, W, H, rx, ry, rw, rh);

        /// <summary>
        /// Checks if one rectangle collides with another rectangle.
        /// </summary>
        /// <param name="ax">x-position of the first rectangle</param>
        /// <param name="ay">y-position of the first rectangle</param>
        /// <param name="aw">width of the first rectangle</param>
        /// <param name="ah">height of the first rectangle</param>
        /// <param name="bx">x-position of the second rectangle</param>
        /// <param name="by">y-position of the second rectangle</param>
        /// <param name="bw">width of the second rectangle</param>
        /// <param name="bh">height of the second rectangle</param>
        /// <returns>Whether one rectangle collides with another</returns>
        public static bool Collide(float ax, float ay, float aw, float ah, float bx, float by, float bw, float bh)
            => ax + aw > bx &&
               ay + ah > by &&
               ax < bx + bw &&
               ay < by + bh;

        /// <summary>
        /// Checks if one rectangle collides with another rectangle.
        /// </summary>
        /// <param name="a">the first rectangle</param>
        /// <param name="b">the second rectangle</param>
        /// <returns>Whether one rectangle collides with another</returns>
        public static bool Collide(Rectangle a, Rectangle b) => Collide(a.X, a.Y, a.W, a.H, b.X, b.Y, b.W, b.H);

        /// <summary>
        /// Whether this rectangle is completely inside the given rectangle.
        /// </summary>
        /// <param name="r">rectangle to see if this rectangle is inside of</param>
        /// <returns>Whether this rectangle is inside the given rectangle</returns>
        public bool Inside(Rectangle r) => Inside(X, Y, W, H, r.X, r.Y, r.W, r.H);

        /// <summary>
        /// Whether this rectangle is completely inside the given rectangle.
        /// </summary>
        /// <param name="rx">x-position of rectangle</param>
        /// <param name="ry">y-position of rectangle</param>
        /// <param name="rw">width of rectangle</param>
        /// <param name="rh">height of rectangle</param>
        /// <returns>Whether this rectangle is inside the given rectangle</returns>
        public bool Inside(float rx, float ry, float rw, float rh) => Inside(X, Y, W, H, rx, ry, rw, rh);

        /// <summary>
        /// Whether one rectangle is completely inside another rectangle.
        /// </summary>
        /// <param name="ax">x-position of the enveloped rectangle</param>
        /// <param name="ay">y-position of the enveloped rectangle</param>
        /// <param name="aw">width of the enveloped rectangle</param>
        /// <param name="ah">height of the enveloped rectangle</param>
        /// <param name="bx">x-position of the enveloping rectangle</param>
        /// <param name="by">y-position of the enveloping rectangle</param>
        /// <param name="bw">width of the enveloping rectangle</param>
        /// <param name="bh">height of the enveloping rectangle</param>
        /// <returns>Whether one rectangle is inside another</returns>
        public static bool Inside(float ax, float ay, float aw, float ah, float bx, float by, float bw, float bh)
            => ax >= bx &&
               ay >= by &&
               ax + aw < bx + bw &&
               ay + ah < by + bh;
        
        /// <summary>
        /// Sets this rectangle's position/dimensions to match the given rectangle.
        /// </summary>
        /// <param name="_rectangle">rectangle to match</param>
        public void MatchPositionAndDimensions(Rectangle _rectangle)
        {
            X = _rectangle.X;
            Y = _rectangle.Y;
            W = _rectangle.W;
            H = _rectangle.H;
        }

        public static Rectangle operator +(Rectangle _rectangle, IPosition _position)
            => new Rectangle(_rectangle.X + _position.X, _rectangle.Y + _position.Y, _rectangle.W, _rectangle.H);
        public static Rectangle operator +(Rectangle _rectangle, (float X, float Y) _position)
            => new Rectangle(_rectangle.X + _position.X, _rectangle.Y + _position.Y, _rectangle.W, _rectangle.H);

        public static Rectangle operator -(Rectangle _rectangle, IPosition _position)
            => new Rectangle(_rectangle.X - _position.X, _rectangle.Y - _position.Y, _rectangle.W, _rectangle.H);
        public static Rectangle operator -(Rectangle _rectangle, (float X, float Y) _position)
            => new Rectangle(_rectangle.X - _position.X, _rectangle.Y - _position.Y, _rectangle.W, _rectangle.H);

        public static Rectangle operator *(Rectangle _rectangle, float _scalar)
            => new Rectangle(_rectangle.X * _scalar, _rectangle.Y * _scalar, _rectangle.W * _scalar, _rectangle.H * _scalar);
        public static Rectangle operator *(Rectangle _rectangle, int _scalar)
            => new Rectangle(_rectangle.X * _scalar, _rectangle.Y * _scalar, _rectangle.W * _scalar, _rectangle.H * _scalar);

        public static Rectangle operator /(Rectangle _rectangle, float _scalar)
            => new Rectangle(_rectangle.X / _scalar, _rectangle.Y / _scalar, _rectangle.W / _scalar, _rectangle.H / _scalar);
        public static Rectangle operator /(Rectangle _rectangle, int _scalar)
            => new Rectangle(_rectangle.X / _scalar, _rectangle.Y / _scalar, _rectangle.W / _scalar, _rectangle.H / _scalar);

        public static bool operator ==(Rectangle _rectangle, Rectangle _other)
            => _rectangle.X == _other.X && _rectangle.Y == _other.Y && _rectangle.W == _other.W && _rectangle.H == _other.H;
        public static bool operator !=(Rectangle _rectangle, Rectangle _other)
            => !(_rectangle == _other);

        /// <summary>
        /// Generates set of coordinates corresponding to the rectangle's corners.
        /// </summary>
        /// <returns>(x,y) coordinates of corners of the rectangle in counter-clockwise order.</returns>
        public List<(float X, float Y)> ToVertices()
            => new List<(float X, float Y)>
            {
                (X, Y),
                (X + W, Y),
                (X + W, Y + H),
                (X, Y + H)
            };

        public override string ToString() => $"[({X}, {Y}) {W}x{H}]";
    }

    public static class RectangleExtensions
    {
        /// <summary>
        /// Checks whether any rectangle in one set of rectangles collides with any rectangle in another.
        /// </summary>
        /// <param name="_a">the first set of rectangles</param>
        /// <param name="_b">the second set of rectangles</param>
        /// <returns>Whether any rectangle in _a collides with any rectangle in _b</returns>
        public static bool CollidesAny(this IEnumerable<Rectangle> _a, IEnumerable<Rectangle> _b) => _a.Any(a => a.CollidesAny(_b));
    }
}
