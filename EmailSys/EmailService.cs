using EmailSys.Impl;
using System.Collections.Generic;
using System.Text;
namespace EmailSys
{
    public class EmailService : IEmailService
    {
        private EmailEmitterController _controller = EmailEmitterControllerFactory.GetController();
       
        public bool Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
        {
            _controller.Send(tos, subject, body, subjectEncoding, bodyEncoding, isHtmlBody, attachmentPath);

            return true;
        }

        public bool Send(string to, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath)
        {
            IList<string> list = new List<string>();

            list.Add(to);

            _controller.Send(list, subject, body, subjectEncoding, bodyEncoding, isHtmlBody, attachmentPath);

            return true;
        }
    }

    internal static class EmailEmitterControllerFactory
    {
        static EmailEmitterController _controller;
        static bool _loadController = false;
        static object _syncRoot = new object();
        internal static EmailEmitterController GetController()
        {
            if (!_loadController)
            {
                lock (_syncRoot)
                {
                    _controller = EmailEmitterController.Instance;
                    _loadController = true;
                }

                for (int i = 0; i < 1000; i++)
                {

                    if (_controller.IsInitialed)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }
            return _controller;
        }
    }
}
