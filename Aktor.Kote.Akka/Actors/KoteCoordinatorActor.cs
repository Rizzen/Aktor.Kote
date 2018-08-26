using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote.Akka.Actors
{
    public class KoteCoordinatorActor : ReceiveActor
    {
        public KoteCoordinatorActor()
        {
            Receive<CoordinatorStartMessage>(Start);
            Receive<KoteCreateMessage>(CreateKote);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x =>
            {
                switch (x)
                {
                    case NotSupportedException _: return Directive.Restart;
                    default: return Directive.Stop;
                }
            });
        }

        private bool Start(CoordinatorStartMessage message)
        {
            return true;
        }

        private bool CreateKote(KoteCreateMessage message)
        {
            var prop = Props.Create<KoteActor>();
            
            Context.ActorOf(prop, message.Name).Tell(message);
            
            return true;
        }
    }
}