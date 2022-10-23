namespace Micro.Common.Library
{
    /// <summary>
    /// 微服务AppId 清单
    /// </summary>
    public class AppIdList
    {
        /// <summary>
        ///一、 订单微服务的AppId： orderapi  。
        ///     注： linux 区分大小写，因此 appid 的值 都采用小写,减少不必要的麻烦
        /// </summary>
        public static readonly string MicroOrderAPI = "orderapi";

        /// <summary>
        ///二、 订单微服务AppId： hotelapi  。
        ///     注： linux 区分大小写，因此 appid 的值 都采用小写,减少不必要的麻烦
        /// </summary>
        public static readonly string MicroHotelAPI = "hotelapi";

        //todo: 其他 微服务AppId ....
    }
}