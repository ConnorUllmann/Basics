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

        protected MarkovChain(Link _start=null) => current = _start;

        public void Update()
        {
            current = current?.Sample();
            current.Execute();
        }

        private static bool IsValid(string startName, IDictionary<string, (Action action, IDictionary<string, float> weightByName)> linkInfoByName)
        {
            //startName must be a state in the dictionary
            var outerNames = new HashSet<string>(linkInfoByName.Keys);
            if (!outerNames.Contains(startName))
                return false;

            //Ensure that all states specified in all weight dictionaries exist in the outer dictionary
            var innerNames = new HashSet<string>(linkInfoByName.SelectMany(o => o.Value.weightByName.Select(i => i.Key)));
            if (!innerNames.IsSubsetOf(outerNames))
                return false;

            return true;
        }

        public static MarkovChain FromDictionary(string startName, IDictionary<string, (Action action, IDictionary<string, float> weightByName)> linkInfoByName)
        {
            if (!IsValid(startName, linkInfoByName))
                return null;

            var chain = new MarkovChain();

            //Create all links
            var linksByName = new Dictionary<string, Link>();
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
