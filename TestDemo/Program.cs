using EmailSys;
using System;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //如需运行在Base/BaseController.cs  LoadConfig() 方法中修改成可用的发送邮箱账户和密码

            EmailService emailService = new EmailService();

            emailService.Send("403470046@qq.com", "里面有两个发送器", "里面有两个发送器", System.Text.Encoding.UTF8, System.Text.Encoding.UTF8, true,"");

            emailService.Send("hnyxlmq@163.com", "我是网易", "我是网易", System.Text.Encoding.UTF8, System.Text.Encoding.UTF8, true, "");
            Console.ReadLine();
        }
    }
}
