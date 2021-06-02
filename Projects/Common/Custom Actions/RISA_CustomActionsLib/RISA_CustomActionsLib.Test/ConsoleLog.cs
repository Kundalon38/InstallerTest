using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RISA_CustomActionsLib.Models.Linked;

namespace RISA_CustomActionsLib.Test
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
