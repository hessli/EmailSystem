using EmailSys.Core;
using EmailSys.Impl;
using EmailSys.Interceptor;
using System.Collections.Generic;

namespace EmailSys.Base
{


    public abstract class BaseController:IJob
    {
        public string ControllerName { get; }

        static string DEFAULTNAME = "Email";

        private int _accountCount = 0;

        public BaseController(string controllerName)
        {
            ControllerName = controllerName;
        }
        public BaseController() : this(DEFAULTNAME)
        {
            JobManager.InitialStart(this, 7200000);
        }

        protected IList<SmtpHostConfig> LoadConfig()
        {
            //可以读数据库也可以读其配置文件
            IList<SmtpHostConfig> configs = new List<SmtpHostConfig>();

            var wyConfig = new Core.SmtpHostConfig
            {
                Account = "",
                Host = "smtp.163.com",
                Credentials = "",
                Port = 25,
                SSLPort = 465,
                TagName = "163",
            };


            wyConfig.InterceptorConfig = new List<InterceptorConfig>();

            wyConfig.InterceptorConfig.Add(new CommInterceptorConfig(wyConfig.TagName, (int)Frequency.Hour, 1));

            wyConfig.InterceptorConfig.Add(new CommInterceptorConfig(wyConfig.TagName, (int)Frequency.Day, 2));

            configs.Add(wyConfig);

            _accountCount++;

            return configs;
        }

        /// <summary>
        /// 如果加载到了配置文件就说明初始化
        /// 成功了
        /// </summary>
        public virtual bool IsInitialed
        {
            get
            {
                return _accountCount > 0;
            }
        }
        protected abstract void LoadData();

        protected abstract void Clear();

        public void Excute()
        {
            LoadData();
        }
    }
}
