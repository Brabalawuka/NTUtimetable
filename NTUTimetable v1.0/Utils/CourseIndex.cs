using System.Collections.Generic;

namespace ParsingClass
{
    
    public class CourseIndex
    {
        public string courseName;
        public string name;
        public List<int> WeekSpan;
        public bool[,,] classes = new bool[14, 7, 14];
        public CourseIndex(string name, string courseName) {
            this.name = name;
            this.courseName = courseName;
         
        }
        public CourseIndex() { }

    }
    
}
