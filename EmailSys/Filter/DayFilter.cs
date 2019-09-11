using EmailSys.Impl;
using System.Collections.Generic;
using EmailSys.Core;
using EmailSys.Interceptor;

namespace EmailSys.Filter
{
    public class DayFilter : IFilter
    {
        public bool Filter(IList<InterceptorConfig> regulars, Interceptors restrict)
        {
            var isSuccess = false;
            foreach (var item in regulars)
            {
                if (item.Frequency == (int)Frequency.Day)
                {
                    var dayRecord=  restrict[Frequency.Day];

                    if (dayRecord == null || dayRecord.Count <= item.MaxCount)
                    {
                        isSuccess = true;
                        break;    
                    }
                }
            }
            return isSuccess;
        }
    }
}
