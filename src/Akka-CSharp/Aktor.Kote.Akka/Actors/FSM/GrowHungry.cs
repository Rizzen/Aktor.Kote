namespace Aktor.Kote.Akka.Actors.FSM
{
    public class GrowHungry : IData
    {
        public int Hunger { get; }

        public GrowHungry(int hunger)
        {
            Hunger = hunger;
        }
    }
}