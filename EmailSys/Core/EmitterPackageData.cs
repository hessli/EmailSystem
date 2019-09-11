using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
namespace EmailSys.Core
{
   public class EmitterPackageData
    {
         
        private uint _packageId;

        private string _subject;

        private string _body;

        private Encoding _subjectEncoding = Encoding.UTF8;

        private Encoding _bodyEncoding = Encoding.UTF8;

        private IList<string> _tos;

        public EmitterPackageData(uint packageId, IList<string> tos, string subject, string body)
        {
            if (tos == null || tos.Count==0)
            {
                throw new ArgumentNullException("tos");
            } 

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException("subject");
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentNullException("body");
            }

            _tos = tos;

            _subject = subject;

            _body = body;

            _packageId = packageId;
        
        }

      

        public uint PackageId
        {
            get {

                return _packageId;
            }
        }

        public IList<string> Tos
        {
            get {
                return _tos;
            }
        }
        /// <summary>
        /// 邮件主题编码
        /// </summary>
        public Encoding SubjectEncoding
        {
            get
            {
                return _subjectEncoding;
            }
            set
            {
                if (value != default(Encoding))
                {
                    _subjectEncoding = value;
                }
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
                if (value != default(Encoding))
                {
                    this._bodyEncoding = value;
                }
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
        public Attachment Attachment
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AttachmentPath))
                {
                    var attachment = new Attachment(AttachmentPath);
                    return attachment;
                }
                return null;
            }
        }

        internal MailMessage GetSmtpMessage(string from)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(from);


            message.Subject = this.Subject;

            message.SubjectEncoding = this.SubjectEncoding;

            message.BodyEncoding = this.BodyEncoding;

            message.Body = this.Body;

            message.IsBodyHtml = this.IsBodyHtml;

            if (this.Attachment != null)
            {
                message.Attachments.Add(Attachment);
            }

            foreach (var item in _tos)
            {
                
                    message.To.Add(item);
                
           }
            return message;
        }

    }
}
