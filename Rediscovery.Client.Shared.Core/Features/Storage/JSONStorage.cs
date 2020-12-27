using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rediscovery.Client.Shared.Core.Features.Storage
{
    public class JSONStorage : IJSONStorage
    {
        private readonly ILogger _logger;
        private readonly ISettingValue<StorageSetting> _monitorSettings;

        public JSONStorage(ILogger logger, ISettingValue<StorageSetting> storageSettingValue)
        {
            _logger = logger;
            _monitorSettings = storageSettingValue;
        }

        public T GetFileContent<T>(string filePath)
        {
            return OnGetFileContent<T>(filePath, true).Item2;
        }

        private Tuple<bool, T> OnGetFileContent<T>(string filePath, bool repeat = false)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    var content = System.IO.File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(content))
                        return new Tuple<bool, T>(true, Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content));
                }
                return new Tuple<bool, T>(false, default);
            }
            catch (System.IO.IOException ioEX)
            {
                Tuple<bool, T> result = new Tuple<bool, T>(false, default);
                if (repeat)
                {
                    int count = 0;
                    do
                    {
                        Thread.Sleep(100);
                        result = OnGetFileContent<T>(filePath);
                        count++;
                    } while (count < 5 || !result.Item1);
                }
                if (!result.Item1)
                {
                    _logger.LogError(ioEX);
                    return new Tuple<bool, T>(false, default);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new Tuple<bool, T>(false, default);
            }
        }

        public bool SetFileContent<T>(T value, string filePath)
        {
            return OnSetFileContent(value, filePath, true);
        }

        private bool OnSetFileContent<T>(T value, string filePath, bool repeat = false)
        {
            try
            {
                var content = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                System.IO.File.WriteAllText(filePath, content);
                return true;
            }
            catch (System.IO.IOException ioEX)
            {
                bool result = false;
                if (repeat)
                {
                    int count = 0;
                    do
                    {
                        Thread.Sleep(100);
                        result = SetFileContent(value, filePath);
                        count++;
                    } while (count < 5 || !result);
                }
                if (!result)
                {
                    _logger.LogError(ioEX);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
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
                _logger.LogError(ex);
                return false;
            }
        }
    }
}
