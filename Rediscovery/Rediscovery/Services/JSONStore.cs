using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.JSONStore))]
namespace Rediscovery.Services
{
    public class JSONStore : BaseService, IJSONStore
    {
        public T GetFileContent<T>(string filePath)
        {
            try
            {
                var content = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(content))
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
                return default;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default;
            }
        }

        public bool SetFileContent<T>(T value, string filePath)
        {
            try
            {
                var content = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                System.IO.File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
    }
}
