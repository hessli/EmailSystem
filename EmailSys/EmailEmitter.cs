using EmailSys.Core;
using EmailSys.Base;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System;
using System.Linq;
namespace EmailSys
{
    public class EmailEmitter:IJob
    {
        private SmtpClient _smtpClient;

        private ConcurrentQueue<EmitterPackageData> _dataQueue;

        private IEmailConfig _emailConfig;
        

        private object _synState = new object();

        public string TagName { get; private set; } = string.Empty;

        public event SendCompleteHandler onSendComplete;

        /// <summary>
        /// 发送器状态
        /// </summary>
        public RuningState State {
            get;
            private set;

        }


        public EmailEmitter(IEmailConfig emailConfig)
        {
            _emailConfig = emailConfig;

            TagName = emailConfig.TagName;

            _dataQueue = new ConcurrentQueue<EmitterPackageData>();
        }

        public void Stop()
        {

            State = RuningState.Stop;
            JobManager.Stop(this);

            var   disp  = this as IDisposable;

            if (disp != null)
                disp.Dispose();
        }

        public void Start()
        {
            State = RuningState.Runing;

            JobManager.InitialStart(this, 3000);
        }

           
    
        public void Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
        {
            var packageId = GeneratorPackgeId.GetPakcageId();

                EmitterPackageData data = new EmitterPackageData(packageId,tos, subject, body);

                data.SubjectEncoding = subjectEncoding;

                data.BodyEncoding = bodyEncoding;

                data.IsBodyHtml = isHtmlBody;

                data.AttachmentPath = attachmentPath;

                this._dataQueue.Enqueue(data);
        }
       
        /// <summary>
        /// 连接
        /// </summary>
        private void Connect()
        {
            _smtpClient = new SmtpClient();

            _smtpClient.Port = _emailConfig.Port;

            _smtpClient.Host = _emailConfig.Host;

            TagName = _emailConfig.TagName;

            _smtpClient.Credentials = new NetworkCredential(_emailConfig.Account, _emailConfig.Credentials);

            switch (_emailConfig.DeliveryMethod)
            {
                case (int)EmailDeliveryMethod.Network:

                    _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    break;
            }
        }
      
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public void Excute()
        {
            lock (_synState)
            {
                while (!_dataQueue.IsEmpty)
                {
                    EmitterPackageData data = null;
                    SendResultEventArgs args=null;
                    Exception exception = null;
                    SendResult result = SendResult.Success;
                    try
                    {
                        if (_dataQueue.TryDequeue(out data))
                        {
                            if (_smtpClient == null)
                            {
                                Connect();
                            }
                            var mailMessage = data.GetSmtpMessage(_emailConfig.Account);

                            _smtpClient.Send(mailMessage);
                            args = new SendResultEventArgs(TagName, data, SendResult.Success, "成功");
                        }
                    }
                    catch (FormatException e)
                    {
                        exception = e;
                        result = SendResult.Args;
                    }
                    catch (SmtpException e)
                    {
                        exception = e;
                        result = SendResult.Smtp;

                    }
                    catch (Exception e)
                    {
                        exception = e;
                        result = SendResult.Ohter;

                    }
                    finally
                    {
                        InvokeComplete(data, result, exception);

                        if (result == SendResult.Ohter || result == SendResult.Smtp)
                        {
                            Stop();
                        }
                    }
                }
                //关闭链接
                CloseConnect();
            }
        }

        private void InvokeComplete(EmitterPackageData data, SendResult result, Exception e)
        {
            if (onSendComplete != null)
            {
                SendResultEventArgs item = new SendResultEventArgs(TagName, data, result, e);

                IList<SendResultEventArgs> args = null;

                if (result == SendResult.Ohter || result == SendResult.Smtp)
                    args = this.GetSurplusData(result);
                else
                    args = new List<SendResultEventArgs>();

                args.Add(item);

                onSendComplete.BeginInvoke(this, args.ToArray(), null, null);
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
        /// 释放
        /// </summary>
        private IList<SendResultEventArgs> GetSurplusData(SendResult result)
        {
            lock (_synState)
            {
                var count = _dataQueue.Count;

                IList<SendResultEventArgs> args = new List<SendResultEventArgs>();
                while (count > 0)
                {
                    EmitterPackageData data = null;
                    if (_dataQueue.TryDequeue(out data))
                    {
                        args .Add(new SendResultEventArgs(TagName, data, result, ""));
                    }
                    count--;
                }
                return args;
            }
        }
    }
}
