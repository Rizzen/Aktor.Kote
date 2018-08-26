using System;
using Akka.Actor;
using Aktor.Kote.Akka.Actors.Messages;

namespace Aktor.Kote.Akka.Actors
{
    public class KoteActor : ReceiveActor
    {
        private readonly string _name;
        
        public KoteActor(string name)
        {
            _name = name;
            
            Console.WriteLine($"{name} just born");
            
            Receive<KoteStatusMessage>(StatusHandle);
            Receive<KoteInfoMessage>(GreetNewKote);
            ReceiveAny(DefaultHandle);
        }

        private bool StatusHandle(KoteStatusMessage statusMessage)
        {
            Console.WriteLine($"Actor {Self.Path} : Kote {_name} received status {statusMessage.Status}");
            return true;
        }

        private bool GreetNewKote(KoteInfoMessage message)
        {
            Console.WriteLine($"Hello, Kote {message}");
            return true;
        }

        private void DefaultHandle<T>(T message)
        {
            Console.WriteLine("Default Handle");
            throw new NotSupportedException();
        }
    }
}