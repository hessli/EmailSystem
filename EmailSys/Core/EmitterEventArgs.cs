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

        //public   void Set(EmitterPackageData data)
        //{
        //    if (data != null)
        //    {

        //        this.AttachmentPath = data.AttachmentPath;

        //        this.IsBodyHtml = data.IsBodyHtml;

        //        this.SubjectEncoding = data.SubjectEncoding;

        //        this.BodyEncoding = data.BodyEncoding;
        //    }
        //}
    }

    //public class EmitterSuccessEventArgs : AbEmitterEventArgs
    //{
    //    public EmitterSuccessEventArgs(uint packageId,string tagName, IList<string> tos, string subject, string body)
    //        : base(packageId,tagName, tos, subject, body)
    //    {

             
    //    }
    //}

    //public class EmitterErrorEventArgs : AbEmitterEventArgs
    //{
    //    private ErrorLevel _errorLevel;

    //    private Exception _exception;
    //    public EmitterErrorEventArgs(uint packageId,string tagName, IList<string> tos, string subject, string body, ErrorLevel level, Exception ex) 
    //        :base(packageId, tagName,tos,subject,body)
    //    {
    //        _errorLevel = level;

    //        _exception = ex;

    //    } 

    //    public ErrorLevel ErrorLevel
    //    {
    //        get {
    //            return _errorLevel;
    //        }
    //    }

    //    public Exception exception
    //    {
    //        get {

    //            return _exception;
    //        }
    //    }
    //}

    //public class EmitterSmtpErrorEventAgs : AbEmitterEventArgs
    //{
    //    private SmtpException _ex;
    //    public EmitterSmtpErrorEventAgs(uint packageId, string tagName, IList<string> tos, string subject, string body,SmtpException ex) :
    //        base(packageId, tagName, tos, subject, body)
    //    {
    //        _ex = ex;
    //    }
    //    public SmtpException Ex {

    //        get {
    //            return _ex;
    //        }
    //    }
    //}

    //public class EmitterReleasEventArgs : AbEmitterEventArgs
    //{
    //    public EmitterReleasEventArgs(uint packageId, string tagName, IList<string> tos, string subject, string body) :
    //        base(packageId, tagName, tos, subject, body)
    //    {

            
             
    //    }
    //}


    //public class EmitterArgErrorEventArgs : EventArgs
    //{


    //    public EmitterArgErrorEventArgs(Exception ex)
    //    {
    //        Ex = ex;
    //    }
    //    public Exception Ex { get; }

    //}
}
