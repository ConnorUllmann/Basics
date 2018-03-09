using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Basics
{
    public class MarkovChain
    {
        protected class Link
        {
            private List<(Link link, float weight)> linkWeights;

            private Action action;

            public Link(Action _action=null)
            {
                action = _action;
                linkWeights = new List<(Link, float)>();
            }

            public void AddLink(Link _link, float _weight) 
                => linkWeights.Add((_link, _weight));

            public void Execute() => action();

            public Link Sample()
            {
                if (linkWeights.Count <= 0)
                    return null;

                var max = linkWeights.Sum(o => o.weight);
                var selection = Utils.RandomDouble() * max;
                var partialSum = 0f;
                foreach(var o in linkWeights)
                {
                    if (partialSum + o.weight >= selection)
                        return o.link;
                    partialSum += o.weight;
                }
                throw new Exception($"Failed to sample for the next markov link. partialSum: {partialSum}, selection: {selection}, max: {max}");
            }
        }
        
        protected Link current;

        protected MarkovChain() => current = null;

        protected void SetStart(Link _start) => current = _start;

        public void Update()
        {
            current = current?.Sample();
            current.Execute();
        }
    }

    /// <summary>
    /// Markov chain in which each link has the same weight in its relationship with each other link
    /// </summary>
    public class MarkovCloud : MarkovChain
    {
        private Dictionary<Link, float> weightsByLink;

        public MarkovCloud() : base()
        {
            weightsByLink = new Dictionary<Link, float>();
        }

        public void AddLink(Action _action, float _weight)
        {
            var link = new Link(_action);
            if (weightsByLink.Count == 0)
                SetStart(link);
            weightsByLink[link] = _weight; //Each link links to itself since this comes before the loop
            foreach (var kv in weightsByLink)
            {
                var otherLink = kv.Key;
                var otherWeight = kv.Value;
                otherLink.AddLink(link, _weight);
                link.AddLink(otherLink, otherWeight);
            }
        }
    }
}
