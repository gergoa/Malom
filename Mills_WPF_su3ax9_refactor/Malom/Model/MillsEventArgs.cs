namespace Malom.Model
{
    public class MillsEventArgs
    {
        public string NextAction { get; set; }
        public MillsEventArgs(string nextAction)
        {
            NextAction = nextAction;
        }
    }
}
