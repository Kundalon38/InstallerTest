using System;
using RISA_CustomActionsLib.Models.Linked;

namespace Silent_PreInstall_TestLib.Tests
{
    public class ConsoleLog : ISiLog
    {
        public void Write(string who, string msg)
        {
            const string dtFmt = "dd-MMM-yy HH:mm:ss";
            Console.WriteLine($"{DateTime.Now.ToString(dtFmt)} {who} - {msg}");
        }
    }
}
