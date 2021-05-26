namespace RISA_CustomActionsLib.Models.Linked
{
    public class CmdLineProperty
    {
        public string PropName { get; }
        public string PropValue { get; }

        public CmdLineProperty(string propName, string propValue)
        {
            PropName = propName.ToUpper();
            PropValue = propValue;
        }
    }
}
