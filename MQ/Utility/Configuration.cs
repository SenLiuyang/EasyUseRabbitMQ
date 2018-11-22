using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Utility
{
    /// <summary>
    /// 系统配置类
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// MQ配置 UserName 
        /// </summary>
        public static string UserName
        {
            get
            {
                return  ConfigurationManager.AppSettings["UserName"];
            }
        }
        /// <summary>
        /// MQ配置 Password 
        /// </summary>
        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["Password"];
            }
        }
        /// <summary>
        /// MQ配置 VirtualHost
        /// </summary>
        public static string VirtualHost
        {
            get
            {
                return ConfigurationManager.AppSettings["VirtualHost"];
            }
        }
        /// <summary>
        /// MQ 配置 HostName
        /// </summary>
        public static string HostName
        {
            get
            {
                return ConfigurationManager.AppSettings["HostName"];
            }
        }

        public static string Port
        {
            get
            {
                return ConfigurationManager.AppSettings["Port"];
            }
        } 
        public static string Exchange
        {
            get
            {
                return ConfigurationManager.AppSettings["Exchange"];
            }
        }


        public static string mq_cps_payment_order_mq
        {
            get
            {
                return ConfigurationManager.AppSettings["mq_cps_payment_order_mq"];
            }
        }


        public static string mq_csp_payment_order_reply
        {
            get
            {
                return ConfigurationManager.AppSettings["mq_csp_payment_order_reply"];
            }
        }
        public static string  mq_csp_payment_order_actual_reply
        {
            get
            {
                return ConfigurationManager.AppSettings["mq_csp_payment_order_actual_reply"];
            }
        }
    }
}
