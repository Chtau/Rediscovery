using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Resources
{
    public class AssemblyResourceProvider : IAssemblyResourceProvider
    {
        private readonly ILogger _logger;

        public AssemblyResourceProvider(ILogger logger)
        {
            _logger = logger;
        }

        public string Read(Assembly assembly, string name)
        {
            try
            {
                // Determine path
                string resourcePath = name;
                // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
                if (!name.StartsWith(nameof(Rediscovery)))
                {
                    resourcePath = assembly.GetManifestResourceNames()
                        .Single(str => str.EndsWith(name));
                }

                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return null;
        }
    }
}
