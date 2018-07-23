using System;
using System.Collections.Generic;
using System.Linq;

namespace Basics
{
    public class Ranges
    {
        internal struct Range
        {
            private readonly bool valid;
            public readonly float start;
            public readonly float end;

            public Range(float _start, float _end)
            {
                if (_start > _end)
                {
                    var temp = _start;
                    _start = _end;
                    _end = temp;
                }

                start = _start;
                end = _end;
                valid = start != end;
            }

            public bool Collides(Range range)
            {
                if (!valid || !range.valid)
                    return false;

                if (start < range.start)
                {
                    if (end > range.start)
                        return true;
                }
                else
                {
                    if (start < range.end)
                        return true;
                }
                return false;
            }
        }

        internal List<Range> ranges = new List<Range>();

        public Ranges() { }

        private void handleCollisions()
        {
            ranges = ranges.OrderBy(o => o.start).ToList();
            for(var i = 0; i < ranges.Count-1; i++)
            {
                var range = ranges[i];
                var nextRange = ranges[i + 1];

                if (range.Collides(nextRange) || range.end == nextRange.start)
                {
                    ranges[i] = new Range(Math.Min(range.start, nextRange.start), Math.Max(range.end, nextRange.end));
                    ranges.RemoveAt(i + 1);
                    i--;
                    continue;
                }
            }
        }

        public void Add(float start, float length)
        {
            if (length <= 0)
                return;
            ranges.Add(new Range(start, start + length));
            handleCollisions();
        }

        public bool Collides(float start, float length)
        {
            if (length <= 0)
                return false;
            return ranges.Any(o => o.Collides(new Range(start, start + length)));
        }
    }
}
