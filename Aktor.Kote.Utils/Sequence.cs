using System.Threading;

namespace Aktor.Kote.Utils
{
    public static class Sequence
    {
        private static long _currentId;

        public static long GetId()
        {
            var value = Interlocked.Increment(ref _currentId);
            return value;
        }
    }
}