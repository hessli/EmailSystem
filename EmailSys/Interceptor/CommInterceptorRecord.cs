
using System;

namespace EmailSys.Filter
{

    [Serializable]
   public class CommInterceptorRecord : Base.BaseInterceptorRecord
    {
        public CommInterceptorRecord(string tagName) : base(tagName)
        {
        }

        private DateTime _sendTime=DateTime.Now;
        public override DateTime SendTime
        {
            get
            {
                return _sendTime;
            }
            set
            {
                if (_sendTime == default(DateTime))
                {
                    value = DateTime.Now;
                }
                _sendTime = value;
            }
        }

    }

}
