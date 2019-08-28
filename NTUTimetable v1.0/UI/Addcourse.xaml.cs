using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NTUTimetable_v1._0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 
    

    
    public sealed partial class AddCourse : Page
    {
        public static string EXAM_INFO_URI = "ms-appx:///Assets/examinfo.json";
        public static string MYCOURSE_INFO = "mycourse.json";

        List<string> courseCode = new List<string>();
        List<string> courseIndex = new List<string>();
        StorageFile examFile, myCourseJsonFile;
        List<ExamInfo> examInfoList = new List<ExamInfo>();
        List<CourseInfo> myCourseInfoList = new List<CourseInfo>();
        JArray myCourseInfoArray = new JArray();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
        }


        
        public AddCourse()
        {
            this.InitializeComponent();

            readFiles();
        }


        public async Task readFiles() {
            //Debug.WriteLine("Read exam file");
            try
            {
                examFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(EXAM_INFO_URI));
                //open the current coursefile
                myCourseJsonFile = await ApplicationData.Current.LocalFolder.GetFileAsync(MYCOURSE_INFO);
                
                string mycourses = await FileIO.ReadTextAsync(myCourseJsonFile);
                if (mycourses.Length >= 10)
                    myCourseInfoArray = JArray.Parse(mycourses);
                
            }
            catch (Exception ex)
            {
                myCourseJsonFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(MYCOURSE_INFO);
                Debug.WriteLine(ex.ToString());
            }
            
            
            string exams = await FileIO.ReadTextAsync(examFile);
            JArray examsArray = JArray.Parse(exams);
            //Debug.WriteLine("Read exam file");
            foreach (var item in examsArray)
            {
                examInfoList.Add(item.ToObject<ExamInfo>());
               // Debug.WriteLine(item.ToObject<ExamInfo>().Course);
            }
            return;
        }

        

        private async void generateTimetablebyCopyPaste(object sender, RoutedEventArgs e)
        {




            //courses are stored in a Copurse_info object format, in a json array

            //JArray mycourseinfoarray = new JArray();

            if (string.IsNullOrWhiteSpace(myCourseInfoTextBox.Text) || myCourseInfoTextBox.Text[0] != 'S')
            {
                generateDialog(
                        "Parsing Failed!",
                        "Please ensure you have followed the instruction and do not edit the pasted content!",
                        "Try Again");
            }

            else
            {
                try
                {
                    string myCourseInfoString = myCourseInfoTextBox.Text;
                    myCourseInfoString.Trim();

                    var rawLines = myCourseInfoString.Split("\r");
                    var effectiveLines = new List<string>();

                    Exception exception = new Exception();

                    for (int i = 11; i < rawLines.Length; i++)
                    {
                        var Lines = rawLines[i].Split("\t");
                        if (Lines.Length >= 6 && Lines[Lines.Length - 1][0] == 'T') //Filter out lines representing mothing or online course
                            effectiveLines.Add(rawLines[i]);

                    }
                    if (effectiveLines.Count <= 0)
                        throw new FormatException();

                    //New List of course

                    //List<CourseInfo> mycourseinfolist = new List<CourseInfo>();

                    for (int i = 0; i < effectiveLines.Count; i++)
                    {
                        var temp = effectiveLines[i].Split("\t");
                        if (temp.Length == 15)
                        {
                            //New course
                            CourseInfo myCourseInfo = new CourseInfo();
                            JArray myClassInfoArray = new JArray();
                            myCourseInfo.courseCode = temp[0];
                            myCourseInfo.courseIndex = temp[6];

                            //Add class info in the course info line
                            ClassInfo myClassInfo = new ClassInfo();
                            myClassInfo.classType = temp[9];
                            myClassInfo.group = temp[10];
                            myClassInfo.venue = temp[13];
                            myClassInfo.weekSpan = CourseUtils.findWeekSpan(temp[14]);
                            var temp2 = temp[12].Split("-");
                            myClassInfo.rowTime = CourseUtils.findRowTime(temp2[0]);
                            myClassInfo.rowSpanDuration = CourseUtils.findRowSpanDuration(temp2[0], temp2[1]);
                            myClassInfo.colDay = CourseUtils.findColDay(temp[11]);
                            JObject myClassInfoObject = (JObject)JToken.FromObject(myClassInfo);
                            myClassInfoArray.Add(myClassInfoObject);

                            //Add Class Info in other lines

                            for (int k = i + 1; k < effectiveLines.Count; k++)
                            {
                                var otherClassInfoStringArray = effectiveLines[k].Split("\t");
                                if (otherClassInfoStringArray.Length == 6)
                                {
                                    ClassInfo myClassInfo2 = new ClassInfo();

                                    myClassInfo2.classType = otherClassInfoStringArray[0];
                                    myClassInfo2.group = otherClassInfoStringArray[1];
                                    myClassInfo2.venue = otherClassInfoStringArray[4];
                                    myClassInfo2.weekSpan = CourseUtils.findWeekSpan(otherClassInfoStringArray[5]);
                                    var temp4 = otherClassInfoStringArray[3].Split("-");
                                    myClassInfo2.rowTime = CourseUtils.findRowTime(temp4[0]);
                                    myClassInfo2.rowSpanDuration = CourseUtils.findRowSpanDuration(temp4[0], temp4[1]);
                                    myClassInfo2.colDay = CourseUtils.findColDay(otherClassInfoStringArray[2]);
                                    JObject myClassInfoObject2 = (JObject)JToken.FromObject(myClassInfo2);
                                    myClassInfoArray.Add(myClassInfoObject2);
                                }
                                else
                                    break;
                            }

                            myCourseInfo.ClassArray = myClassInfoArray;

                            //Add Exam Info 

                            var matchedexam = examInfoList.FirstOrDefault(match => (match.Course.ToUpper() == myCourseInfo.courseCode.ToUpper()));
                            if (matchedexam != null)
                            {
                                myCourseInfo.ExamInfo = "FINAL EXAM: " + matchedexam.Date + " " + matchedexam.Day + " " + matchedexam.Time + " " + matchedexam.Duration + "h";
                            }
                            else {
                                myCourseInfo.ExamInfo = "No Exam Infomation Found";
                            }

                            myCourseInfoList.Add(myCourseInfo);



                        }


                    }
                    //Form Json File
                    foreach (var item in myCourseInfoList)
                    {
                        JObject mycourse = (JObject)JToken.FromObject(item);
                        myCourseInfoArray.Add(mycourse);
                    }

                    string everythingString = myCourseInfoArray.ToString();
                    myCourseInfoTextBox.Text = "SUCCESS";
                    await FileIO.WriteTextAsync(myCourseJsonFile, everythingString);


                    generateDialog(
                        "Parsing Successful!",
                        "Go back to calendar view and check ur timetable for current week",
                        "OK");

                }
                catch (FormatException)
                {
                    generateDialog(
                        "Parsing Failed!",
                        "Please ensure you have followed the instruction and do not edit the pasted content!",
                        "Try Again");

                }
                



            }

            
        }


        private async void addCourseButtonClick(object sender, RoutedEventArgs e)
        {


            if (courseIndexBox.Text != null)
            {

                var courseNameplusIndex = courseIndexBox.Text.Split("/");
                if (courseNameplusIndex.Length != 2)
                {
                    displayCourseEntered(false);
                    return;
                    
                }
                if (!this.courseCode.Contains(courseNameplusIndex[0].ToUpper()) && !this.courseIndex.Contains(courseNameplusIndex[1].ToUpper()))
                {
                    this.courseCode.Add(courseNameplusIndex[0].ToUpper());
                    this.courseIndex.Add(courseNameplusIndex[1].ToUpper());
                    displayCourseEntered(true);
                    clearButton.IsEnabled = false;
                    addCourseProgress.Visibility = Visibility.Visible;


                    WebRequest webRequest = new WebRequest(courseCode);
                    CourseInfo courseinfo = await webRequest.startTimetableParsingAsync(courseNameplusIndex[0].ToUpper(), courseNameplusIndex[1].ToUpper());
                    var matchedexam = examInfoList.FirstOrDefault(match => (match.Course.ToUpper() == courseinfo.courseCode.ToUpper()));
                    if (matchedexam != null)
                    {
                        courseinfo.ExamInfo = "FINAL EXAM: " + matchedexam.Date + " " + matchedexam.Day + " " + matchedexam.Time + " " + matchedexam.Duration + "h";
                    }
                    myCourseInfoList.Add(courseinfo);

                    addCourseProgress.Visibility = Visibility.Collapsed;
                    clearButton.IsEnabled = true;

                }
                    
            }
            

        }

        public void displayCourseEntered(bool format)
        {
            if (format)
            {
                string courseListString = string.Join(", ", this.courseCode.ToArray());
                string courseIndexString = string.Join(", ", this.courseIndex.ToArray());
                enteredCoursecode.Text = "Entered Code: " + courseListString + "  Entered Index: " + courseIndexString; //Display course code
            }
            else
            {
                enteredCoursecode.Text = "Wrong Input Format";
            }
           
        }


        private void ClearCourse(object sender, RoutedEventArgs e)
        {
            courseCode.Clear();
            courseIndex.Clear();
            myCourseInfoList.Clear();
            displayCourseEntered(true);
        }

        private async void generateTimetable(object sender, RoutedEventArgs e)
        {
            if (myCourseInfoList.Count == 0) {
                return;
            }

            StorageFile storagefile = await ApplicationData.Current.LocalFolder.GetFileAsync("mycourse.json");

            foreach (var item in myCourseInfoList)
            {
                JObject mycourse = (JObject)JToken.FromObject(item);
                myCourseInfoArray.Add(mycourse);
            }

            string aaa = myCourseInfoArray.ToString();
            myCourseInfoTextBox.Text = "SUCCESS";
            await FileIO.WriteTextAsync(storagefile, aaa);

            ContentDialog mydialog2 = new ContentDialog();
            mydialog2.Title = "Parsing Successful!";
            mydialog2.Content = "Go back to calendar view and check ur timetable for current week";
            mydialog2.CloseButtonText = "OK";
            await mydialog2.ShowAsync();
        }


        private async void generateDialog(string title, string content, string closeButtonText) {

            ContentDialog mydialog = new ContentDialog();
            mydialog.Title = title;
            mydialog.Content = content;
            mydialog.CloseButtonText = closeButtonText;
            await mydialog.ShowAsync();

        }
    }
}
