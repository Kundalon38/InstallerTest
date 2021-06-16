namespace RISA_CustomActionsLib.Models.Linked
{
    public class SiError
    {
        public bool IsFatal { get; }
        public string Text { get; }

        public SiError(string text, bool isFatal = true)
        {
            IsFatal = isFatal;
            Text = text;
        }
    }
}
