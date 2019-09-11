using EmailSys.Core;
using System.Collections.Generic;

namespace EmailSys
{
    //这是一个中间类
    public class EmailContainer
    {
         static  HashSet<uint> cach = new HashSet<uint>();
        public EmailContainer()
        {

        }
        /// <summary>
        /// 添加部件
        /// 这一块可以以后优化吧
        /// </summary>
        public void AddServiceComponent(EmailEmitterService service, EmitterReleasEventArgs[] args)
        {
            if (service != null && args != null && args.Length > 0)
            {
                foreach (var item in args)
                {
                    lock (cach)
                    {
                        if (cach.Contains(item.PackageId))
                        {
                            cach.Remove(item.PackageId);
                            //处理两次都发送不成功的号码比如记录日志什么的等等等。。。。。。
                            continue;
                        }
                        cach.Add(item.PackageId);

                        service.Send(item.Tos, item.Subject, item.Body, item.SubjectEncoding, item.BodyEncoding, item.IsBodyHtml, item.AttachmentPath);
                    }
                }
            } 
        }

        public void AddServiceComponent(EmailEmitterService service, EmitterSmtpErrorEventAgs args)
        {

            if (service != null && args != null)
            {
                lock (cach)
                {
                    if (cach.Contains(args.PackageId))
                    {
                        cach.Remove(args.PackageId);
                        //处理两次都发送不成功的号码比如记录日志什么的等等等。。。。。。
                        return;
                    }
                    cach.Add(args.PackageId);

                    service.Send(args.Tos, args.Subject, args.Body, args.SubjectEncoding, args.BodyEncoding, args.IsBodyHtml, args.AttachmentPath);
                }
            }

        }

    }
}
