using System;

namespace EmailSys.Base
{
    [Serializable]
    public abstract class BaseInterceptorRecord
    {
        private string _tagName;
        public BaseInterceptorRecord(string tagName)
        {
            _tagName = tagName;
        }
        public string Tag
        {
            get
            {

                return _tagName;
            }
        }
        /// <summary>
        /// 发送时间
        /// </summary>
        private DateTime _sendTime;
        public virtual DateTime SendTime
        {
            get
            {
                return _sendTime;
            }
            set {
                _sendTime = value;
            }
        }

        public int Count { get; set; }

    }
}
