//using EmailSys.Impl;
//using System.Collections.Generic;
//using System.Text;

//namespace EmailSys
//{
//    public class EmailService : IEmailService
//    {
//        private EmailEmitterController _controller = null;
//        private bool _loadController = false;
//        /// <summary>
//        /// 服务是否已启动
//        /// </summary>
//        /// <value>true</value>
//        /// <c>false</c>
//        public bool IsInitialed
//        {
//            get
//            {
//                return this._controller != null && this._controller.IsInitialed;
//            }
//        }
//        public EmailService()
//        {
//        }
//        public bool Run()
//        {
//            if (!_loadController)
//            {
//                lock (this)
//                {
//                    this._controller = EmailEmitterController.Instance;
//                    //this._controller.Init();
//                    this._loadController = true;
//                }

//            }

//            //检测服务启动
//            for (int i = 0; i < 1000; i++)
//            {
//                if (_controller.IsInitialed)
//                {
//                    break;
//                }
//                System.Threading.Thread.Sleep(10);
//            }

//            return this._controller.IsInitialed;
//        }

      
//        public bool Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
//        {
//             _controller.Send(tos,subject,body,subjectEncoding,bodyEncoding,isHtmlBody,attachmentPath);

//            return true;
//        }

//        public bool Send(string to, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
//        {
//            IList<string> list = new List<string>();

//            list.Add(to);

//            _controller.Send(list,subject,body,subjectEncoding,bodyEncoding,isHtmlBody,attachmentPath);

//            return true;
//        }
//    }
//}
