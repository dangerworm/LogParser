using System;

namespace LogParser
{
    public class DateRange
    {
        private DateTime? _minDateTime;
        private DateTime? _maxDateTime;

        public DateRange()
        {
            _minDateTime = null;
            _maxDateTime = null;
        }

        public DateRange(DateTime minDateTime, DateTime maxDateTime)
        {
            if (minDateTime <= maxDateTime)
            {
                _minDateTime = minDateTime;
                _maxDateTime = maxDateTime;
                return;
            }

            _minDateTime = maxDateTime;
            _maxDateTime = minDateTime;
        }

        public bool IsInitialised()
        {
            return _minDateTime.HasValue && _maxDateTime.HasValue;
        }

        public DateTime GetMinDateTime()
        {
            if (!_minDateTime.HasValue)
            {
                throw new NullReferenceException("The minimum date/time has not been initalised.");
            }

            return _minDateTime.Value;
        }

        public DateTime GetMinDate()
        {
            return GetMinDateTime().Date;
        }

        public DateTime GetMaxDateTime()
        {
            if (!_maxDateTime.HasValue)
            {
                throw new NullReferenceException("The maximum date/time has not been initalised.");
            }

            return _maxDateTime.Value;
        }

        public DateTime GetMaxDate()
        {
            return GetMaxDateTime().Date;
        }

        public DateRange GetMidnightToMidnight()
        {
            var value = new DateRange(
                GetMinDate(),
                GetMaxDate() + new TimeSpan(23, 59, 59)
            );

            return value;
        }

        public void UpdateDateRange(DateTime dateTime)
        {
            if (!_minDateTime.HasValue || dateTime < _minDateTime)
            {
                _minDateTime = dateTime.Date;
            }

            if (!_maxDateTime.HasValue || dateTime > _maxDateTime)
            {
                _maxDateTime = dateTime.Date;
            }
        }
    }
}
