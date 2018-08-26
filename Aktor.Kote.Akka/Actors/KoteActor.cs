using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote.Akka.Actors
{
    public class KoteActor : ReceiveActor
    {
        private string _name;
        
        public KoteActor()
        {   
            Receive<KoteStatusMessage>(StatusHandle);
            Receive<KoteCreateMessage>(Born);
            ReceiveAny(DefaultHandle);
        }

        private bool StatusHandle(KoteStatusMessage statusMessage)
        {
            Console.WriteLine($"Actor {Self.Path} : Kote {_name} received status {statusMessage.Status}");
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