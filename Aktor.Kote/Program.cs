using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors;
using Aktor.Kote.Akka.Actors.Messages;
using Aktor.Kote.Utils;

namespace Aktor.Kote
{
    static class Program
    {
        public static void Main()
        {
            //ActorWork();
            Console.WriteLine(new KoteNameGenerator("names.json").GetKoteName());
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