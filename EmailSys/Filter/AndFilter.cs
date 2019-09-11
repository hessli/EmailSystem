using EmailSys.Impl;
using System.Collections.Generic;
using EmailSys.Interceptor;

namespace EmailSys.Filter
{
    public class AndFilter : IFilter
    {
        private IList<IFilter> _conditions;

        public AndFilter(IList<IFilter> conditions)
        {
            _conditions = conditions;
        }

        public bool Filter(IList<InterceptorConfig> regulars, Interceptors restrict)
        {
            var isSuccess = true;
            foreach (var item in _conditions)
            {
                isSuccess= item.Filter(regulars,restrict);
                if (!isSuccess)
                {
                    break;
                }
            }
            return isSuccess;
        }
    }
}
