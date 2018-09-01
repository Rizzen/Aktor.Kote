using System;
using System.Threading;
using Akka.Actor;
using Aktor.Kote.Akka.Actors.FSM;
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

            var kote = Context.ActorOf(prop, message.Name);
            
            kote.Tell(FallAsleep.Instance);
            Thread.Sleep(1500);
            kote.Tell(new WakeUp {With = "Hey, kotik, wake up!"});
            
            return true;
        }
    }
}