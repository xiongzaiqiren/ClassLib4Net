using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClassLib4Net.MsmqRount
{
    public class CommonConfig : CommonConfigBase
    {
        /// <summary>
        /// 
        /// </summary>
        public CommonConfig()
        {

        }

        public CommonConfig(string key)
        {
            this.Key = key;

        }

        [ConfigurationProperty("name", IsRequired = false, IsKey = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{};'\"|\\", MinLength = 0, MaxLength = 600)]
        public string Key
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
    }
    public class CommonConfigBase : ConfigurationElement
    {
        /// <summary>
        /// 
        /// </summary>
        public CommonConfigBase()
        {

        }
        /// <summary>
        /// 消息队列模块配置
        /// </summary>
        [ConfigurationProperty("MsmqConfig", IsRequired = false, IsKey = false)]
        public MsmqConfig MsmqConfig
        {
            get
            {
                return (MsmqConfig)this["MsmqConfig"];
            }
        }
    }

}