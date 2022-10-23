using Microsoft.AspNetCore.Mvc;

namespace Micro.Common.Library
{
    /// <summary>
    /// 微服务API 控制器 基类
    /// </summary>
    public class MicroBaseAPIController : ControllerBase
    {
        #region API返回值基层

        /// <summary>
        /// 定义一个成功返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [NonAction]
        public OkObjectResult Success<T>(T data, string msg = "成功")
        {
            return Ok(new BaseModel<T>()
            {
                Data = data,
                IsError = false,
                Msg = msg,
                Code = 200,
            });
        }

        /// <summary>
        /// 返回一个失败对象
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [NonAction]
        public OkObjectResult Failed<T>(string msg = "失败", int status = 500)
        {
            return Ok(new BaseModel<T>()
            {
                Data = default(T),
                IsError = true,
                Msg = msg,
                Code = status,
            });
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [NonAction]
        public OkObjectResult Result<T>(BaseModel<T> obj)
        {
            return Ok(obj);
        }

        #endregion API返回值基层

    }
}