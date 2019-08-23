using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp;

namespace ParsingClass
{
    class Program
    {
        public static List<Course> myCourse = new List<Course>();
        public static void Main(string[] args)
        {

            string[] courseName = { "CZ2004", "CZ3002", "CZ3004", "CZ3005", "CZ3007", "CZ4045" };
            for (int i = 0; i < courseName.Length; i++)
            {
                string requestAddress = "https://wish.wis.ntu.edu.sg/webexe/owa/AUS_SCHEDULE.main_display1?acadsem=2019;1&r_search_type=F&r_subj_code=" + courseName[i] + "&boption=Search&staff_access=false";
                MainAsync(requestAddress, courseName[i]).Wait();
            }

            findCombination(myCourse);



        }


        static async Task MainAsync(string requestAddress, string courseName)

        {
            
            Course newCourse = new Course(courseName);
            
            //WebRequest Config
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(requestAddress);
            var cells = document.QuerySelectorAll("TR[BGCOLOR]"); // DataBlock
            var cellsmore = document.QuerySelectorAll("TR[BGCOLOR='#EBFAFF']");
            CourseIndex courseIndex = null;


            foreach (var block in cells)
            {

                String blkinfo = block.TextContent;
                Console.WriteLine(blkinfo);
                var lineInfo = blkinfo.Split("\n");
                if (lineInfo[1].Length == 5)
                {
                    if (courseIndex != null)
                    {
                        newCourse.allIndex.Add(courseIndex);

                        //test
                        for (int i = 0; i < courseIndex.classes.GetLength(2); i++)
                        {
                            for (int y = 0; y < courseIndex.classes.GetLength(1); y++)
                            {
                                for (int x = 0; x < courseIndex.classes.GetLength(0); x++)
                                {
                                    Console.Write((courseIndex.classes[x, y, i] == true) ? "True" : "----") ;
                                }
                                Console.WriteLine();
                               
                            }
                            Console.WriteLine();
                        }
                    }

                    courseIndex = new CourseIndex(lineInfo[1], courseName);
                };

                var timeStartEnd = lineInfo[5].Split('-');
                List<int> WeekSpan = FindWeekSpan(lineInfo[7]);
                int day = FindCol_day(lineInfo[4]);
                List<int> timeDuration = findTimeDuration(timeStartEnd[0], timeStartEnd[1]);


                foreach (var week in WeekSpan)
                {
                    foreach (var time in timeDuration)
                    {
                        Console.Write(time.ToString()+ " " + day.ToString()+ " "+ week.ToString()+"  ---");
                        
                        courseIndex.classes[time, day, week] = true; 
                    }
                }
                Console.WriteLine();









            }
            newCourse.allIndex.Add(courseIndex);

            myCourse.Add(newCourse);
            Console.WriteLine("==================================");
            


        }

        public static int FindCol_day(string myday)
        {
            switch (myday)
            {
                case "MON":
                    return 1;
                case "TUE":
                    return 2;
                case "WED":
                    return 3;
                case "THU":
                    return 4;
                case "FRI":
                    return 5;
                case "SAT":
                    return 6;
                default:
                    return 0;


            }
        }

        public static List<int> FindWeekSpan(string weekspanremark)
        {

            if (weekspanremark == "")

            {
                List<int> myweekspan = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
                return myweekspan;

            }
            else
            {
                List<int> myweekspanlist = new List<int>();
                weekspanremark = weekspanremark.Remove(0, 11);
                string[] newweekspan = weekspanremark.Split(',');
                foreach (string item in newweekspan)
                {
                    if (!item.Contains("-"))
                    {
                        myweekspanlist.Add(int.Parse(item));
                    }
                    else
                    {
                        string[] newitem = item.Split('-');
                        for (int i = 0; i < 15; i++)
                        {
                            if (i >= int.Parse(newitem[0]) && i <= int.Parse(newitem[1]))
                            {
                                myweekspanlist.Add(i);
                            }
                        }
                    }
                }

                return myweekspanlist;
            }
        }

        public static List<int> findTimeDuration(string startTimeS, string endTimeS) {
            List<int> time = new List<int>();
            int startTime = int.Parse(startTimeS);
            int endTime = int.Parse(endTimeS);

            int startIndex = startTime / 100 - 8;
            int duration = (endTime - startTime) / 100;


            for (int i = 0; i < duration; i++)
            {
                time.Add(startIndex++);
            }

            return time;



        }

        public struct Combination {
            public List<CourseIndex> indexCombi;
            public int conflict;
        }



        public static List<Combination> findCombination(List<Course> courseList) {
            List<Combination> combinationList = new List<Combination>();
            List<Combination> combinationlistTemp = new List<Combination>();


            foreach (var course in courseList)
            {
                combinationlistTemp.Clear();
                foreach (var index in course.allIndex)
                {
                   
                    Console.WriteLine("CombinationList Count:" + course.allIndex.Count);
                    Console.WriteLine("CurrentIndex" + course.allIndex.IndexOf(index));
                    
                    if (courseList.IndexOf(course) == 0)
                    {
                        Combination combination = new Combination
                        {
                            conflict = 0,
                            indexCombi = new List<CourseIndex> {index}
                        };
                        
                        combinationlistTemp.Add(combination);
                        Console.WriteLine(index.name + "1st index ");
                    }
                    else
                    {
                        
                        foreach (var combination in combinationList)
                        {

                            Combination combinationtemp = new Combination
                            {
                                conflict = 0,
                                indexCombi = new List<CourseIndex>()
                            };
                            combinationtemp.indexCombi.AddRange(combination.indexCombi);
                            combinationtemp.indexCombi.Add(index);
                            combinationlistTemp.Add(combinationtemp);
                        }
                        
                    }
                    
                }
                combinationList.Clear();
                combinationList.AddRange(combinationlistTemp);

            }



            combinationList = findConflict(combinationList);

            foreach (var combination in combinationList)
            {

                foreach (var item in combination.indexCombi)
                {
                    Console.Write(item.name + ", ");
                }
                Console.WriteLine(" Conflict: " + combination.conflict);

            }


            return combinationList;
        }

        public static List<Combination> findConflict(List<Combination> combinationList) {

            
            List<Combination> combinationListTemp = new List<Combination>();

            foreach (var combination in combinationList)
            {
                bool[,,] temp = new bool[14, 7, 14];
                Combination combinationTemp = new Combination();
                int a = 0;
               
                foreach (var courseIndex in combination.indexCombi)
                {
                    for (int j = 0; j < courseIndex.classes.GetLength(2); j++)
                    {
                        for (int y = 0; y < courseIndex.classes.GetLength(1); y++)
                        {
                            for (int x = 0; x < courseIndex.classes.GetLength(0); x++)
                            {
                                if (courseIndex.classes[x, y, j])
                                {
                                    if (temp[x, y, j]) a++;
                                    else temp[x, y, j] = true;
                                }
                            }


                        }

                    }
                }
                combinationTemp = combination;
                combinationTemp.conflict = a;
                combinationListTemp.Add(combinationTemp);
               
            }

           
           

            return combinationListTemp;
        }

    }
}
