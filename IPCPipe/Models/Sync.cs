using System;
using System.Collections.Generic;
using System.Text;

namespace IPCPipe.Models
{
    public enum SyncAction
    {
        None,
        Add,
        Delete,
        Update
    }

    public class Sync<T>
    {
        public SyncAction ActionType { get; set; }

        public T Entity { get; set; }
    }
}
