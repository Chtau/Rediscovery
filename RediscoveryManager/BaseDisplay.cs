using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rediscovery.Client.App.Manager.Console
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

        public virtual void Handle() { }
        internal virtual void DisplayTitle() { }

        internal virtual bool ResetOrBack(string input)
        {
            if (Commands.MatchInput(input, Commands.Back))
            {
                return false;
            }
            else
            {
                DisplayTitle();
                return true;
            }
        }
    }
}
