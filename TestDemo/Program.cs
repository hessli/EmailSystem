using EmailSys;
using EmailSys.Impl;
using System;


namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //如需运行在Base/BaseController.cs  LoadConfig() 方法中修改成可用的发送邮箱账户和密码

            IEmailService emailService = new EmailService();

            emailService.Run();

            var isSuccess = emailService.Send("403470046@qq.com", "xxx", "xxx", System.Text.Encoding.UTF8, System.Text.Encoding.UTF8, true, "");
            Console.ReadLine();
        }
    }
}
