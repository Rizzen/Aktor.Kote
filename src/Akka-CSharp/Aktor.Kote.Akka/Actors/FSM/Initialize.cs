namespace Aktor.Kote.Akka.Actors.FSM
{
    public class Initialize : IData
    {
        public string Name { get; }

        public Initialize(string name)
        {
            Name = name;
        }
    }
}