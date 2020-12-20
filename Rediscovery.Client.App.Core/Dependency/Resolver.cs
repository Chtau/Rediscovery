using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rediscovery.Shared.Base.Extensions;

namespace Rediscovery.Client.App.Core.Dependency
{
    public static class Resolver
    {
        private static List<object> items = new List<object>();

        public static T Get<T>()
        {
            return (T)items?.FirstOrDefault(x => x is T);
        }

        public static void Register<T, I>() where I : class, T, new()
        {
            // TODO: this should be lazy
            items.Add(new I());
        }

        public static void Register<T, I>(I instance) where I : class, T
        {
            items.Add(instance);
        }

        public static void Register<T>(T instance) where T : class
        {
            items.Add(instance);
        }

        public static T Scope<T>()
        {
            var instance = Get<T>();
            return instance.Clone();
        }
    }
}
