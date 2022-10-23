namespace Micro.Common.Library.Entitys
{
    [Serializable]
    public class OrderInfoModel
    {
        /// <summary>
        /// 客人手机号
        /// </summary>
        public string GuestMobile { get; set; } = string.Empty;

        /// <summary>
        /// 客人姓名
        /// </summary>
        public string GuestName { get; set; } = string.Empty;

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 房间信息
        /// </summary>
        public List<RoomInfoModel> RoomInfo { get; set; } = new List<RoomInfoModel>();
    }
}