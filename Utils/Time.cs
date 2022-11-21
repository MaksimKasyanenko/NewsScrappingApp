using System;
using System.Linq;

namespace NewsParsingApp.Utils
{
    internal struct Time
    {
        public Time(string time)
        {
            int[] t;
            try
            {
                t = time.Split(':').Select(n => Int32.Parse(n)).ToArray();
                if(t.Length != 2) throw new Exception();
            }
            catch
            {
                throw new FormatException($"'{nameof(time)}' argument has invalid format");
            }
            Hours = t[0];
            Minutes = t[1];
        }

        public int Hours {get;}

        public int Minutes {get;}

        public DateTime ToDateTime(int year, int month, int day)
        {
            return new DateTime(year, month, day, Hours, Minutes, 0);
        }
    }
}