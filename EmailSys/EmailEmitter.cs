using EmailSys.Core;
using EmailSys.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmailSys
{
    public class EmailEmitter
    {
        private SmtpClient _smtpClient;

        private ConcurrentQueue<EmitterPackageData> _dataQueue;

        private IEmailConfig _emailConfig;

        private object _synPackeIdObj = new object();

        private uint _packageId;

        private RuningState _runingState;
        public RuningState RuningState
        {
            get
            {
                return _runingState;
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

        private Task _task;

        private object _synState = new object();

        private string _tagName = string.Empty;
        public string TagName
        {
            get
            {
                return _tagName;
            }
        }


        private bool _isRunReaseShutDowm = false;

        public EmailEmitter(IEmailConfig emailConfig)
        {
            _emailConfig = emailConfig;

            _dataQueue = new ConcurrentQueue<EmitterPackageData>();

            Connect();
        }


        private uint GetPakcageId()
        {
            uint packageId;
            lock (_synPackeIdObj)
            {
                this._packageId += 1u;
                if (this._packageId == 0u)
                {
                    this._packageId += 1u;
                }
                packageId = this._packageId;
            }
            return packageId;
        }
        /// <summary>
        /// 销毁
        /// </summary>
    
        public void Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
        {

            try
            {

                var packageId = this.GetPakcageId();

                EmitterPackageData data = new EmitterPackageData(packageId,tos, subject, body);

                data.SubjectEncoding = subjectEncoding;

                data.BodyEncoding = bodyEncoding;

                data.IsBodyHtml = isHtmlBody;

                data.AttachmentPath = attachmentPath;

                this._dataQueue.Enqueue(data);
            }
            catch (ArgumentNullException ex)
            {
                if (OnArgError != null)
                {
                    OnArgError.BeginInvoke(this,new EmitterArgErrorEventArgs(ex),null,null);
                }
            }
        }
       
        /// <summary>
        /// 连接
        /// </summary>
        private void Connect()
        {
            _smtpClient = new SmtpClient();

            _smtpClient.Port = _emailConfig.Port;

            _smtpClient.Host = _emailConfig.Host;

            _tagName = _emailConfig.TagName;

            _smtpClient.Credentials = new NetworkCredential(_emailConfig.Account, _emailConfig.Credentials);

            switch (_emailConfig.DeliveryMethod)
            {
                case (int)EmailDeliveryMethod.Network:

                    _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    break;
            }
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Stop()
        {
            lock (_synState)
            {
                if (_runingState == RuningState.Runing)
                {
                    //关闭服务
                    CloseConnect();
                    if (_task != null)
                    {
                        try
                        {
                            _runingState = RuningState.Stop;
                            _task.Wait();
                        }
                        catch (AggregateException ex)
                        {

                        }
                        finally
                        {
                            _task.Dispose();
                            _task = null;

                            if (!_isRunReaseShutDowm)
                            {
                                RaiseReleasShutDown();

                                _isRunReaseShutDowm = false;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            if (_task == null)
            {
               _task = new Task(Runable);

                _task.Start();

                _runingState = RuningState.Runing;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        private void Runable()
        {
            lock (_synState)
            {
                while (true)
                {
                    if (_dataQueue.IsEmpty)
                    {
                        CloseConnect();
                        Thread.Sleep(10);
                    }
                    EmitterPackageData data = null;
                    if (_dataQueue.TryDequeue(out data))
                    {
                        try
                        {
                            if (_smtpClient == null)
                            {
                                Connect();
                            }
                            var mailMessage = data.GetSmtpMessage(_emailConfig.Account);

                            _smtpClient.Send(mailMessage);

                            RaiseSuccess(data);
                        }
                        catch (FormatException argumentFomart)
                        {
                            RaiseException(data, ErrorLevel.Args, argumentFomart);
                        }
                        catch (SmtpException e)
                        {
                            _runingState = RuningState.Stop;

                            RaiseSmtpException(data, e);

                            break;
                        }
                        catch (Exception e)
                        {
                            RaiseException(data,ErrorLevel.Application, e);
                            break;
                        }
                    }
                }
                //运行状态改为停止
                _runingState = RuningState.Stop;
                //关闭链接
                CloseConnect();
                // 触发事件
                RaiseReleasShutDown();

                _isRunReaseShutDowm = true;
            }
        }
        /// <summary>
        /// 关闭链接
        /// </summary>
        private void CloseConnect()
        {
            lock (_synState)
            {
                if (_smtpClient != null)
                {
                    _smtpClient.Dispose();

                    _smtpClient = null;
                }
            }
        }

        /// <summary>
        /// 发送成功
        /// </summary>
        /// <param name="data"></param>
        private void RaiseSuccess(EmitterPackageData data)
        {
            if (this.OnSuccess != null)
            {
                EmitterSuccessEventArgs args = new EmitterSuccessEventArgs(data.PackageId, this._tagName, data.Tos, data.Subject, data.Body);

                args.Set(data);

                OnSuccess.BeginInvoke(this, args, null, null);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        private void RaiseReleasShutDown()
        {
            lock (_synState)
            {
                if (OnReleas != null)
                {
                    var count = _dataQueue.Count;

                    EmitterReleasEventArgs[] args = new EmitterReleasEventArgs[count];

                    while (count > 0)
                    {
                        EmitterPackageData data = null;

                        if (_dataQueue.TryDequeue(out data))
                        {
                            var item = new EmitterReleasEventArgs(data.PackageId, this._tagName, data.Tos, data.Subject, data.Body);

                            item.Set(data);

                            args[count - 1] = item;
                        }
                        count--;
                    }

                    OnReleas.BeginInvoke(this, args, null, null);
                }
            }
        }

        /// <summary>
        /// 出现异常
        /// </summary>
        /// <param name="data"></param>
        /// <param name="level"></param>
        /// <param name="e"></param>
        private void RaiseException(EmitterPackageData data, ErrorLevel level, Exception e)
        {
            if (OnError == null)
                return;

            EmitterErrorEventArgs args = null;

            args = new Core.EmitterErrorEventArgs(data.PackageId, this._tagName, data.Tos, data.Subject, data.Body, level, e);

            args.Set(data);
        
            OnError.BeginInvoke(this, args, null, null);
        }

        /// <summary>
        /// Smtp服务器异常
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        private void RaiseSmtpException(EmitterPackageData data, SmtpException ex)
        {
            if (this.OnSmtpError != null)
            {
                EmitterSmtpErrorEventAgs args = new EmitterSmtpErrorEventAgs(data.PackageId, this._tagName, data.Tos, data.Subject, data.Body, ex);

                args.Set(data);

                this.OnSmtpError.BeginInvoke(this, args, null, null);
            }
        }

     
    }
}
