using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    }
}
