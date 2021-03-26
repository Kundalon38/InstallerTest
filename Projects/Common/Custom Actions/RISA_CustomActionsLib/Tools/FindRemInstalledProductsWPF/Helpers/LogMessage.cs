using System;

namespace FindRemInstalledProductsWPF.Helpers
{
    public enum eLogMessageType { Debug, Error, Normal, Warn }

    public class LogMessage
    {
        public DateTime MessageDT { get; set; }
        public eLogMessageType MessageType { get; set; }
        public string MessageStr { get; set; }

        public LogMessage(string msg) : this(eLogMessageType.Normal, DateTime.Now, msg)
        {
        }

        public LogMessage(eLogMessageType mt, string msg) : this(mt, DateTime.Now, msg)
        {
        }
        public LogMessage(eLogMessageType mt, DateTime msgDT, string msg)
        {
            MessageType = mt;
            MessageStr = msg;
            MessageDT = msgDT;
        }


        public static eLogMessageType LogMessageType_Debug => eLogMessageType.Debug;
        public static eLogMessageType LogMessageType_Error => eLogMessageType.Error;
        public static eLogMessageType LogMessageType_Normal => eLogMessageType.Normal;
        public static eLogMessageType LogMessageType_Warn => eLogMessageType.Warn;


        public override string ToString()
        {
            const string dtFmt = "dd-MMM-yy HH:mm:ss";
            return $"{MessageDT.ToString(dtFmt)} %{MessageType.ToString().Substring(0, 1)} {MessageStr}";
        }
    }
}
