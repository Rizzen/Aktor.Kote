namespace Aktor.Kote.Akka.Actors.FSM
{
    public class FallAsleep : IData
    {
        public static readonly FallAsleep Instance = new FallAsleep();

        private FallAsleep()
        {
        }
    }
}