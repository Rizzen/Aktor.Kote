using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote.Akka.Actors
{
    public class KoteCoordinatorActor : ReceiveActor
    {
        public KoteCoordinatorActor()
        {
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

        private bool CreateKote(KoteCreateMessage message)
        {
            var prop = Props.Create<KoteActor>(message.Name);
            
            Context.ActorOf(prop, message.Name);
            
            return true;
        }
    }
}