using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NTUTimetable_v1._0
{
    public class ClassInfo
    {

        public string CourseType { get; set; }
        public string Venue { get; set; }
        public string group { get; set; }
        public List<int> WeekSpan { get; set; }
        public int Row_Time { get; set; }
        public int Col_day { get; set; }
        public int RowSpan_Duration { get; set; }
    }

    public class CourseInfo
    {
        public string CourseCode;
        public string CourseIndex;
        public string ExamInfo;

        public JArray ClassArray { get; set; }
    }


    public class ExamInfo
    {
        public string Course;
        public string Day;
        public string Date;
        public string Time;
        public float Duration;
    }
}
