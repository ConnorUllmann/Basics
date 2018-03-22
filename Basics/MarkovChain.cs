using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Basics
{
    public class MarkovChain
    {
        protected struct Link
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

            public Link? Sample()
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
        
        protected Link? current;

        protected MarkovChain(Link? _start=null) => current = _start;

        public void Update()
        {
            current = current?.Sample();
            current?.Execute();
        }

        private static void CheckValidity<T>(T startName, IDictionary<T, (Action action, IDictionary<T, float> weightByName)> linkInfoByName)
        {
            //startName must be a state in the dictionary
            var outerNames = new HashSet<T>(linkInfoByName.Keys);
            if (!outerNames.Contains(startName))
                throw new ArgumentException($"Cannot create MarkovChain from the given startName ({startName}) and linkInfoByName dictionary! There is no key in linkInfoByName which matches: {string.Join(", ", linkInfoByName.Keys.ToList())}");

            //Ensure that all states specified in all weight dictionaries exist in the outer dictionary
            var innerNames = new HashSet<T>(linkInfoByName.SelectMany(o => o.Value.weightByName.Select(i => i.Key)));
            if (!innerNames.IsSubsetOf(outerNames))
                throw new ArgumentException($"Cannot create MarkovChain from the given startName ({startName}) and linkInfoByName dictionary! There is a key in a linkInfoByName item's weightByName property which does not have a corresponding key in linkInfoByName");
        }

        public static MarkovChain FromDictionary<T>(T startName, IDictionary<T, (Action action, IDictionary<T, float> weightByName)> linkInfoByName)
        {
            CheckValidity(startName, linkInfoByName);

            var chain = new MarkovChain();

            //Create all links
            var linksByName = new Dictionary<T, Link>();
            foreach(var kv in linkInfoByName)
            {
                var name = kv.Key;
                var action = kv.Value.action;
                var link = new Link(action);
                linksByName.Add(name, link);
            }

            //Connect chain links
            foreach(var a in linkInfoByName)
            {
                var name = a.Key;
                var link = linksByName[name];
                var weightByName = a.Value.weightByName;
                foreach(var b in weightByName)
                    link.AddLink(linksByName[b.Key], b.Value);
            }
            
            //IsValid ensures startName is present
            chain.current = linksByName[startName];
            return chain;
        }
    }
}
