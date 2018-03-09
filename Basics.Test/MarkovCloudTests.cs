using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace Basics.Test
{
    public class MarkovCloudTests
    {

        [Fact]
        public void MarkovCloud()
        {
            var cloud = new MarkovCloud();
            var state = string.Empty;
            cloud.AddLink(() => state = "B", 3);
            cloud.AddLink(() => state = "C", 2);
            cloud.AddLink(() => state = "D", 1);

            var states = new Dictionary<string, int>();
            for (var i = 0; i < 1000000; i++)
            {
                cloud.Update();

                if (!states.TryGetValue(state, out var count))
                    states.Add(state, 1);
                states[state]++;
            }

            var sum = states.Values.Sum();
            foreach(var kv in states)
            {
                System.Diagnostics.Debug.WriteLine($"{kv.Key}: {kv.Value} = {Math.Round(100f * kv.Value / sum)}%");
            }
        }
    }
}
