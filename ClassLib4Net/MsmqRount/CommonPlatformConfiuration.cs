using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClassLib4Net.MsmqRount
{
    /// <summary>
    /// CommonPlatformConfiguration
    /// </summary>
    public class CommonPlatformConfiguration
    {
        private static CommonPlatformConfiurationSectionHandler instance;

        public static CommonPlatformConfiurationSectionHandler Instance
        {
            get
            {
                // Uses "Lazy initialization"
                if (instance == null)
                {
                    instance = (CommonPlatformConfiurationSectionHandler)ConfigurationManager.GetSection("ClassLib4Net.MSMQConfig");
                }
                return instance;
            }
        }
        /// <summary>
        /// ∂¡»°MsmqConfig≈‰÷√
        /// </summary>
        /// <returns></returns>
        public static MsmqConfig GetMsmqConfig()
        {
            return Instance.CommonConfig.MsmqConfig;
        }
        /// <summary>
        /// ∂¡»°MsmqConfig≈‰÷√
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static MsmqConfig GetMsmqConfig(string configName)
        {
            if (string.IsNullOrEmpty(configName))
            {
                return GetMsmqConfig();
            }
            return (Instance.CommonConfigs[configName] == null || Instance.CommonConfigs[configName].MsmqConfig == null) ?
                   GetMsmqConfig() :
                   Instance.CommonConfigs[configName].MsmqConfig;
        }
    }
}