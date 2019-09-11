using EmailSys.Core;
using EmailSys.Impl;
using EmailSys.Interceptor;
using System;
using System.Collections.Generic;
using System.Timers;

namespace EmailSys.Base
{
    public abstract class BaseController
    {
        private string _controllerName;

        public string ControllerName
        {
            get
            {
                return _controllerName;
            }
        }

        static string DEFAULTNAME = "Email";

        private int _syncSpan = 3000;

        private Timer _timer;

        private int _accountCount = 0;

        public BaseController(string controllerName)
        {
            _controllerName = controllerName;
        }
        public BaseController() : this(DEFAULTNAME)
        {

        }

        public void Init()
        {
            lock (this)
            {
                try
                {
                    LoadData();
                }
                catch (Exception ex)
                {

                }
                _timer = new Timer();

                _timer.Interval = _syncSpan;

                _timer.AutoReset = false;

                _timer.Elapsed += LoadElapsed;

                _timer.Start();
            }

        }

        protected IList<SmtpHostConfig> LoadConfig()
        {
            //可以读数据库也可以读其配置文件
            IList<SmtpHostConfig> configs = new List<SmtpHostConfig>();

            var wyConfig = new Core.SmtpHostConfig
            {
                Account = "邮箱账户",
                Host = "smtp.163.com",
                Credentials = "邮箱密码",
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

        protected void LoadElapsed(object sender, ElapsedEventArgs args)
        {

            //定时更新配置文件
            Timer timer = sender as Timer;

            lock (this)
            {
                try
                {
                    LoadData();
                }
                catch (Exception ex)
                {


                }
                timer.Interval = _syncSpan;
                timer.Start();
            }

        }


        public virtual void Release()
        {

            lock (this)
            {

                //释放控制器
                if (this._timer != null)
                {
                    this._timer.Stop();
                    this._timer.Dispose();
                    _timer = null;
                }
                //释放发送器
                Clear();
            }
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
    }
}
