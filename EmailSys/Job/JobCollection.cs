using System;
using System.Collections.Generic;
using System.Linq;

namespace EmailSys
{
    internal class JobCollection
    {

        private List<JobConfig> _configs = new List<JobConfig>();

        private object _synch = new object();

        internal void Add(JobConfig item)
        {
            lock (_synch)
            {
                _configs.Add(item);
            }
        }
        internal JobConfig First()
        {
            lock (_synch)
            {
                return _configs.FirstOrDefault();
            }
        }
        internal void Sort()
        {
            lock (_synch)
            {
            _configs.Sort((x,y)=>DateTime.Compare(x.NextRun,y.NextRun));
            }
           
        }

        internal void Remove(JobConfig item)
        {
            lock (_synch)
            {

                _configs.Remove(item);
            }
        }

        internal void Remove(IJob job)
        {
            lock (_synch)
            {
               var i= _configs.Count-1;

                while (i >=0)
                {
                    if (ReferenceEquals(_configs[i].Job, job))
                    {
                        _configs.Remove(_configs[i]);
                    }
                    i--;
                }
            }
        }

        internal bool Any()
        {
            lock (_synch)
            {
                return  _configs.Any();
            }

        }
    }
}
