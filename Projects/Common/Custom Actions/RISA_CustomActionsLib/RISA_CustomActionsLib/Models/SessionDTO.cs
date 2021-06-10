using System;
using System.Collections.Generic;

namespace RISA_CustomActionsLib.Models
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

        public static SessionDTO Clone(SessionDTO sess)
        {
            var clone = new SessionDTO(sess._dlgtLog);
            foreach(var kvp in sess.PropDict) clone.PropDict.Add(kvp.Key,kvp.Value);
            return clone;
        }

        private readonly Action<string> _dlgtLog;
        public Dictionary<string,string> PropDict { get; } = new Dictionary<string, string>();

        public void Log(string msg)
        {
            _dlgtLog(msg);
        }

        public string this[string propName]
        {
            get
            {
                if (PropDict.TryGetValue(propName, out var propValue)) return propValue;
                throw new IndexOutOfRangeException($"Property {propName} is not defined");
            }
            set => PropDict[propName] = value;
        }

        public bool PropertyExists(string propName)
        {
            return PropDict.ContainsKey(propName);
        }

        public override string ToString()
        {
            var outStr = string.Empty;
            foreach (var kvp in PropDict)
                outStr += $"{kvp.Key} = {kvp.Value}{Environment.NewLine}";
            return outStr;
        }
    }
}
