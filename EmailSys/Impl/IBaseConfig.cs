
namespace EmailSys.Base
{
  public  interface IBaseConfig
    {
        string Account { get; set; }

        string Host { get; set; }

        string Credentials { get; set; }

        int Port { get; set; }

    }
}
