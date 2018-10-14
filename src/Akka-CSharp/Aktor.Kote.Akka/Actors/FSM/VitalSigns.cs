namespace Aktor.Kote.Akka.Actors.FSM
{
    public class VitalSigns : IData
    {
        public int Hunger { get; }

        public VitalSigns(int hunger)
        {
            Hunger = hunger;
        }
    }
}