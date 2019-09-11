using System.Collections.Generic;
using EmailSys.Base;
using EmailSys.Impl;

namespace EmailSys.Core
{
    public  class SmtpHostConfig:BaseConfig,IEmailConfig
    {  
        public int SSLPort { get; set; }
    
        public string TagName
        {
            get;
            set;
        }
        public int MaxQueueCount
        {
            get;set;
        }

        public int  DeliveryMethod
        {
            get
            {
                return (int)EmailDeliveryMethod.Network;
            }
        }
        public IList<InterceptorConfig> InterceptorConfig
        {
            get;
            set;
        }
    }
}
