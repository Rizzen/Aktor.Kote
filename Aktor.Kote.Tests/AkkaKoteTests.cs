using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aktor.Kote.Utils;
using Xunit;

namespace Aktor.Kote.Tests
{
    public class AkkaKoteTests
    {
        [Fact]
        public async void SequenceTest()
        {
            Func<long> func = Sequence.GetId;
            
            var tasks = new List<Task<long>>(1_000_000);
            
            for (int i = 0; i < 1_000_000; i++)
            {
                tasks.Add(new Task<long>(func));
            }
            
            foreach (var task in tasks)
            {
                task.Start();
            }

            var results = await Task.WhenAll(tasks);
            
            Assert.Equal(tasks.Count, results.Distinct().Count());
        }
    }
}