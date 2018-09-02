using System;
using System.Threading;
using Akka.Actor;
using Akka.Event;
using Aktor.Kote.Akka.Actors.FSM;
using Aktor.Kote.Akka.Actors.Messages;
using Aktor.Kote.Utils;

namespace Aktor.Kote.Akka.Actors
{
    public class KoteCoordinatorActor : ReceiveActor
    {
        private readonly int _koteCount;
        
        private readonly ILoggingAdapter _log = Context.GetLogger();
        
        public KoteCoordinatorActor(int koteCount)
        {
            _koteCount = koteCount;
            Receive<CoordinatorStartMessage>(Start);
            Receive<KoteCreateMessage>(CreateKote);
        }

        private bool Start(CoordinatorStartMessage message)
        {
            return CreateKotes(_koteCount);
        }

        private bool CreateKotes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var prop = Props.Create<KoteActor>();

                var name = KoteNameGenerator.Default.GetKoteName();
                var kote = Context.ActorOf(prop, name);
                
                kote.Tell(new Initialize(name));
            }
            return true;
        }
        
        private bool CreateKote(KoteCreateMessage message)
        {
            var prop = Props.Create<KoteActor>();

            var kote = Context.ActorOf(prop, message.Name);
            
            kote.Tell(new Initialize(message.Name));
            return true;
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, 30_000, x =>
            {
                switch (x)
                {
                    case NotSupportedException _: return Directive.Restart;
                    case KoteDeadException kde:
                        _log.Error($"{kde.Name} is dead!!");
                        return Directive.Stop;
                    default: return Directive.Stop;
                }
            },false);
        }
    }
}