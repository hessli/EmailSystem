using System;

namespace EmailSys.Base
{
    public abstract class BaseConfig : IBaseConfig
    {

        private string account;
        public string Account
        {
            get
            {

                return account;
            }
            set
            {

                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException();
                account = value;
            }

        }


        private string credentials;
        public string Credentials
        {
            get
            {

                return credentials;
            }
            set
            {

                credentials = value;
            }
        }

        private string host;
        public string Host
        {
            get
            {

                return host;
            }
            set
            {

                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("host");
                host = value;
            }
        }


        private int port;
        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                if (value == 0)
                {
                    this.port = -1;
                    return;
                }
               
                this.port = value;
            }
        }
    }
}
