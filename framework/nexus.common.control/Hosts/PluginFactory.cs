using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace nexus.common.control
{

    public static class PluginFactory
    {
        private static readonly Dictionary<string, Type> _pluginRegistry = new();

        static PluginFactory()
        {
        }

        public static void Register(string key, Type type)
        {
            if (!typeof(IMainUI).IsAssignableFrom(type))
                throw new ArgumentException("Type must implement IMainUI", nameof(type));

            _pluginRegistry[key] = type;
        }

        public static IMainUI? Create(string key)
        {
            if (_pluginRegistry.TryGetValue(key, out var type))
            {
                return Activator.CreateInstance(type) as IMainUI;
            }

            return null;
        }

        public static IEnumerable<string> ListAvailablePlugins() => _pluginRegistry.Keys;
    }
}
