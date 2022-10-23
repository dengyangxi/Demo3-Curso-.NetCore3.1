using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client.Autogen.Grpc.v1;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Micro.Order.API.GrpcProtos;

namespace Micro.Order.API.OrderService
{
    public class OrderService : AppCallback.AppCallbackBase
    { 

        public override  Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();

            switch (request.Method)
            {
                //方法一  : 获取订单详情
                case "GetOrderByGrpc":

                    //1.1 ） 解压：请求信息
                    var req_getOrder = request.Data.Unpack<GetOrderRequest>();

                    //1.2）  处理：请求业务逻辑代码
                    var result_orderInfo = new GetOrderResponse
                    {

                        IsError = false,
                        Data = new OrderInfo
                        {
                            GuestMobile = "13888888888",
                            GuestName = "菜徐坤",
                            OrderID = req_getOrder.OrderID,

                        },
                        Msg = $"请求成功,用户需要获取订单{req_getOrder.OrderID}的信息"
                    };
                    result_orderInfo.Data.RoomInfo.AddRange(new List<RoomInfo>()
                    {
                        new RoomInfo (){ RmNo ="8001", RmType="豪华大床房" },
                        new RoomInfo (){ RmNo ="8011", RmType="豪华标间" },
                        new RoomInfo (){ RmNo ="8021", RmType="至尊套房" }
                    });
                     
                    //1.3）  压缩: 处理结果
                    response.Data = Any.Pack(result_orderInfo);

                    break;

                //方法二 ： 给某个房间添加在住人
                case "AddRoomGuest":
                    //1.1 ） 解压：请求信息
                    var req_addRoomGuest = request.Data.Unpack<GetOrderRequest>();
                    //1.2）  处理：请求业务逻辑代码
                    var result_addRoomGuest = new GetOrderResponse
                    {
                        IsError = false,
                        Data = new OrderInfo
                        {
                            GuestMobile = "1399999",
                            GuestName = "添加在住人",
                            OrderID = req_addRoomGuest.OrderID,

                        },
                        Msg = $"请求成功,订单{req_addRoomGuest.OrderID}的添加再住人"
                    };
                    //1.3）  压缩: 处理结果
                    response.Data = Any.Pack(result_addRoomGuest);
                    break;

                //方法三 ： ....

            }

            //返回处理结果
            return Task.FromResult(response); 

        }
    }
}
