using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Storage
{
    public interface IJSONStore
    {
        T GetFileContent<T>(string filePath);
        bool SetFileContent<T>(T value, string filePath);
        bool DeleteFile(string filePath);
    }
}
