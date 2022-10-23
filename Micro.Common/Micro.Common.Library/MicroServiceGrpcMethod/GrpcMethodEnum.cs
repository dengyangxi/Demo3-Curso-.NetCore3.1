using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Common.Library
{

    /// <summary>
    /// 微服务 枚举方法
    /// </summary>
    public class GrpcMethodEnum
    {

        #region  一、Order 微服务 Grpc方法清单 

        /// <summary>
        /// Order 微服务 方法清单 
        ///  
        /// </summary>
        public enum OrderAPI
        {

            /// <summary>
            ///  获取订单详情信息
            /// </summary>
            GetOrderByGrpc,

            /// <summary>
            ///  添加入住人信息
            /// </summary>
            AddRoomGuest,
        }

        #endregion


        #region  二、Hotel 微服务 Grpc方法清单 

        /// <summary>
        /// Hotel 微服务 方法清单 
        ///  
        /// </summary>
        public enum HotelAPI
        {

            //todu ： Hotel 微服务提供的Grpc方法

        }

        #endregion 

    }
}
