using EmailSys.Core;
using EmailSys.Base;
using System.Collections.Generic;
using System.Text;
using EmailSys.Impl;
using EmailSys.Filter;

namespace EmailSys
{
   public class EmailEmitterService
    {
        private IEmailConfig _emailConfig;
        public IEmailConfig EmailConfig
        {
            get
            {
                return _emailConfig;
            }
        }
        private string _tagName;
        private string TagName
        {

            get {

                return _tagName;
            }
        }
        /// <summary>
        /// 应用程序出现异常
        /// </summary>
        public event EmitterErrorEventHandler OnError;

        /// <summary>
        /// smtp异常
        /// </summary>
        public event EmitterSmtpErrorEventHandler OnSmtpError;
        /// <summary>
        /// 服务停止事件
        /// </summary>
        public event EmitterReleasEventHandler OnReleas;

        /// <summary>
        /// 发送成功
        /// </summary>
        public event EmitterSuccessEventHandler OnSuccess;

        public event EmitterArgErrorEventHandler OnArgError;

        private EmailEmitter _emailEmitter;

        private RuningState _runingState;
        public RuningState RuningState
        {
            get {
                if (_emailEmitter == null)
                {
                    _runingState= RuningState.Stop;
                }
                else
                {
                    _runingState = _emailEmitter.RuningState;
                }
                return _runingState;
            }
        }

        private IInterceptorEmitter _interceptorEmitter;
        public IInterceptorEmitter  InterceptorEmitter {

            get {
                return _interceptorEmitter;
            }set
            {
                _interceptorEmitter = value;
            }
        }

        public EmailEmitterService(IEmailConfig emailConfig)
        {
            _emailConfig = emailConfig;

       

            this.CreatEmitter(_emailConfig);

            _tagName = this._emailEmitter.TagName;
        }
        public void Start()
        {
            _emailEmitter.OnSmtpError += OnSmtpError;

            _emailEmitter.OnReleas += OnReleas;

            _emailEmitter.OnSuccess += OnSuccess;

            _emailEmitter.OnError += OnError;

            _emailEmitter.OnArgError += OnArgError;

            _emailEmitter.Run();
        }

        public void Stop()
        {
            if (_emailEmitter != null)
            {
                _emailEmitter.Stop();

                _emailEmitter = null;
            }
        }

        public  void Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
        {

            if (_interceptorEmitter != null)
            {
               var record=  new CommInterceptorRecord(this.TagName);

                record.Count = tos.Count;

               var  isSuccess= _interceptorEmitter.IsInterceptor(record);

                if (!isSuccess)
                {
                    //此处可以做很多事情
                    //比如触发重发事件
                    return;
                }
            }

            _emailEmitter.Send(tos,subject,body,subjectEncoding,bodyEncoding,isHtmlBody,attachmentPath);
        }

        //每小时发送三条
        //每天最多发送五十条

        /// <summary>
        /// 构造发射器
        /// </summary>
        /// <param name="config"></param>
        public void CreatEmitter(IEmailConfig config)
        {
            SmtpHostConfig eConfig = new SmtpHostConfig();

            eConfig.Account = config.Account;

            eConfig.Credentials = config.Credentials;

            eConfig.Port = config.Port;

            eConfig.TagName = config.TagName;

            eConfig.Host = config.Host;

            EmailEmitter emailEmitter = new EmailEmitter(eConfig);

            _emailEmitter = emailEmitter;
        }
    }
}
