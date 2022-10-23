namespace Micro.Common.Library.Entitys
{
    /// <summary>
    /// 订单查询，请求类
    /// </summary>
    [Serializable]
    public class OrderInfoRequest
    {
        /// <summary>
        ///  订单号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 酒店编号
        /// </summary>
        public string HotelCd { get; set; } = string.Empty;
    }
}