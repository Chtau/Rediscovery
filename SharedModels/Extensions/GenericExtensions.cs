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
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}
