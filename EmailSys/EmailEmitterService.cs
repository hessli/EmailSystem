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
        public IEmailConfig EmailConfig { get; }
        public string TagName { get; private set; }

        private EmailEmitter _emailEmitter;
        public IInterceptorEmitter InterceptorEmitter { get; set; }

        public EmailEmitterService(IEmailConfig emailConfig)
        {
            EmailConfig = emailConfig;

            this.CreatEmitter(EmailConfig);

            TagName = this._emailEmitter.TagName;
        }

        public RuningState  State {

            get {
                return _emailEmitter.State;
            }
        }
        public void Start()
        {
            _emailEmitter.onSendComplete += OnSendComplete;
            _emailEmitter.Start();
        }

        public void Stop()
        {
            _emailEmitter.Stop();

        }

        public event SendCompleteHandler OnSendComplete;

        public  void Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
        {

            if (InterceptorEmitter != null)
            {
               var record=  new CommInterceptorRecord(this.TagName);

                record.Count = tos.Count;

               var  isSuccess= InterceptorEmitter.IsInterceptor(record);

                if (!isSuccess)
                {
                    //将当前服务停止
                    //再次触发重发事件
                    this.Stop();
                    if(OnSendComplete!=null)
                    OnSendComplete.Invoke(this,new SendResultEventArgs(this.TagName,tos,subject, GeneratorPackgeId.GetPakcageId(), body,bodyEncoding,subjectEncoding, isHtmlBody, "",SendResult.Ohter,""));
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
      
         static HashSet<uint> cach = new  HashSet<uint>();
         static object syncRoot = new object();
        /// <summary>
        /// 发送器因异常未发送完成的邮件
        /// 转移到其他可用的发送器发送
        /// </summary>
        /// <param name="service"></param>
        /// <param name="args"></param>
        public static void Transfer(EmailEmitterService service, SendResultEventArgs[] args)
        {
            lock (syncRoot)
            {
                if (service != null && args != null && args.Length > 0)
                {
                    foreach (var item in args)
                    {
                        if (cach.Contains(item.PackageId))
                        {
                            cach.Remove(item.PackageId);
                            continue;
                        }
                        cach.Add(item.PackageId);

                        service.Send(item.Tos, item.Subject, item.Body, item.SubjectEncoding, item.BodyEncoding, item.IsBodyHtml, item.AttachmentPath);
                    }
                }
            }
        }
    }
}
