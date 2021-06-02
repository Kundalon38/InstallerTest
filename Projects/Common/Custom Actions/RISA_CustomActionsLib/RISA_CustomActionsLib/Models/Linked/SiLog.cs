using System;
using System.IO;

namespace RISA_CustomActionsLib.Models.Linked
{
    public interface ISiLog
    {
        void Write(string who, string msg);
    }
    public class SiLog : ISiLog
    {
        public SiLog(string fn, bool append = true)
        {
            if (string.IsNullOrEmpty(fn)) return;
            try
            {
                _sw = new StreamWriter(fn, append);
            }
            catch (Exception)
            {
            }
        }

        public void Write(string who, string msg)
        {
            if (_sw == null) return;
            try
            {
                const string dtFmt = "dd-MMM-yy HH:mm:ss";
                _sw.WriteLine($"{DateTime.Now.ToString(dtFmt)} {who} - {msg}");
                _sw.Flush();    // file is never explicitly Close()d, often the best is written last
            }
            catch (Exception)
            {
            }
        }

        private readonly StreamWriter _sw;
    }
}
