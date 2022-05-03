using System.Text.Json;

namespace Micro.Hotel.API.Common
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
    }
}
