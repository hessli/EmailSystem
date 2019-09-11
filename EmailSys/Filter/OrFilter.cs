using System.Collections.Generic;
using EmailSys.Impl;
using EmailSys.Interceptor;

namespace EmailSys.Filter
{
    public class OrFilter : IFilter
    {
         private IList<IFilter> _conditions;
        public OrFilter(IList<IFilter> conditions)
        {
            _conditions = conditions;
        }
        public bool Filter(IList<InterceptorConfig> regulars, Interceptors restrict)
        {
            foreach (var item in _conditions)
            {
                if (item.Filter(regulars, restrict))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
