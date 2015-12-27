using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    // MARTIAN YEAR = 669 Sols
    // MARTIAN DAY = 24h 39m 35s

    // Martians Season Length (Sols)
    // Spring = 194
    // Summer = 178
    // Autumn = 142
    // Winter = 154

    public class Clock
    {
        private const int DAYS_IN_MARTIAN_YEAR = 669;
        private const int SEASON_SPRING = 0;
        private const int SEASON_SUMMER = 194;
        private const int SEASON_AUTUMN = 194 + 178;
        private const int SEASON_WINTER = 194 + 178 + 142;

        private int _sols;
        private Season _season;

        private double _years;
        private double _days;
        private double _hours;
        private double _minutes;
        private double _seconds;

        public Clock()
        {
            _sols = 0;
            _season = Season.Spring;
            _years = 0;
            _days = 0;
            _hours = 0;
            _minutes = 0;
            _seconds = 0;
        }

        public void Update(GameTime gameTime, ClockSpeed clockSpeed = ClockSpeed.RealTime, int speedMultiplier = 0)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (clockSpeed == ClockSpeed.RealTime)
            {
                _seconds += elapsedTime;
            }
            else
            {
                if (speedMultiplier > 0)
                {
                    elapsedTime = elapsedTime * speedMultiplier;
                }
                
                if (clockSpeed == ClockSpeed.SecondsToMinutes)
                {
                    _minutes += elapsedTime;
                }
                else if (clockSpeed == ClockSpeed.MinutesToHours)
                {
                    _hours += elapsedTime;
                }
                else if (clockSpeed == ClockSpeed.HoursToDays)
                {
                    _days += elapsedTime;
                }
                else if (clockSpeed == ClockSpeed.DaysToYears)
                {
                    _years += elapsedTime;
                }
            }

            if (_seconds >= 60)
            {
                // A MINUTE HAS PASSED
                _seconds = _seconds - 60;
                _minutes++;
            }

            if (_minutes >= 60)
            {
                // AN HOUR HAS PASSED
                _minutes = _minutes - 60;
                _hours++;
            }

            if (_hours >= 24)
            {
                if (_minutes >= 37)
                {
                    if (_seconds >= 35)
                    {
                        // A DAY HAS PASSED
                        _hours = _hours - 24;
                        _minutes = _minutes - 37;
                        _seconds = _seconds - 35;
                        _days++;
                        _sols++;
                    }
                }
            }

            if (_days > DAYS_IN_MARTIAN_YEAR)
            {
                // A YEAR HAS PASSED
                _days = _days - DAYS_IN_MARTIAN_YEAR;
                _years++;
            }

            // Seasons
            if (_days >= SEASON_WINTER)
            {
                _season = Season.Winter;
            }
            else if (_days >= SEASON_AUTUMN)
            {
                _season = Season.Autumn;
            }
            else if (_days >= SEASON_SUMMER)
            {
                _season = Season.Summer;
            }
            else
            {
                _season = Season.Spring;
            }
        }

        public int Sols
        {
            get { return _sols; }
            set { _sols = value; }
        }

        public Season Season
        {
            get { return _season; }
            set { _season = value; }
        }

        public string DebugText
        {
            get
            {
                return "Year: " + _years.ToString("N0") +
                    " / Day: " + _days.ToString("N0") +
                    " / Hour: " + _hours.ToString("N0") +
                    " / Minute: " + _minutes.ToString("N0") +
                    " / Second: " + _seconds.ToString("N0") +
                    " / Season: " + _season.ToString();
            }
        }
    }

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    public enum ClockSpeed
    {
        RealTime,
        SecondsToMinutes,
        MinutesToHours,
        HoursToDays,
        DaysToYears
    }
}
