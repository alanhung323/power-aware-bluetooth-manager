﻿using System;

namespace PowerAwareBluetooth.Model
{
    [Serializable]
    public class TimeInterval
    {
        public const int MIN_HOUR = 0;
        public const int MAX_HOUR = 23;
        public const int MIN_MINUTES = 0;
        public const int MAX_MINUTES = 59;

//        private const int UNDEFINED = -1;

        private int m_StartHour;
        private int m_StartMinutes;
        private int m_EndHour;
        private int m_EndMinutes;

        public TimeInterval()
        {
            
        }

        public TimeInterval(int startHour, int startMinutes, int endHour, int endMinutes)
        {
            if (!IsLegalHour(startHour) ||
                !IsLegalHour(endHour) ||
                !IsLegalMinutes(startMinutes) ||
                !IsLegalMinutes(endMinutes) ||
                IsStartBeforeEnd(startHour, startMinutes, endHour, endMinutes))
            {
                throw new ArgumentException("Bad time interval arguments");
            }

            m_StartHour = startHour;
            m_StartMinutes = startMinutes;
            m_EndHour = endHour;
            m_EndMinutes = endMinutes;
        }

        public int StartMinutes
        {
            get { return m_StartMinutes; }
            set { m_StartMinutes = value; }
        }

        public int StartHour
        {
            get { return m_StartHour; }
            set { m_StartHour = value; }
        }

        public int EndMinutes
        {
            get { return m_EndMinutes; }
            set { m_EndMinutes = value; }
        }

        public int EndHour
        {
            get { return m_EndHour; }
            set { m_EndHour = value; }
        }

        public int StartNumber
        {
            get
            {
                return m_StartHour*24 + m_StartMinutes;
            }
        }

        public int EndNumber
        {
            get
            {
                return m_EndHour*24 + m_EndMinutes;
            }
        }

        public static bool IsStartBeforeEnd(int startHour, int startMinutes, int endHour, int endMinutes)
        {
            return (startHour > endHour ||
                    (startHour == endHour && startMinutes > endMinutes));
        }

        public static bool IsLegalHour(int hour)
        {
            return IsInRange(hour, MIN_HOUR, MAX_HOUR);
        }

        public static bool IsLegalMinutes(int minutes)
        {
            return IsInRange(minutes, MIN_MINUTES, MAX_MINUTES);
        }

        private static bool IsInRange(int value, int min, int max)
        {
            return (value >= min && value <= max);
        }

        /// <summary>
        /// tests if the given time (expressed in hour and minute) is
        /// contained in this time-interval.
        /// </summary>
        /// <param name="hour">the hour in the day</param>
        /// <param name="minute">the minute in the hour</param>
        /// <returns>true if is contained in this interval, false otherwise</returns>
        public bool Contains(int hour, int minute)
        {
            bool result = (StartHour < hour && hour < EndHour ||
                           (StartHour == hour && hour < EndHour && StartMinutes <= minute) ||
                           (StartHour < hour && hour == EndHour && minute < EndMinutes) ||
                           (StartHour == EndHour && StartHour == hour && StartMinutes <= minute && minute < EndMinutes));
            return result;
        }

        /// <summary>
        /// tests if the given interval overlaps with this interval
        /// </summary>
        /// <param name="otherTimeInterval">an interval to test</param>
        /// <returns>true if this and the given interval overlap, false otherwise</returns>
        public bool IsOverlap(TimeInterval otherTimeInterval)
        {
            bool res =
                (IsBetween(StartNumber, EndNumber, otherTimeInterval.StartNumber)) ||
                 (IsBetween(StartNumber, EndNumber, otherTimeInterval.EndNumber)) ||
                 (IsBetween(otherTimeInterval.StartNumber, otherTimeInterval.EndNumber, StartNumber)) ||
                  (IsBetween(otherTimeInterval.StartNumber, otherTimeInterval.EndNumber, EndNumber));
            return res;
        }

        /// <summary>
        /// tests if a number is between max and min
        /// </summary>
        /// <param name="min">a min value</param>
        /// <param name="max">a max value</param>
        /// <param name="what">a number that could be between min and max</param>
        /// <returns>true if what is in [min, max], false otherwise</returns>
        private bool IsBetween(int min, int max, int what)
        {
            return (min <= what && what <= max);
        }
    }
}
