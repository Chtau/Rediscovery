using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Deep copy of a object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            var sourceType = source.GetType();
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return (T)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source), sourceType, deserializeSettings);
        }
    }
}
