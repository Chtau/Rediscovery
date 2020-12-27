using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Features.Storage
{
    public interface IJSONStorage
    {
        T GetFileContent<T>(string filePath);
        bool SetFileContent<T>(T value, string filePath);
        bool DeleteFile(string filePath);
    }
}
