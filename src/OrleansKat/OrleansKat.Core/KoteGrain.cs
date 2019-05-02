using System;
using System.Threading.Tasks;
using Orleans;
using OrleansKat.Domain;

namespace OrleansKat.Core
{
    public class KoteGrain : Grain, IKoteGrain
    {
        private KoteState _state;
        private string _key;
        
        public override Task OnActivateAsync()
        {
            Console.WriteLine($"Grain {_key} activate");
            _key = this.GetPrimaryKeyString();
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine($"Grain {_key} deactivate");
            return base.OnDeactivateAsync();
        }

        public Task Initialize()
        {
            _state = new KoteState()
            {
                Name = _key,
                Hunger = 0,
                State = KoteStateEnum.Walking
            };
            
            return Task.CompletedTask;
        }

        public Task ChangeState(KoteStateEnum newState)
        {
            _state.State = newState;
            return Task.CompletedTask;
        }

        public Task Exception() => throw new NotImplementedException();

        public Task<KoteState> GetState() => Task.FromResult(_state);

        public Task Meow()
        {
            Console.WriteLine($"Kote {_state.Name} says MEOW");
            return Task.CompletedTask;
        }
    }
}