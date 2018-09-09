using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NTUTimetable_v1._0
{
    public class Class_info
    {

        public string CourseType { get; set; }
        public string Venue { get; set; }
        public string group { get; set; }
        public List<int> WeekSpan { get; set; }
        public int Row_Time { get; set; }
        public int Col_day { get; set; }
        public int RowSpan_Duration { get; set; }
    }

    public class Course_info
    {
        public string CourseCode;
        public string CourseIndex;

        public JArray ClassArray { get; set; }
    }
}
