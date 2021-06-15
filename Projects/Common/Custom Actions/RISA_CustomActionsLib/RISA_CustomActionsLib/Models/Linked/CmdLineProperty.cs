namespace RISA_CustomActionsLib.Models.Linked
{
    public class CmdLineProperty
    {
        public string PropName { get; }     // the prop names that matter, from cmd line
        public string PropValue { get; set; }
        public string IniPropName { get; }  // used in diagnostic msgs to user, if read from ini file

        public CmdLineProperty(string propName, string propValue)
        {
            PropName = propName.ToUpper();
            PropValue = propValue;
        }
        public CmdLineProperty(string propName, string iniPropName, string propValue)
        {
            PropName = propName.ToUpper();
            IniPropName = iniPropName;
            PropValue = propValue;
        }
        public override string ToString()
        {
            string propNameStr;
            if(IniPropName == null) propNameStr = string.IsNullOrEmpty(PropName) ? "no-name" : PropName;
            else propNameStr = IniPropName;

            var propValueStr = string.IsNullOrEmpty(PropValue) ? "missing-value" : PropValue.Dq();
            return $"{propNameStr}={propValueStr}";
        }
    }
}
