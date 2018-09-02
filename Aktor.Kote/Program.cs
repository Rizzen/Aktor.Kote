using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors;
using Aktor.Kote.Akka.Actors.FSM;
using Aktor.Kote.Akka.Actors.Messages;
using Aktor.Kote.Utils;

namespace Aktor.Kote
{
    static class Program
    {
        public static void Main()
        {
            ActorWork();
        }

        private static void ActorWork()
        {
            var actorSystem = ActorSystem.Create("KoteActorSystem");
            var prop = new Props(typeof(KoteCoordinatorActor),new object[]{10});
            var koteCoordinatorActor = actorSystem.ActorOf(prop, "KoteSupervisor");
            koteCoordinatorActor.Tell(new CoordinatorStartMessage());
            actorSystem.WhenTerminated.Wait();
        }
    }
}