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
    public class EmailEmitterController : BaseController
    {
        //static EmailEmitterController _instance = null;
        private ConcurrentQueue<string> _emmitterQueue = new ConcurrentQueue<string>();
        private object _synch = new object();

        private ConcurrentDictionary<string, EmailEmitterService> _dicEmitter = new ConcurrentDictionary<string, EmailEmitterService>();
        public int CurrentCount { get; private set; } = 0;

        public EmailEmitterController()
        {

        }
        //static EmailEmitterController()
        //{
        //    _instance = new EmailEmitterController();
        //}

        //public static EmailEmitterController Instance
        //{
        //    get {
        //        return _instance;
        //    }
        //}
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

                    var enumerator = _emmitterQueue.GetEnumerator();

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
                                CurrentCount--;
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
                            if (service.State == RuningState.Runing)
                            {
                                _emmitterQueue.Enqueue(tagName);
                                break;
                            }
                            service = null;
                            break;
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

            emitter.Send(tos, subject, body, subjectEncoding, bodyEncoding, isHtmlBody, attachmentPath);
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
                    newEmitterService.OnSendComplete += NewEmitterService_OnSendComplete;

                    newEmitterService.Start();

                    _dicEmitter.TryAdd(item.TagName, newEmitterService);

                    _emmitterQueue.Enqueue(item.TagName);
                }
                CurrentCount = _dicEmitter.Count;
            }
        }

        private void NewEmitterService_OnSendComplete(object sender,
            params SendResultEventArgs[] args)
        {
            if (args != null && args.Length > 0)
            {

                if (args[0].SendResult == SendResult.Ohter ||
                    args[0].SendResult == SendResult.Smtp)
                {
                    StopEmitter(args[0].TagName);
                    var service = Scheduled();
                    EmailEmitterService.Transfer(service, args);
                }
                else
                {
                    //dolog.....
                }
            }
        }
        public override bool IsInitialed
        {
            get
            {
                return CurrentCount > 0;
            }
        }
        protected override void Clear()
        {
            lock (_synch)
            {
                EmailEmitterService[] old = new EmailEmitterService[_dicEmitter.Count];

                _dicEmitter.Values.CopyTo(old, 0);

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
