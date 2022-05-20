using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Micro.Common.Library.Extensions
{
    public static class JsonHelper
    {
        /// <summary>
        /// Json 字符串格式化
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ToJsonFormat(this object model)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(model, options);

            return json;
        }



        /// <summary>
        /// 将 Json字符串 转换成 Model 类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ToModel<T>(this string obj)
        {

#pragma warning disable CS8603 // 可能返回 null 引用。
            return JsonSerializer.Deserialize<T>(obj);
#pragma warning restore CS8603 // 可能返回 null 引用。

        }


        /// <summary>
        /// 转换成Json
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ToJson(this object model)
        {
            return JsonSerializer.Serialize(model);

        }
    }
}
