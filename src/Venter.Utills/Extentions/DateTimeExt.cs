using System;
using System.Collections.Generic;
using System.Text;

namespace Multiblog.Utills.Extentions
{
    public static class DateTimeExt
    {
        public static DateTime GetRandomDate(this DateTime ret, DateTime from)
        {
            Random ran = new Random((int)DateTime.UtcNow.Ticks);

            return new DateTime(ran.Nextlong(ret.Ticks, from.Ticks));
        }
    }
}
