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

            EmailService emailService = new EmailService();

            emailService.Send("403470046@qq.com", "xxx", "xxx", System.Text.Encoding.UTF8, System.Text.Encoding.UTF8, true,"");
            Console.ReadLine();
        }
    }
}
