using EmailSys.Impl;
using System.Collections;
using System.Collections.Generic;

namespace EmailSys.Base
{
    public interface IEmailConfig:IBaseConfig
    {
         /// <summary>
         /// 当前发送器名称
         /// </summary>
         string TagName { get; set; }

         /// <summary>
         /// 一次最大处理量
         /// </summary>
         int MaxQueueCount { get; set; }

        int DeliveryMethod { get; }

        IList<InterceptorConfig> InterceptorConfig { get; set; }

    }
}
