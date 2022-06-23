using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Common.Library
{
    /// <summary>
    /// 返回数据基础类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// T：结构  类型参数必须是值类型。可以指定除 Nullable 以外的任何值类型。有关更多信息，请参见使用可空类型（C# 编程指南）。 
    /// T：类 类型参数必须是引用类型，包括任何类、接口、委托或数组类型。
    /// T：new ()类型参数必须具有无参数的公共构造函数。当与其他约束一起使用时，new () 约束必须最后指定。
    /// T：<基类名> 类型参数必须是指定的基类或派生自指定的基类。
    /// T：<接口名称> 类型参数必须是指定的接口或实现指定的接口。可以指定多个接口约束。约束接口也可以是泛型的。
    /// T：U 为 T 提供的类型参数必须是为 U 提供的参数或派生自为 U 提供的参数。这称为裸类型约束。
    [Serializable]
    public class BaseModel<T> //where T : class
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; } = string.Empty;
        /// <summary>
        /// 请求状态： 200 正常
        /// </summary>
        public int Code { get; set; } = 200;
    }
}
