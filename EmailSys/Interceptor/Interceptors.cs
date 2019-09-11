using EmailSys.Base;
using EmailSys.Core;
using EmailSys.Impl;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Collections;

namespace EmailSys.Interceptor
{
    public class Interceptors: IClearRestricts, IEnumerable<BaseInterceptorRecord>
    {
        private IList<BaseInterceptorRecord> _records;

        private object _sych = new object();

        private int _count = 0;
        public int Count
        {
            get
            {
                return _count;
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

        public BaseInterceptorRecord this[Frequency frequency]
        {

            get
            {
                BaseInterceptorRecord _r = default(BaseInterceptorRecord);
                switch (frequency)
                {
                    case Frequency.Day:
                        _r = this._currentDayRestrict;
                        break;
                    case Frequency.Hour:
                        _r = this._currentHourRestrict;
                        break;
                }

                return _r;
            }
        }

        //当天的记录
        private BaseInterceptorRecord _currentDayRestrict;
        public BaseInterceptorRecord CurrentDayRestrict
        {

            get
            {
                lock (_sych)
                {
                    return _currentDayRestrict;
                }
            }
        }
        //一小时的记录
        private BaseInterceptorRecord _currentHourRestrict;

        public BaseInterceptorRecord CurrentHoursRestrict
        {
            get
            {
                lock (_sych)
                {
                    return _currentDayRestrict;
                }
            }
        }

        public Interceptors(string tagName)
        {
            _tagName = tagName;

            _records = new List<BaseInterceptorRecord>();
        }


        private int GetSubtractDay(DateTime currentTime)
        {
            var subtractDay = currentTime.Subtract(_currentDayRestrict.SendTime).Days;

            return subtractDay;
        }

        private void SetDayRestrict(BaseInterceptorRecord restrict)
        {
            if (_currentDayRestrict == null)
            {
                _currentDayRestrict = ObjectCopier.Clone(restrict);
            }
            else
            {
                var subtractDay = GetSubtractDay(restrict.SendTime);

                _currentDayRestrict.SendTime = restrict.SendTime;

                if (subtractDay == 0)
                {
                    _currentDayRestrict.Count += restrict.Count;
                }
                else
                {
                     _currentDayRestrict.Count = restrict.Count;
                }
            }
        }

        private void SetHourRestrict(BaseInterceptorRecord restrict)
        {
            if (_currentHourRestrict == null)
            {
                _currentHourRestrict = ObjectCopier.Clone<BaseInterceptorRecord>(restrict);
            }
            else
            {
                var subtractTime = restrict.SendTime.Subtract(_currentHourRestrict.SendTime);

                if (subtractTime.Hours > 0)
                {
                    _currentHourRestrict.Count = restrict.Count;

                    _currentHourRestrict.SendTime = restrict.SendTime;
                }
                else
                {
                    _currentHourRestrict.Count += restrict.Count;
                }
            }
        }
        public void Add(BaseInterceptorRecord restrict)
        {
            lock (_sych)
            {
                if (CurrentDayRestrict!=null && GetSubtractDay(restrict.SendTime) > 0)
                {
                    Clear();
                }
                _records.Add(restrict);

                SetDayRestrict(restrict);

                SetHourRestrict(restrict);
 
                Interlocked.Increment(ref _count);
            }
        }

        public void Clear()
        {
            lock (_sych)
            {
                _records.Clear();

                if (_currentDayRestrict != null)
                {
                    _currentDayRestrict.SendTime = DateTime.Now;
                    _currentDayRestrict.Count = 0;     
                }

                if (_currentHourRestrict != null)
                {
                    _currentHourRestrict.SendTime = DateTime.Now;

                    _currentHourRestrict.Count = 0;
                }
                _count = 0;
            }
        }

        private BaseInterceptorRecord _current;
        public BaseInterceptorRecord Current
        {
            get
            {
                return _current;
            }
        }


        public IEnumerator<BaseInterceptorRecord> GetEnumerator()
        {
            lock (_sych)
            {
                foreach (var item in _records)
                {
                    _current = item;
                    yield return item;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
