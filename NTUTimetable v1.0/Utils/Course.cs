using System;
using System.Collections.Generic;
using System.Text;

namespace ParsingClass
{
    class Course
    {
        public List<CourseIndex> allIndex { get; set; }
        public string name { get; set; }

        public Course(string name)
        {
            this.name = name;
            allIndex = new List<CourseIndex>();
        }

       
    }
}
