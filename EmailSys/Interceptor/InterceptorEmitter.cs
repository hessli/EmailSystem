using EmailSys.Impl;
using System.Collections.Generic;
using EmailSys.Filter;
using EmailSys.Base;

namespace EmailSys.Interceptor
{
    public class InterceptorEmitter : IInterceptorEmitter
    {
        private IList<IFilter> _filters;

        public  IList<IFilter> Filters
        {
            get
            {
                return _filters;
            }
        }

        private IList<InterceptorConfig> _configs;
        public IList<InterceptorConfig> Configs
        {
            get
            {

                return _configs;
            }
        }
        private Interceptors _restricts;

        public Interceptors Restricts
        {
            get {

                return _restricts;
            }
        }

        private string _tagName;
        public string TagName
        {
            get
            {
                return _tagName;
            }
        }


        private IFilter _andFilter;

        public InterceptorEmitter(IList<InterceptorConfig> configs, 
            IList<IFilter> filters, string tagName)
        {
            _tagName = tagName;

            _restricts = new Interceptors(tagName);

            _configs = configs;

            _filters = filters;

            _andFilter=new  AndFilter(_filters);
        }
        private void AddSendRecord(BaseInterceptorRecord restrict)
        {
            _restricts.Add(restrict);
        }

        //开始拦截记录
        public bool IsInterceptor(BaseInterceptorRecord restrict)
        {
            this.AddSendRecord(restrict);

           return _andFilter.Filter(_configs,_restricts);  
        }

    }
}
