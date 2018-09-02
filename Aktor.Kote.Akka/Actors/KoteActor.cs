using System;
using Akka;
using Akka.Actor;
using Akka.Event;
using Aktor.Kote.Akka.Actors.FSM;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote.Akka.Actors
{
    public class KoteActor : FSM<KoteState, IData>
    {
        private string _name; 
        private readonly ICancelable _hungerControl;
        private readonly ILoggingAdapter _log = Context.GetLogger();
        
        public KoteActor()
        {
            _hungerControl = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5), Self, new GrowHungry(10), Self);
            
            StartWith(KoteState.Idle, new VitalSigns(5));
            
            When(KoteState.Idle, state =>
            {
                switch (state.FsmEvent)
                {
                    case FallAsleep _:
                        return GoTo(KoteState.Sleeping);
                    case Initialize init : 
                        return Born(init.Name);
                }
                return null;
            });
            
            When(KoteState.Sleeping, state =>
            {
                switch (state.FsmEvent)
                {
                    case WakeUp wakeUp :
                        return GoTo(KoteState.Idle);
                }

                return null;
            });
            
            When(KoteState.Hunger, state => null );
            
            When(KoteState.Walking, state =>
            {
                SetTimer("walkingTimer", $"Kote {_name} walking", TimeSpan.FromSeconds(1), true);
                return null;
            });
            
            WhenUnhandled(DefaultHandle);
            
            OnTransition((state, nextState) =>
            {
                if (state == KoteState.Sleeping)
                    _log.Warning($"Kote {_name} awake");
                
                if (state == KoteState.Walking)
                    CancelTimer("walkingTimer");

                if (nextState == KoteState.Sleeping)
                    _log.Warning($"Kote {_name} is now sleeping");
            });
            
            Initialize();
        }
        
        private State<KoteState, IData> DefaultHandle(Event<IData> state)
        {
            switch (state.FsmEvent)
            {
                case GrowHungry gh when state.StateData is VitalSigns vs:
                    return Hunger(vs, gh.Hunger);
            }
            
            _log.Info($"Received {state.FsmEvent}");
            return Stay();
        }

        private State<KoteState, IData> Born(string name)
        {
            _name = name;
            _log.Info($"{_name} just born\n{_name} : Meow!");
            return Stay();
        }
        
        private State<KoteState, IData> Hunger(VitalSigns signs, int hunger)
        {
            var newHunger = signs.Hunger + hunger;
            
            _log.Warning($"{_name} is now hungry on {newHunger}");
            
            if (newHunger > 80 && newHunger < 100)
                return GoTo(KoteState.Hunger).Using(new VitalSigns(newHunger));
            
            if (newHunger >= 100)
                throw new KoteDeadException(_name);
            
            return Stay().Using(new VitalSigns(newHunger));
        }

        protected override void PostStop()
        {
            _hungerControl.Cancel();
            base.PostStop();
        }
    }
}