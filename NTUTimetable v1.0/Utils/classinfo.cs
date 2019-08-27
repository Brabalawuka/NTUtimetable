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

        public string classType { get; set; }
        public string venue { get; set; }
        public string group { get; set; }
        public List<int> weekSpan { get; set; }
        public int rowTime { get; set; }
        public int colDay { get; set; }
        public int rowSpanDuration { get; set; }
    }

    public class CourseInfo
    {
        public List<CourseIndex> allIndex { get; set; }
        public string courseCode;
        public string courseIndex;
        public string ExamInfo;

        public JArray ClassArray { get; set; }

        public CourseInfo(string name)
        {
            this.courseCode = name;
            allIndex = new List<CourseIndex>();
        }
        public CourseInfo() { }
    }


    public class ExamInfo
    {
        public string Course;
        public string Day;
        public string Date;
        public string Time;
        public float Duration;
    }

    public class CourseIndex
    {
        public string courseName;
        public string name;
        public List<int> WeekSpan;
        public bool[,,] classes = new bool[14, 7, 14];
        public CourseIndex(string name, string courseName)
        {
            this.name = name;
            this.courseName = courseName;

        }
        public CourseIndex() { }

    }

    public struct Combination
    {
        public List<CourseIndex> indexCombi;
        public int conflict;
    }

}
