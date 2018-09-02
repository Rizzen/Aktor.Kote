using System.Threading.Tasks;
using Aktor.Kote.Akka;

namespace Aktor.Kote
{
    static class Program
    {
        public static async Task Main()
        {
            await (new KoteHome(10)).Start();
        }
    }
}