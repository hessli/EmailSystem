using EmailSys.Base;
using EmailSys.Core;
using EmailSys.Filter;
using EmailSys.Impl;
using EmailSys.Interceptor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace EmailSys
{
    /// <summary>
    /// 控制器
    /// </summary>
   public class EmailEmitterController: BaseController
    {
        static EmailEmitterController _instance = null;

        private ConcurrentQueue<string> _emmitterQueue = new ConcurrentQueue<string> ();

        private int _currentCount=0;

        private object _synch = new object();

        private ConcurrentDictionary<string, EmailEmitterService> _dicEmitter=new ConcurrentDictionary<string, EmailEmitterService> ();
        public int CurrentCount
        {
            get {
                return _currentCount;
            }
        }

        public EmailEmitterController()
        {
              
        }

        static EmailEmitterController()
        {
            _instance = new EmailEmitterController();
        }

        public static EmailEmitterController Instance
        {
            get {
                return _instance;
            }
        }

        private void StopEmitter(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
                return;

            lock (_synch)
            {
                var count = _emmitterQueue.Count;

                while (count > 0)
                {
                    var currentName = string.Empty;

                    var enumerator=_emmitterQueue.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        if (!string.IsNullOrWhiteSpace(enumerator.Current) && enumerator.Current.Equals(tagName))
                        {
                            EmailEmitterService service = null;
                            if (_dicEmitter.TryGetValue(tagName, out service))
                            {

                                //不能移除，避免配置错误没有及时更新
                                //_dicEmitter.TryRemove(tagName, out service);

                                service.Stop();

                                count = 0;
                                _currentCount--;
                                break;
                            }
                        }
                    }
                    count--;
                }
            }
        }

        /// <summary>
        /// 调度
        /// </summary>
        /// <returns></returns>
        private EmailEmitterService Scheduled()
        {
            lock (_synch)
            {
                var tagName = string.Empty;

                EmailEmitterService service = null;

                var count = _emmitterQueue.Count;

                while (count > 0)
                {
                    if (_emmitterQueue.TryDequeue(out tagName))
                    {
                        if (_dicEmitter.TryGetValue(tagName, out service))
                        {
                            if (service.RuningState == RuningState.Runing)
                            {
                                _emmitterQueue.Enqueue(tagName);
                                break;
                            }
                        }
                    }
                    count--;
                }

                return service;
            }
        }
        public void Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
        {
            var emitter = Scheduled();

            emitter.Send(tos,subject,body,subjectEncoding,bodyEncoding,isHtmlBody,attachmentPath);
        }
        protected override void LoadData()
        {
            IList<SmtpHostConfig> newList = base.LoadConfig();

            if (newList == null || newList.Count == 0)
            {
                //未加载配置信息
                throw new ArgumentException("config not found");
            }

            lock (_synch)
            {
                foreach (var item in newList)
                {
                    EmailEmitterService emitterService = null;

                    if (_dicEmitter.TryGetValue(item.TagName, out emitterService))
                    {
                        IEmailConfig emitterConfig = emitterService.EmailConfig;

                        if (emitterConfig.Host != item.Host ||
                           emitterConfig.Account != item.Account ||
                           emitterConfig.Credentials != item.Credentials ||
                           emitterConfig.Port != item.Port ||
                           emitterConfig.DeliveryMethod != item.DeliveryMethod
                           )
                        {
                            _dicEmitter.TryRemove(item.TagName, out emitterService);

                            emitterService.Stop();
                        }
                        else
                        {
                            continue;
                        }
                    }


                    EmailEmitterService newEmitterService = new EmailEmitterService(item);
                    if (item.InterceptorConfig != null && item.InterceptorConfig.Count > 0)
                    {
                        var filters = new List<IFilter>();

                        filters.Add(new DayFilter());

                        filters.Add(new HourFilter());

                        var interceptorEmitter = new InterceptorEmitter(item.InterceptorConfig, filters, item.TagName);

                        newEmitterService.InterceptorEmitter = interceptorEmitter;
                    }

                    newEmitterService.OnSuccess += NewEmitterService_OnSuccess;

                    newEmitterService.OnReleas += NewEmitterService_OnReleas;

                    newEmitterService.OnSmtpError += NewEmitterService_OnSmtpError;

                    newEmitterService.OnError += NewEmitterService_OnError;

                    newEmitterService.OnArgError += NewEmitterService_OnArgError;

                    newEmitterService.Start();

                    _dicEmitter.TryAdd(item.TagName,newEmitterService);

                    _emmitterQueue.Enqueue(item.TagName);
                }
                _currentCount = _dicEmitter.Count;
            }
        }

        private void NewEmitterService_OnArgError(object sender, EmitterArgErrorEventArgs args)
        {
            //dologo
        }

        private void NewEmitterService_OnError(object sender, EmitterErrorEventArgs args)
        {
             //dologo
        }

        private void NewEmitterService_OnSmtpError(object sender, EmitterSmtpErrorEventAgs args)
        {
            //dolog
            var tagName = string.Empty;
            if (args != null )
            {
                tagName = args.TagName;

                StopEmitter(tagName);

                EmailContainer container = new EmailSys.EmailContainer();

                var service = this.Scheduled();

                container.AddServiceComponent(service, args);
            }
        }
        private void NewEmitterService_OnReleas(object sender, EmitterReleasEventArgs[] args)
        {
            var tagName = string.Empty;
            if (args != null && args.Length > 0)
            {
                tagName = args[0].TagName;

                StopEmitter(tagName);

                EmailContainer container = new EmailSys.EmailContainer();

                var service= this.Scheduled();

                container.AddServiceComponent(service,args);
            }
        }

        private void NewEmitterService_OnSuccess(object sender, EmitterSuccessEventArgs args)
        {
           //dolog
        }

        public override bool IsInitialed
        {
            get
            {
                return  _currentCount > 0;
            }
        }

        protected override void Clear()
        {
            lock (_synch)
            {
                EmailEmitterService[] old = new EmailEmitterService[_dicEmitter.Count];

                _dicEmitter.Values.CopyTo(old,0);

                _dicEmitter.Clear();

                var count = _emmitterQueue.Count;

                while (count > 0)
                {
                    var tag = string.Empty;
                    _emmitterQueue.TryDequeue(out tag);
                    count--;
                }
            }
        }
    }
}
