using EmailSys.Base;
using System.Collections.Generic;
namespace EmailSys.Impl
{
    public interface IInterceptorEmitter
    {
        /// <summary>
        /// 属于哪个发送器的拦截器
        /// </summary>
        string TagName { get;  }
        /// <summary>
        /// 规则配置
        /// </summary>
         IList<InterceptorConfig> Configs { get; }

        /// <summary>
        /// 当前记录是否需要拦截
        /// </summary>
        /// <returns></returns>
        bool IsInterceptor(BaseInterceptorRecord record);
    }
}
