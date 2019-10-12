using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSys.Core
{ 
    public  class SendResultEventArgs : EventArgs
    {

        public SendResultEventArgs(string tagName, EmitterPackageData data,
           SendResult result, Exception ex) 
            : this(tagName,data,result,ex==null?"":ex.Message)
        {

        }
        public SendResultEventArgs(string tagName, EmitterPackageData data, 
            SendResult result, string message)
        {
            Tos = data.Tos;
            Subject = data.Subject;
            Body = data.Body;
            PackageId = data.PackageId;
            TagName = tagName;
            BodyEncoding = data.BodyEncoding;
            SubjectEncoding = data.SubjectEncoding;
            IsBodyHtml = data.IsBodyHtml;
            AttachmentPath = data.AttachmentPath;
            SendResult = result;
            Message = message;
        }

        //public SendResultEventArgs(string tagName, 
        //    EmitterPackageData packageData
        //    ,SendResult result,string message)
        //{
        //    Tos = packageData.Tos;

        //    Subject = packageData.Subject;

        //    Body = packageData.Body;

        //    PackageId = packageData.PackageId;

        //    TagName = tagName;

        //}
        //public SendResultEventArgs(uint packageId,string tagName, IList<string> tos, 
        //    string subject, string body)
        //{
        //    Tos = tos;

        //    Subject = subject;

        //    Body = body;

        //    PackageId = packageId;

        //    TagName = tagName;
        //}

        public uint PackageId { get; private set; }
        public string TagName { get; private set; }

        public DateTime Time { get; } = DateTime.Now;

        public IList<string> Tos { get; private set; }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; private set; }
        /// <summary>
        /// 邮件内容编码
        /// </summary>
        public Encoding BodyEncoding { get; private set; }

        public Encoding SubjectEncoding { get; private set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// 如果要发送html格式的消息，需要设置这个属性
        /// </summary>
        public bool IsBodyHtml { get;private set; }

        public string AttachmentPath { get; private set; }


        public SendResult SendResult { get; private set; }

        public string Message { get; private set; }


}
