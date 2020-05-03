using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.JSONStore))]
namespace Rediscovery.Services
{
    public class JSONStore : BaseService, IJSONStore
    {
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
                    _logger.Error(ioEX);
                    return new Tuple<bool, T>(false, default);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                    _logger.Error(ioEX);
                    return false;
                }
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
