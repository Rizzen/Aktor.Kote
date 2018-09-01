namespace Aktor.Kote.Akka.Actors.FSM
{
    public class Uninitialized : IData
    {
        public static Uninitialized Instance = new Uninitialized();
        
        private Uninitialized()
        {
        }
    }
}