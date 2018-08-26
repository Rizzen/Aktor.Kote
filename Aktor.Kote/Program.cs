using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote
{
    static class Program
    {
        public static void Main()
        {
            ActorWork();
            Console.WriteLine("Hello World!");
        }

        private static void ActorWork()
        {
            var actorSystem = ActorSystem.Create("KoteActorSystem");
            var koteCoordinatorActor = actorSystem.ActorOf<KoteCoordinatorActor>("KoteSupervisor");
            koteCoordinatorActor.Tell(new KoteCreateMessage{Name = "Vasya"});
            
            actorSystem.WhenTerminated.Wait();
        }
    }
}