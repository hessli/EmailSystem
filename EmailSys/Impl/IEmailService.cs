
using System.Collections.Generic;
using System.Text;

namespace EmailSys.Impl
{
   public interface IEmailService: IBaseService
    {
        bool Send(IList<string> tos, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath);

        bool Send(string to, string subject, string body, Encoding subjectEncoding, Encoding bodyEncoding, bool isHtmlBody, string attachmentPath);

    }
}
