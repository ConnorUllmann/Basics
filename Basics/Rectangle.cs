using System;
using System.Collections.Generic;
using System.Text;

namespace Basics
{
    public class Rectangle
    {
        public float X;
        public float Y;
        public float W;
        public float H;

        public Rectangle() { }
        public Rectangle(Rectangle _r) : this(_r.X, _r.Y, _r.W, _r.H) { }
        public Rectangle(float _x, float _y, float _w, float _h)
        {
            X = _x;
            Y = _y;
            W = _w;
            H = _h;
        }

        public float Area => W * H;

        public bool Collides(float px, float py) 
            => px >= X &&
               py >= Y &&
               px < X + W &&
               py < Y + H;
        public bool Collides(Rectangle b) => Collide(X, Y, W, H, b.X, b.Y, b.W, b.H);
        public bool Collides(float bx, float by, float bw, float bh) => Collide(X, Y, W, H, bx, by, bw, bh);
        public static bool Collide(float ax, float ay, float aw, float ah, float bx, float by, float bw, float bh)
            => ax + aw >= bx &&
               ay + ah >= by &&
               ax <= bx + bw &&
               ay <= by + bh;

        public bool Inside(Rectangle b) => Inside(X, Y, W, H, b.X, b.Y, b.W, b.H);
        public bool Inside(float bx, float by, float bw, float bh) => Inside(X, Y, W, H, bx, by, bw, bh);
        public static bool Inside(float ax, float ay, float aw, float ah, float bx, float by, float bw, float bh)
            => ax >= bx &&
               ay >= by &&
               ax + aw < bx + bw &&
               ay + ah < by + bh;
        
        public void SetDimensions(Rectangle _rectangle)
        {
            X = _rectangle.X;
            Y = _rectangle.Y;
            W = _rectangle.W;
            H = _rectangle.H;
        }
    }
}
