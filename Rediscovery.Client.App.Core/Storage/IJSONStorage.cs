using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Storage
{
    public interface IJSONStorage
    {
        T GetFileContent<T>(string filePath);
        bool SetFileContent<T>(T value, string filePath);
        bool DeleteFile(string filePath);
    }
}
