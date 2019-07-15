using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Models
{
    public class LoadBinding : BaseModel
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
    }
}
