using System;

namespace EasyMemoryCache.Memcached.Logging
{
    /// <summary>
    /// Creates loggers based on the current configuration.
    /// </summary>
    /// <example>
    ///
    /// Config file:
    ///
    /// <configuration>
    ///		<configSections>
    ///			<sectionGroup name="enyim.com">
    ///				<section name="log" type="Enyim.Caching.EnyimLoggerSection, Enyim.Caching" />
    ///			</sectionGroup>
    ///		</configSections>
    ///		<enyim.com>
    ///			<log factory="Enyim.Caching.Log4NetLoggerFactory, Enyim.Caching" />
    ///		</enyim.com>
    /// </configuration>
    ///
    /// Code:
    ///
    ///		LogManager.AssignFactory(new Log4NetLogFactory());
    ///
    /// </example>
    public static class LogManager
    {
        private static ILogFactory _factory;

        //TODO: Swith to Microsoft.Extensions.Logging
        static LogManager()
        {
#if DEBUG
            factory = new ConsoleLogFactory();
#else
            _factory = new NullLoggerFactory();
#endif
        }

        /// <summary>
        /// Assigns a new logger factory programmatically.
        /// </summary>
        /// <param name="factory"></param>
        public static void AssignFactory(ILogFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            {
                LogManager._factory = factory;
            }
        }

        /// <summary>
        /// Returns a new logger for the specified Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILog GetLogger(Type type)
        {
            return _factory.GetLogger(type);
        }

        /// <summary>
        /// Returns a logger with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog GetLogger(string name)
        {
            return _factory.GetLogger(name);
        }
    }
}