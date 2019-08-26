using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace NTUTimetable_v1._0
{
    public class CourseUtils
    {
        

        
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
        public static int FindRow_Time(string mystarttime)
        {
            int starttime = int.Parse(mystarttime);
            if (starttime % 100 == 0)
            {
                return ((starttime - 800) / 50);
            }
            else
            {
                return (((starttime - 30) - 800) / 50 + 1);
            }
        }

        public static int FindRowSpan_Duration(string mystarttime, string myendtime)
        {
            int endrow = FindRow_Time(myendtime);
            int startrow = FindRow_Time(mystarttime);
            return endrow - startrow;

        }


        public static List<int> findTimeDuration(string startTimeS, string endTimeS)
        {
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

        public static List<Combination> findCombination(List<CourseInfo> courseList)
        {
            List<Combination> combinationList = new List<Combination>();
            List<Combination> combinationlistTemp = new List<Combination>();


            foreach (var course in courseList)
            {
                combinationlistTemp.Clear();
                foreach (var index in course.allIndex)
                {

                    //Debug.WriteLine("CombinationList Count:" + course.allIndex.Count);
                    //Debug.WriteLine("CurrentIndex" + course.allIndex.IndexOf(index));

                    if (courseList.IndexOf(course) == 0)
                    {
                        Combination combination = new Combination
                        {
                            conflict = 0,
                            indexCombi = new List<CourseIndex> { index }
                        };

                        combinationlistTemp.Add(combination);
                        //Debug.WriteLine(index.name + "1st index ");
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
                    Debug.Write(item.name + ", ");
                }
                Debug.WriteLine(" Conflict: " + combination.conflict);

            }


            return combinationList;
        }


        public static List<Combination> findConflict(List<Combination> combinationList)
        {


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
                                if (a >= 5) break;
                            }
                            if (a >= 5) break;

                        }
                        if (a >= 5) break;
                    }
                }
                if (a < 5) {
                    combinationTemp = combination;
                    combinationTemp.conflict = a;
                    combinationListTemp.Add(combinationTemp);
                }
                

            }




            return combinationListTemp;
        }



    }
}
