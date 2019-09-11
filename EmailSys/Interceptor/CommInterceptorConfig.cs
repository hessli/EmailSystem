using EmailSys.Impl;

namespace EmailSys.Interceptor
{
    public class CommInterceptorConfig : InterceptorConfig
    {

        private int _frequency;

        private int _maxCount;

        private string _tagName;
        public CommInterceptorConfig(string tagName,int frequency,int maxCount)
        {

            _frequency = frequency;

            _maxCount = maxCount;

            _tagName = tagName;
        }
        public int Frequency
        {
            get {
                return _frequency;
            }
           
        }

        public int MaxCount
        {
            get {

                return _maxCount;
            }
        }

        public string TagName
        {
            get {
                return _tagName;
            }
        }
    }
}
