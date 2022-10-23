namespace Micro.Common.Library.Entitys
{
    /// <summary>
    /// 消息通知  pub  sub
    /// </summary>
    [Serializable]
    public class NotificationMessage
    {
        /// <summary>
        /// 酒店编号
        /// </summary>
        public string HotelCd { get; set; } = string.Empty;

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; } = string.Empty;

        /// <summary>
        /// 消息详细内容
        /// </summary>
        public string DataJson { get; set; } = string.Empty;

        /// <summary>
        /// 消息发布时间
        /// </summary>
        public DateTime PubTime { get; set; } = DateTime.Now;

        //可以根据自定义 其他字段ing.....
    }

    /// <summary>
    /// 消息基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseMessage<T>
    {
        public string id { get; set; } = string.Empty;
        public string source { get; set; } = string.Empty;
        public string pubsubname { get; set; } = string.Empty;
        public string traceid { get; set; } = string.Empty;
        public string traceparent { get; set; } = string.Empty;
        public string tracestate { get; set; } = string.Empty;
        public string specversion { get; set; } = string.Empty;
        public string datacontenttype { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string topic { get; set; } = string.Empty;
        public T? data { get; set; }

        //   public string data { get; set; } = string.Empty;

        //private T? _dataModel { get; set; }
        //public T DataModel
        //{
        //    get
        //    {
        //        if (_dataModel == null)
        //        {
        //            _dataModel = data.ToModel<T>();
        //        }
        //        return _dataModel;
        //    }
        //}
    }
}