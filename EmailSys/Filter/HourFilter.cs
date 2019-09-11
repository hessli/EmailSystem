using EmailSys.Impl;
using System.Collections.Generic;
using EmailSys.Core;
using EmailSys.Interceptor;
namespace EmailSys.Filter
{
    public class HourFilter : IFilter
    {
        public bool Filter(IList<InterceptorConfig> regulars, Interceptors restrict)
        {    
            var isSuccess = false;
            foreach (var item in regulars)
            {
                if (item.Frequency == (int)Frequency.Hour)
                {

                    var hourRecord = restrict[Frequency.Hour];
                    if (hourRecord == null ||
                               hourRecord.Count <= item.MaxCount)
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
