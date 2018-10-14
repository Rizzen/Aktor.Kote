using System.Threading.Tasks;
using Akka.Actor;
using Aktor.Kote.Akka.Actors;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote.Akka
{
    public class KoteHome
    {
        private readonly ActorSystem system;
        private readonly IActorRef _coordinator;

        public KoteHome(int catCount)
        {
            system = ActorSystem.Create("KoteActorSystem");
            var prop = new Props(typeof(KoteCoordinatorActor), new object[] {catCount});
            _coordinator = system.ActorOf(prop, "KoteSupervisor");
        }

        public async Task Start()
        {
            _coordinator.Tell(new CoordinatorStartMessage());
            await system.WhenTerminated;
        }
    }
}