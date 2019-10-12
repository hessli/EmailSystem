using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmailSys
{
    internal static class JobManager
    {
        static Timer _timer = new Timer((state) => ScheduleJobs(), null, Timeout.Infinite, Timeout.Infinite);

        private static readonly ISet<Tuple<IJob, Task>> _running = new HashSet<Tuple<IJob, Task>>();

        private const uint _maxTimerInterval = (uint)0xfffffffe;

        static JobCollection jobs = new JobCollection();

        internal static void InitialStart(IJob job, double interval)
        {
            jobs.Add(new JobConfig
            {
                Interval = interval,
                Job = job
            });
            Start();
        }

        internal static void Start()
        {
            ScheduleJobs();
        }

       internal static void Stop(IJob job)
        {
            jobs.Remove(job);

        }

        static void ScheduleJobs()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            jobs.Sort();

            if (!jobs.Any())
            {
                return;
            }

            var firstJob = jobs.First();

            //递归执行需要立马运行的任务,剩下的是需要执行定时的任务
            if (firstJob.NextRun<=DateTime.Now)
            {
                RunJob(firstJob.Job);

                firstJob.NextRun = DateTime.Now.AddMilliseconds(firstJob.Interval);

                ScheduleJobs();
                return;
            }

           var interval= firstJob.NextRun - DateTime.Now;

            if (interval <= TimeSpan.Zero)
            {
                ScheduleJobs();
                return;
            }
            else
            {
                if (interval.TotalMilliseconds > _maxTimerInterval)
                {
                    interval = TimeSpan.FromMilliseconds(_maxTimerInterval);
                }
                _timer.Change(interval, interval);
            }
        }

        static void RunJob(IJob job)
        {
            lock (_running)
            {
                if (_running.Any(x => ReferenceEquals(x.Item1, job)))
                {
                    return;
                }
            }
            Tuple<IJob, Task> tuple = null;

            var task = new Task(() =>
            {

                try
                {
                    job.Excute();
                }
                catch (Exception e)
                {
                    var aggregate = e as AggregateException;

                    if (aggregate != null && aggregate.InnerExceptions.Count == 1)
                        e = aggregate.InnerExceptions.Single();
                }
                finally
                {
                    lock (_running)
                    {
                        _running.Remove(tuple);
                    }
                }

            }, TaskCreationOptions.PreferFairness);

            tuple = new Tuple<IJob, Task>(job, task);

            lock (_running)
            {
                _running.Add(tuple);
            }
            task.Start();
        }
    }
}
