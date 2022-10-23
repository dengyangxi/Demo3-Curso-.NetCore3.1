using Microsoft.Extensions.DependencyInjection;

namespace Micro.Common.Library.SystemSetup
{
    /// <summary>
    /// Cors 启动服务： 跨域策略
    /// </summary>
    public static class CorsSetup
    {
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //根据配置文件 实施跨域策略。
            services.AddCors(c =>
            {
                if (!Appsettings.app(new string[] { "Startup", "Cors", "EnableAllIPs" }).ObjToBool())
                {
                   
                    c.AddPolicy(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }),

                        policy =>
                        {
                            policy
                            .WithOrigins(Appsettings.app(new string[] { "Startup", "Cors", "IPs" }).Split(','))
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                        });
                }
                else
                {
                    //允许任意跨域请求
                    c.AddPolicy(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }),
                        policy =>
                        {
                            policy
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                        });
                }

            });
        }

        /// <summary>
        /// bool 值转换
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool ObjToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }
    }
}
