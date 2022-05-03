using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Micro.Hotel.API.Common;

namespace Micro.Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpController : ControllerBase
    {
        public ActionResult Get()
        {
            var ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces().Select(p => p.GetIPProperties()).SelectMany(p => p.UnicastAddresses)
                      .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                      .FirstOrDefault()?.Address.ToString();

            return Ok(new List<string> { ip ?? "IP获取失败" });
        }

        [HttpGet]
        [Route("Hello")]
        public ActionResult Hello()
        {
            var model = ServerInformation();

            //  var json = Newtonsoft.Json.Linq.JValue.Parse(JsonConvert.SerializeObject(model)).ToString(Newtonsoft.Json.Formatting.Indented);

            //var options = new JsonSerializerOptions
            //{
            //    WriteIndented = true,
            //    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            //};
            //var json = JsonSerializer.Serialize(model, options);

            return Ok(model.ToJsonFormat());
        }
        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        private static ServerInformationModel ServerInformation()
        {
            Process[] p = Process.GetProcesses();//获取进程信息
            var model = new ServerInformationModel();
            Int64 totalMem = 0;
            //string info = "";
            foreach (Process pr in p)
            {
                totalMem += pr.WorkingSet64 / 1024;
                //   info += pr.ProcessName + "内存：-----------" + (pr.WorkingSet64 / 1024).ToString() + "KB\r\n";//得到进程内存
            }
            model.服务器时间 = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
            //   list.Add(info);
            model.占用总内存 = (totalMem / 1024) + "M";
            //   list.Add("判断是否为Windows Linux OSX");
            model.是否是Linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux).ToString();
            model.是否是OSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX).ToString();
            model.是否是Windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows).ToString();
            model.系统架构 = RuntimeInformation.OSArchitecture.ToString();
            model.系统名称 = RuntimeInformation.OSDescription.ToString();
            model.进程架构 = RuntimeInformation.ProcessArchitecture.ToString();
            model.是否64位操作系统 = Environment.Is64BitOperatingSystem.ToString();
            model.CPU_CORE = Environment.ProcessorCount.ToString();
            model.HostName = Environment.MachineName.ToString();
            model.Version = Environment.OSVersion.ToString();
            model.内存相关 = Environment.WorkingSet.ToString();
            model.硬盘 = Environment.GetLogicalDrives();

            model.站点名称 = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
            return model;
        }

        private class ServerInformationModel
        {

            public string 服务器时间 { get; set; } = "";
            public string 占用总内存 { get; set; } = "";

            public string 是否是Linux { get; set; } = "";

            public string 是否是OSX { get; set; } = "";

            public string 是否是Windows { get; set; } = "";
            public string 进程架构 { get; set; } = "";
            public string 系统架构 { get; set; } = "";
            public string 系统名称 { get; set; } = "";
            public string 是否64位操作系统 { get; set; } = "";

            public string CPU_CORE { get; set; } = "";

            public string HostName { get; set; } = "";
            public string Version { get; set; } = "";

            public string 内存相关 { get; set; } = "";

            public string[] 硬盘 { get; set; } = Array.Empty<string>();

            public string? 站点名称 { get; set; }

          
        }
    }
}
