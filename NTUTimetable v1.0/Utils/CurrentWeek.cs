using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTUTimetable_v1._0
{
    class CurrentWeek
    {
        public int week;

        public CurrentWeek()
        {
            DateTime weeekstart = new DateTime(2020, 8, 9);
            int weekspan = (DateTime.Now.Subtract(weeekstart).Days) / 7;
            
            if (weekspan <= 7)
                week = weekspan >= 1 ? weekspan : 1;
            else
                week = weekspan - 1;
              
        }
    }
}
