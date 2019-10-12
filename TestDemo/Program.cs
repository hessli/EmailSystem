using EmailSys;
using EmailSys.Impl;
using System;
using System.Collections.Generic;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //如需运行在Base/BaseController.cs  LoadConfig() 方法中修改成可用的发送邮箱账户和密码

            //IEmailService emailService = new EmailService();

            //emailService.Run();

            EmailEmitterController co = new EmailEmitterController();


            while (true)
            {
                if (co.IsInitialed)
                    break;
            }
                 
            List<string> l = new List<string>();
                l.Add("403470046@qq.com");

             co.Send(l, "xxx", "xxx", System.Text.Encoding.UTF8, System.Text.Encoding.UTF8, true,"");
            Console.ReadLine();
        }
    }
}
