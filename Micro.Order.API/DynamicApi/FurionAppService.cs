namespace Micro.Order.API.DynamicApi
{


	/// <summary>
	///  动态API
	/// 继承 IDynamicApiController 接口 或 贴 [DynamicApiController] 特性
	/// </summary>
	public class FurionAppService : IDynamicApiController
	{
		public string Get()
		{
			return $"Hello  Dynamic Api  {nameof(Furion)}";
		}
	}
}
