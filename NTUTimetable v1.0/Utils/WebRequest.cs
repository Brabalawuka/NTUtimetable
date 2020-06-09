using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using Newtonsoft.Json.Linq;

namespace NTUTimetable_v1._0
{
    public class WebRequest
    {
        List<Combination> combinationList;
        List<string> courseName = new List<string>();
        List<CourseInfo> myCourse = new List<CourseInfo>();
        
        string requestAddrPrefix = "https://wish.wis.ntu.edu.sg/webexe/owa/AUS_SCHEDULE.main_display1?acadsem=2020;1&r_search_type=F&r_subj_code=";
        string requestAddrSurfix = "&boption=Search&staff_access=false";
        public WebRequest(List<string> coursename) {
            this.courseName = coursename;
        }

        public WebRequest()
        {}

        public void addcpursename(string coursename) {
            this.courseName.Add(coursename.ToUpper());
        }

        public async Task<List<Combination>> startCombinatnionParsingAsync() {

            foreach (var coursenameitem in courseName)
            {
               
                string requestAddress = requestAddrPrefix + coursenameitem + requestAddrSurfix;
                await GetInfoAsync(requestAddress, coursenameitem);
            }

            combinationList = CourseUtils.findCombination(myCourse);
            combinationList = combinationList.OrderBy(o => o.conflict).ToList(); ;
            return combinationList;
        }



        public async Task<CourseInfo> startTimetableParsingAsync(string coursename, string indexname)
        {

            string requestAddress = requestAddrPrefix + coursename+ requestAddrSurfix;
            CourseInfo info = await GetInfoAsync(requestAddress, coursename, indexname);
            
     
            return info;
        }

        public async Task<CourseInfo> GetInfoAsync(string requestAddress, string courseName, string indexName) {
            bool matchedindex = false; //block selection

            //New course 
            
            CourseInfo newCourse = new CourseInfo(courseName);
            newCourse.courseIndex = indexName;
            newCourse.courseCode = courseName;
            JArray myclassinfoarray = new JArray();
            Debug.WriteLine(newCourse.courseCode + newCourse.courseIndex);
            //WebRequest Config
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(requestAddress);
            var cells = document.QuerySelectorAll("TR[BGCOLOR]"); // DataBlock
            Debug.WriteLine(cells.Count());

            foreach (var block in cells)
            {
                String blkinfo = block.TextContent;
                Debug.WriteLine(blkinfo);
                var lineInfo = blkinfo.Split("\n");
                if (lineInfo[1].ToUpper() == indexName)
                {
                    Debug.WriteLine("Find you");
                    matchedindex = true;
                    ClassInfo classInfo = new ClassInfo();
                    classInfo.classType = lineInfo[2];
                    Debug.WriteLine(classInfo.classType);
                    classInfo.group = lineInfo[3];
                    Debug.WriteLine(classInfo.group);
                    classInfo.colDay = CourseUtils.findColDay(lineInfo[4]);
                    var rowinfo = lineInfo[5].Split("-");
                    classInfo.rowTime = CourseUtils.findRowTime(rowinfo[0]);
                    classInfo.rowSpanDuration = CourseUtils.findRowSpanDuration(rowinfo[0], rowinfo[1]);
                    classInfo.venue = lineInfo[6];
                    classInfo.weekSpan = CourseUtils.findWeekSpan(lineInfo[7]);
                    JObject myobject = (JObject)JToken.FromObject(classInfo);
                    myclassinfoarray.Add(myobject);

                }
                else if (lineInfo[1].Length != 0 && matchedindex)
                {
                    Debug.WriteLine("Other Course");
                    newCourse.ClassArray = myclassinfoarray;
                    return newCourse;
                }
                    

                else if (lineInfo[1].Length == 0 && matchedindex) {

                    Debug.WriteLine("Same Course ");

                    ClassInfo classInfo = new ClassInfo();
                    classInfo.classType = lineInfo[2];
                    classInfo.group = lineInfo[3];
                    classInfo.colDay = CourseUtils.findColDay(lineInfo[4]);
                    var rowinfo = lineInfo[5].Split("-");
                    classInfo.rowTime = CourseUtils.findRowTime(rowinfo[0]);
                    classInfo.rowSpanDuration = CourseUtils.findRowSpanDuration(rowinfo[0], rowinfo[1]);
                    classInfo.venue = lineInfo[6];
                    classInfo.weekSpan = CourseUtils.findWeekSpan(lineInfo[7]);
                    JObject myobject = (JObject)JToken.FromObject(classInfo);
                    myclassinfoarray.Add(myobject);

                }

            }
            newCourse.ClassArray = myclassinfoarray;
            Debug.WriteLine(myclassinfoarray.Count());
            return newCourse;

        }

        public async Task GetInfoAsync(string requestAddress, string courseName)

        {
           
            CourseInfo newCourse = new CourseInfo(courseName);

            //WebRequest Config
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(requestAddress);
            
            var cells = document.QuerySelectorAll("TR[BGCOLOR]"); // DataBlock
            Debug.WriteLine(cells.Count());
            CourseIndex courseIndex = null;


            foreach (var block in cells)
            {

                String blkinfo = block.TextContent;
                //Debug.WriteLine(blkinfo);
                var lineInfo = blkinfo.Split("\n");
                if (lineInfo[1].Length == 5)
                {
                    if (courseIndex != null)
                    {
                        newCourse.allIndex.Add(courseIndex);

                        //test
                        //for (int i = 0; i < courseIndex.classes.GetLength(2); i++)
                        //{
                        //    for (int y = 0; y < courseIndex.classes.GetLength(1); y++)
                        //    {
                        //        for (int x = 0; x < courseIndex.classes.GetLength(0); x++)
                        //        {
                        //            Debug.Write((courseIndex.classes[x, y, i] == true) ? "True" : "----");
                        //        }
                        //        Debug.WriteLine(" ");

                        //    }
                        //    Debug.WriteLine(" ");
                        //}
                    }

                    courseIndex = new CourseIndex(lineInfo[1], courseName);
                };

                var timeStartEnd = lineInfo[5].Split('-');
                List<int> WeekSpan = CourseUtils.findWeekSpan(lineInfo[7]);
                int day = CourseUtils.findColDay(lineInfo[4]);
                List<int> timeDuration = CourseUtils.findTimeDuration(timeStartEnd[0], timeStartEnd[1]);

                foreach (var week in WeekSpan)
                {
                    foreach (var time in timeDuration)
                    {
                        //Debug.Write(time.ToString() + " " + day.ToString() + " " + week.ToString() + "  ---");

                        courseIndex.classes[time, day, week] = true;
                    }
                }
                




            }
            newCourse.allIndex.Add(courseIndex);

            myCourse.Add(newCourse);
            Debug.WriteLine("==================================");


            return;
        }
    }
}
