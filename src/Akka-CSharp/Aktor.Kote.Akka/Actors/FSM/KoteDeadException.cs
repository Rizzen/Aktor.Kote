using System;

namespace Aktor.Kote.Akka.Actors.FSM
{
    public class KoteDeadException : Exception
    {
        public string Name { get; }

        public KoteDeadException(string name)
        {
            Name = name;
        }
    }
}