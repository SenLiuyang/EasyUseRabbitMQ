using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Utility;
namespace MQ.RabbitMQManager
{
    /// <summary>
    /// 作者：liuyang 2018-11-10
    /// RabbitMQ管理类，用来操作消息队列
    /// 功能：
    /// 1.可以连接到MQ的主机。
    /// 2.绑定Exchange和队列。
    /// 3.推送消息到队列
    /// 4.监听队列，使用委托处理消息
    /// 5.标记消费者
    /// 6.从队列中删除消费者
    /// 7.端口连接
    /// </summary>
    public  class RabbitMQManager
    {
        private  readonly string _username;
        private  readonly string _password;
        private  readonly string _virtualHost;
        private  readonly string _hostname;
        private  readonly string _port;
        private  readonly string _exhange;
        /// <summary>
        /// MQ的连接
        /// </summary>
        private  IConnection conn;
        /// <summary>
        /// MQ的model，用来绑定exchange和queue
        /// </summary>
        private  IModel model;
        /// <summary>
        /// 消费者实例，用来监控队列里面的消息
        /// </summary>
        private EventingBasicConsumer consumer;
        /// <summary>
        /// 消费者标识，可以从服务器上取消活动的consumer
        /// </summary>
        private string _consumerTag;
        /// <summary>
        /// 是否创建连接
        /// </summary>
        private bool _isConnectionCreated=false;
        /// <summary>
        /// 连接是否打开
        /// </summary>
        private bool _isConnectionOpen=false;
        /// <summary>
        /// 队列是否已经绑定
        /// </summary>
        private bool _isQueueBind=false;
        /// <summary>
        /// 消费者是否已经实例化
        /// </summary>
        private bool _isConsumerExist=false;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="virtualHost"></param>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="exchangeName"></param>
        public RabbitMQManager(string username,string password,string virtualHost,string hostName,string port,string exchangeName)
        {
            _username=username;
            _password=password;
            _virtualHost=virtualHost;
            _hostname=hostName;
            _port=port;
            _exhange = exchangeName;
        }
        /// <summary>
        /// 初始化到MQ的连接
        public void CreateConnection()
        {
            if(_isConnectionCreated) return;
            ConnectionFactory  factory=new ConnectionFactory();
            factory.UserName=_username;
            factory.Password=_password;
            factory.VirtualHost=_virtualHost;
            factory.HostName=_hostname;
            conn=factory.CreateConnection();
            _isConnectionCreated=true;
        }
        /// <summary>
        /// 初始化到MQ的连接
        /// </summary>
        public void CreateConnectionByUrl()
        {
            if (_isConnectionCreated) return;
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri=CreateUrl();
            _isConnectionCreated=true;
        }
        /// <summary>
        /// 连接到MQ
        /// </summary>
        public void Connect()
        {
            if(!_isConnectionCreated)
            {
                CreateConnection();
            }
            if(_isConnectionOpen)
            {
                return;
            }
            model=conn.CreateModel();
            _isConnectionOpen=true;
        }

        /// <summary>
        /// 断开到MQ的连接
        /// </summary>
        public void Disconnnect()
        {
            if(_isConnectionOpen)
            {
                model.Close();
                _isConnectionOpen=false;
            }
            if(_isConnectionCreated)
            {
                conn.Close();
                _isConnectionCreated=false;
            }
            _isQueueBind=false;
        }
        /// <summary>
        /// 初始化队列，并且将队列绑定
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="bindRoutingKey">绑定路由</param>
        public void InitQueue(string queueName,string bindRoutingKey)
        {
            if(!_isConnectionOpen)
            {
                Connect();
            }
            model.ExchangeDeclare(_exhange,ExchangeType.Direct,true);
            model.QueueDeclare(queueName,true,false,false,null);
            model.QueueBind(queueName,_exhange,bindRoutingKey,null);
            _isQueueBind=true;
        }
        /// <summary>
        /// 基本的消息推送方法
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="bindRouting">绑定的路由</param>
        /// <param name="queueName">队列名称</param>
        public void BasicPushMessage(string message,string bindRouting="",string queueName="")
        {
            if(!_isQueueBind)
            {
                InitQueue(queueName,bindRouting);
            }
            byte [] messageBodyBytes=System.Text.Encoding.UTF8.GetBytes(message);
            model.BasicPublish(_exhange, bindRouting, null, messageBodyBytes);
        }
        /// <summary>
        /// 发送消息，可设置请求属性
        /// </summary>
        /// <param name="message"></param>
        /// <param name="bindRouting"></param>
        /// <param name="contentType"></param>
        /// <param name="contentEncoding"></param>
        /// <param name="deliverMode"></param>
        /// <param name="expiration"></param>
        /// <param name="queueName"></param>
        public void PushMessage(string message, string bindRouting, string contentType, string contentEncoding, byte deliverMode, string expiration="", string queueName = "")
        {
           if(!_isQueueBind)
           {
               InitQueue(queueName, bindRouting);
           }
           IBasicProperties props=model.CreateBasicProperties();
           props.ContentType=contentType;
           props.ContentEncoding=contentEncoding;
           props.DeliveryMode=deliverMode;
           if(!string.IsNullOrEmpty(expiration))
           {
               props.Expiration=expiration;
           }
           BasicPushMessage(message,bindRouting,queueName);
        }
        /// <summary>
        /// 创建消费者
        /// </summary>
        public void CreateConsumer()
        {
             if(!_isConsumerExist)
             {
                 consumer = new EventingBasicConsumer(model);
                 _isConsumerExist=true;
             }
        }
        /// <summary>
        /// 创建标记，需要先绑定队列
        /// </summary>
        /// <param name="queueName"></param>
        public void CreateConsumerTag(string queueName)
        {
            _consumerTag=model.BasicConsume(queueName,false,consumer);
        }
        /// <summary>
        /// 取消消费者，需要先绑定队列
        /// </summary>
        public void CancelConsumer()
        {
            model.BasicCancel(_consumerTag);
        }
        /// <summary>
        /// 传入一个委托，用来处理消息，需要先绑定队列
        /// </summary>
        /// <param name="action"></param>
        public void BindConsumerEvent(Action<object, BasicDeliverEventArgs> action,string queueName)
        {
            CreateConsumer();
            consumer.Received+=(ch,ea)=>
            {
                action(ch, ea);
                model.BasicAck(ea.DeliveryTag,false);
            };
            model.BasicConsume(queueName,false,consumer);
        }

        /// <summary>
        /// 创建MQ的url
        /// </summary>
        /// <returns></returns>
        private string CreateUrl()
        {
            return string.Format("amqp://{0}:{1}@{2}:{3}/{4}",_username,_password,_hostname,_port,_virtualHost);
        }
    }
}
