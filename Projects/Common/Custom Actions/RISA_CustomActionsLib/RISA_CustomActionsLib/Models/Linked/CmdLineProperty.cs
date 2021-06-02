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
        public override string ToString()
        {
            var propNameStr = string.IsNullOrEmpty(PropName) ? "no-name" : PropName;
            var propValueStr = string.IsNullOrEmpty(PropValue) ? "missing-value" : PropValue.Dq();
            return $"{propNameStr}={propValueStr}";
        }
    }
}
