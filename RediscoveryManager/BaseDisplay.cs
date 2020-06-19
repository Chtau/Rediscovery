using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RediscoveryManager
{
    public abstract class BaseDisplay
    {
        internal bool isWriting = false;

        internal void WaitForWriting()
        {
            do
            {
                Thread.Sleep(20);
            } while (isWriting);
        }
    }
}
