using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTUTimetable_v1._0
{
    class currentweek
    {
        public int week;

        public currentweek()
        {
            DateTime weeekstart = new DateTime(2019, 8, 4);
            int weekspan = (DateTime.Now.Subtract(weeekstart).Days) / 7;
            if (weekspan <= 7)
                this.week = weekspan;
            else
                this.week = weekspan - 1;
                
        }
    }
}
