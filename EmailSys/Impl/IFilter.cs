using EmailSys.Interceptor;
using System.Collections.Generic;
namespace EmailSys.Impl
{
    public interface IFilter
    {
       bool Filter(IList<InterceptorConfig> regulars, Interceptors restrict);
    }
}
