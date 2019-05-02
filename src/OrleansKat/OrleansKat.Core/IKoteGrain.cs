using System.Threading.Tasks;
using Orleans;
using OrleansKat.Domain;

namespace OrleansKat.Core
{
    public interface IKoteGrain : IGrainWithStringKey
    {
        Task Meow();

        Task Initialize();

        Task ChangeState(KoteStateEnum newState);
        
        Task Exception();

        Task<KoteState> GetState();
    }
}