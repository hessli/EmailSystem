using System;

namespace EmailSys
{
    internal class JobConfig
    {
        internal IJob Job { get; set; }

        internal double Interval { get; set; }

        internal DateTime NextRun { get; set; }  

    }


   
}
