using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace EmailSys.Core
{ 
    public abstract class AbEmitterEventArgs : EventArgs
    {
        private string _tagName;

        private string _subject;

        private string _body;

        private Encoding _subjectEncoding;

        private Encoding _bodyEncoding;

        private DateTime _time = DateTime.Now;

        private uint _packageId;

        private IList<string> _tos;
        public AbEmitterEventArgs(uint packageId,string tagName, IList<string> tos, string subject, string body)
        {
            _tos = tos;

            _subject = subject;

            _body = body;

            _packageId = packageId;

            _tagName = tagName;
        }

        public uint PackageId {

            get {
                return _packageId;
            }
        }
        public string TagName
        {

            get
            {

                return _tagName;
            }
        }

        public DateTime Time
        {
            get
            {

                return _time;
            }

        }

        public IList<string> Tos
        {

            get {
                return _tos;
            }
        }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject
        {
            get
            {

                return _subject;
            }
        }
        /// <summary>
        /// 邮件内容编码
        /// </summary>
        public Encoding BodyEncoding
        {
            get
            {
                return _bodyEncoding;
            }
            set
            {

                _bodyEncoding = value;
            }
        }


        public Encoding SubjectEncoding
        {
            get
            {
                return _subjectEncoding;
            }
            set
            {

                _subjectEncoding = value;
            }
        }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body
        {
            get
            {
                return _body;
            }
        }

        /// <summary>
        /// 如果要发送html格式的消息，需要设置这个属性
        /// </summary>
        public bool IsBodyHtml { get; set; }

        public string AttachmentPath { get; set; }

        internal virtual void Set(EmitterPackageData data)
        {
            if (data != null)
            {

                this.AttachmentPath = data.AttachmentPath;

                this.IsBodyHtml = data.IsBodyHtml;

                this.SubjectEncoding = data.SubjectEncoding;

                this.BodyEncoding = data.BodyEncoding;
            }
        }
    }

    public class EmitterSuccessEventArgs : AbEmitterEventArgs
    {
        public EmitterSuccessEventArgs(uint packageId,string tagName, IList<string> tos, string subject, string body)
            : base(packageId,tagName, tos, subject, body)
        {

             
        }
    }

    public class EmitterErrorEventArgs : AbEmitterEventArgs
    {
        private ErrorLevel _errorLevel;

        private Exception _exception;
        public EmitterErrorEventArgs(uint packageId,string tagName, IList<string> tos, string subject, string body, ErrorLevel level, Exception ex) 
            :base(packageId, tagName,tos,subject,body)
        {
            _errorLevel = level;

            _exception = ex;

        } 

        public ErrorLevel ErrorLevel
        {
            get {
                return _errorLevel;
            }
        }

        public Exception exception
        {
            get {

                return _exception;
            }
        }
    }

    public class EmitterSmtpErrorEventAgs : AbEmitterEventArgs
    {
        private SmtpException _ex;
        public EmitterSmtpErrorEventAgs(uint packageId, string tagName, IList<string> tos, string subject, string body,SmtpException ex) :
            base(packageId, tagName, tos, subject, body)
        {
            _ex = ex;
        }
        public SmtpException Ex {

            get {
                return _ex;
            }
        }
    }

    public class EmitterReleasEventArgs : AbEmitterEventArgs
    {
        public EmitterReleasEventArgs(uint packageId, string tagName, IList<string> tos, string subject, string body) :
            base(packageId, tagName, tos, subject, body)
        {

            
             
        }
    }


    public class EmitterArgErrorEventArgs : EventArgs
    {


        public EmitterArgErrorEventArgs(Exception ex)
        {
            _ex = ex;
        }

        private Exception _ex;
         public Exception Ex {

            get {
                return _ex;
            }
        }

    }
}
