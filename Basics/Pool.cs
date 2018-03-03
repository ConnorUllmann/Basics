using System;
using System.Collections.Concurrent;

namespace Basics
{
    public class Pool<T>
    {
        private Func<T> generator;
        private ConcurrentBag<T> bag = new ConcurrentBag<T>();
        public int? Capacity;

        public Pool(Func<T> _generator, int? _capacity = null)
        {
            generator = _generator ?? throw new ArgumentNullException("Cannot instantiate Pool instance without a generator method");
            Capacity = _capacity;
        }

        public T Get() => bag.TryTake(out var o) ? o : generator();

        public void Add(T _o)
        {
            if (!Capacity.HasValue || bag.Count < Capacity.Value)
                bag.Add(_o);
        }
    }
}
