using System;
using System.Collections.Generic;

namespace RISA_CustomActionsLib
{
    // mirrors the Session object passed in from MSI,
    // used for testing Custom Actions

    public class SessionDTO
    {
        public SessionDTO(Action<string> dlgtLog)
        {
            // pass in Session.Log(msg) function, or any other of your choosing
            _dlgtLog = dlgtLog;
        }

        private readonly Action<string> _dlgtLog;
        private readonly Dictionary<string,string> _propDict = new Dictionary<string, string>();

        public void Log(string msg)
        {
            _dlgtLog(msg);
        }

        public string this[string propName]
        {
            get
            {
                if (_propDict.TryGetValue(propName, out var propValue)) return propValue;
                throw new IndexOutOfRangeException($"Property {propName} is not defined");
            }
            set => _propDict[propName] = value;
        }

        public override string ToString()
        {
            var outStr = string.Empty;
            foreach (var kvp in _propDict)
                outStr += $"{kvp.Key} = {kvp.Value}{Environment.NewLine}";
            return outStr;
        }
    }
}
