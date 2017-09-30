using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClassLib4Net.MsmqRount
{
    public class CommonPlatformConfiurationSectionHandler : ConfigurationSection
    {

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        [ConfigurationProperty("CommonConfig")]
        public CommonConfigBase CommonConfig
        {
            get { return (CommonConfigBase)base["CommonConfig"]; }
            set { base["CommonConfig"] = value; }
        }
        
        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        [ConfigurationProperty("CommonConfigs")]
        public CommonConfigCollection CommonConfigs
        {
            get { return (CommonConfigCollection)base["CommonConfigs"]; }
            set { base["CommonConfigs"] = value; }
        }
    }
}