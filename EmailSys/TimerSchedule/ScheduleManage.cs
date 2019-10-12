using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace EmailSys.TimerSchedule
{
   public   class ScheduleManage
    {
        private Timer _timer = null;
        /// <summary>
        /// 最大时间间隔
        /// </summary>
        private const uint _maxTimerInterval = (uint)0xfffffffe;
        ScheduleManage()
        {
            _timer = new Timer(state=>Schedule(),null,Timeout.Infinite,Timeout.Infinite);
        }

        void Schedule()
        {
              
        }

        void RunJob()
        {

        }
        void Start()
        {
            Schedule();
        }


        void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        void Releas()
        {
           var dis=  _timer as IDisposable;
            if (disposable != null)
                dis.Dispose();
        }
    }
}
