using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace MQ.RabbitMQManager
{
    /// <param name="messageBody"></param>
    /// <returns></returns>
    public delegate bool ListToQueueProcess(string messageBody);
    /// <summary>
    /// MQ基本服务类，用来连接到一个MQ，发消息或者监听并处理消息
    /// </summary>
    public  class MQBaseService
    { 
        private RabbitMQManager _rabbitMQManager;

        /// <summary>
        /// 代理，用来处理监听到的消息，并返回处理结果
        /// </summary>
        /// <summary>
        /// 用来接收处理方法
        /// </summary>
        ListToQueueProcess _process;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="process"></param>
        public MQBaseService(ListToQueueProcess process,RabbitMQManager rabbitMQManager)
        {
            _process = process;
            _rabbitMQManager=rabbitMQManager;
        }

        /// <summary>
        /// 监听队列
        /// </summary>
        /// <param name="queque"></param>
        /// <param name="bindRoutingKey"></param>
        public virtual void ListenToQueue(string queque,string bindRoutingKey)
        {
            _rabbitMQManager.Connect();
            _rabbitMQManager.InitQueue(queque,bindRoutingKey);
            _rabbitMQManager.BindConsumerEvent(BeigenProcess,queque);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="bindRoutingKey"></param>
        /// <param name="message"></param>
        public virtual void SendMessage(string queueName,string bindRoutingKey,string message)
        {
            _rabbitMQManager.BasicPushMessage(message,bindRoutingKey,queueName);
        }

        /// <summary>
        /// 处理队列消息
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="ea"></param>
        protected virtual bool BeigenProcess(object ch,BasicDeliverEventArgs ea )
        {
            byte [] body= ea.Body;
            string strBody=Encoding.UTF8.GetString(body);
            return _process(strBody);
        }
    }
}
