using Aktor.Kote.Akka.Actors.FSM;

namespace Aktor.Kote.Akka.Actors.Messages
{
    public class KoteStateChangeMessage
    {
        public KoteState State { get; set; }
    }
}