using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors.FSM;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote.Akka.Actors
{
    public class KoteActor : FSM<KoteState, IData>
    {
        private string _name;
        
        public KoteActor()
        {   
            StartWith(KoteState.Idle, Uninitialized.Instance);
            
            When(KoteState.Idle, state =>
            {
                switch (state.FsmEvent)
                {
                    case FallAsleep fallAsleep: return GoTo(KoteState.Sleeping);
                    case Uninitialized uninitialized: break;
                }
                
                return null;
            });
            
            When(KoteState.Sleeping, state =>
            {
                switch (state.FsmEvent)
                {
                    case WakeUp wakeUp :
                        Console.WriteLine($"Kote woke up from \"{wakeUp.With}\"");
                        return GoTo(KoteState.Idle);
                }

                return Stay();
            });
            
            WhenUnhandled(state =>
            {
                Console.WriteLine("Received unhandled " + state.FsmEvent);
                return Stay();
            });
            
            OnTransition((state, nextState) =>
            {
                switch (nextState)
                {
                    case KoteState.Sleeping: Console.WriteLine($"Kote {_name} is now sleeping");
                        break;
                }
            });
            Initialize();
        }

        private bool StatusHandle(KoteStateChangeMessage statusMessage)
        {
            Console.WriteLine($"Actor {Self.Path} : Kote {_name} received status {statusMessage.State}");
            return true;
        }

        private bool Born(KoteCreateMessage message)
        {
            _name = message.Name;
            Console.WriteLine($"{_name} just born\n{_name} : Meow!");

            return true;
        }

        private void DefaultHandle<T>(T message)
        {
            Console.WriteLine("Default Handle");
            throw new NotSupportedException();
        }
    }
}