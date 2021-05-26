using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RISA_CustomActionsLib.Test
{
    public class TestBase
    {
        protected void expecting(bool cond)
        {
            if (cond) return;
            Assert.IsTrue(cond); // will fail
        }
        protected void expecting(bool cond, int step)
        {
            if (cond) return;
            Assert.IsTrue(cond, $"fails at step {step} ");
        }
    }
}
