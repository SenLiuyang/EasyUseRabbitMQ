using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MQ.RabbitMQManager;
using Utility;
namespace MQ
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitMQManager.RabbitMQManager rabbitMQManager=new RabbitMQManager.RabbitMQManager(Configuration.UserName,Configuration.Password,Configuration.VirtualHost,Configuration.HostName,Configuration.Port,Configuration.Exchange);
            ListToQueueProcess process=receiveProcess;
            MQBaseService mqbaseservice = new MQBaseService(process, rabbitMQManager);
            mqbaseservice.ListenToQueue(Configuration.mq_cps_payment_order_mq, "bindRouting");
            string message=string.Empty;
            do{
                  message= Console.ReadLine();
                  if (!string.IsNullOrEmpty(message))
                  mqbaseservice.SendMessage(Configuration.mq_cps_payment_order_mq, "bindRouting", message);
            }while(!string.IsNullOrEmpty(message));
            Thread.Sleep(1000000000);
        }

        /// <summary>
        /// 可以传入一个类的处理方法
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public  static bool receiveProcess(string body)
        {
            return true;
        }
    }
}
