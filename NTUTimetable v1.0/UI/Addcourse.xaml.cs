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
    

    
    public sealed partial class Addcourse : Page
    {
        List<string> coursecode = new List<string>();
        List<string> courseindex = new List<string>();
        StorageFile examfile;
        List<ExamInfo> examInfoList = new List<ExamInfo>();
        List<CourseInfo> mycourseinfolist = new List<CourseInfo>();
        JArray mycourseinfoarray = new JArray();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            
        }


        
        public Addcourse()
        {
            this.InitializeComponent();

            readExamFile();
        }


        public async Task readExamFile() {
            //Debug.WriteLine("Read exam file");
            try
            {
                examfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/examinfo.json"));
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
            }
            
            
            string exams = await FileIO.ReadTextAsync(examfile);
            JArray examsarray = JArray.Parse(exams);
            //Debug.WriteLine("Read exam file");
            foreach (var item in examsarray)
            {
                examInfoList.Add(item.ToObject<ExamInfo>());
               // Debug.WriteLine(item.ToObject<ExamInfo>().Course);
            }
            return;
        }

        

        private async void SubmitButton_ClickAsync(object sender, RoutedEventArgs e)
        {



            //open the current coursefile
            
            StorageFile storagefile =await ApplicationData.Current.LocalFolder.GetFileAsync("mycourse.json");
            


            //courses are stored in a Copurse_info object format, in a json array
            //var mycoursescurrent = JsonConvert.DeserializeObject<List<Course_info>>(content);
            //JArray mycourseinfoarray = new JArray();

            if (string.IsNullOrWhiteSpace(mycourseinfotextbox.Text) || mycourseinfotextbox.Text[0] != 'S')
            {
                ContentDialog mydialog = new ContentDialog();
                mydialog.Title = "Parsing Failed!";
                mydialog.Content = "Please ensure you have followed the instruction and do not edit the pasted content!";
                mydialog.CloseButtonText = "Try Again";
                await mydialog.ShowAsync();
                
            }

            else
            {
                try
                {
                    string mycourseinfostring = mycourseinfotextbox.Text;
                    mycourseinfostring.Trim();

                    var rawLines = mycourseinfostring.Split("\r");
                    var effectiveLines = new List<string>();

                    Exception exception = new Exception();

                    for (int i = 11; i < rawLines.Length; i++)
                    {
                        var Lines = rawLines[i].Split("\t");
                        if (Lines.Length >= 6 && Lines[Lines.Length - 1][0] == 'T')
                            effectiveLines.Add(rawLines[i]);

                    }
                    if (effectiveLines.Count <= 18)
                        throw new FormatException();

                    //New List of course

                    //List<CourseInfo> mycourseinfolist = new List<CourseInfo>();

                    for (int i = 0; i < effectiveLines.Count; i++)
                    {
                        var temp = effectiveLines[i].Split("\t");
                        if (temp.Length == 15)
                        {
                            //New course
                            CourseInfo mycourseinfo = new CourseInfo();
                            JArray myclassinfoarray = new JArray();
                            mycourseinfo.CourseCode = temp[0];
                            mycourseinfo.CourseIndex = temp[6];

                            //Add class info in the course info line
                            ClassInfo myclassinfo = new ClassInfo();
                            myclassinfo.ClassType = temp[9];
                            myclassinfo.group = temp[10];
                            myclassinfo.Venue = temp[13];
                            myclassinfo.WeekSpan = CourseUtils.FindWeekSpan(temp[14]);
                            var temp2 = temp[12].Split("-");
                            myclassinfo.Row_Time = CourseUtils.FindRow_Time(temp2[0]);
                            myclassinfo.RowSpan_Duration = CourseUtils.FindRowSpan_Duration(temp2[0], temp2[1]);
                            myclassinfo.Col_day = CourseUtils.FindCol_day(temp[11]);
                            JObject myobject = (JObject)JToken.FromObject(myclassinfo);
                            myclassinfoarray.Add(myobject);

                            //Add Class Info in other lines

                            for (int k = i + 1; k < effectiveLines.Count; k++)
                            {
                                var temp3 = effectiveLines[k].Split("\t");
                                if (temp3.Length == 6)
                                {
                                    ClassInfo myclassinfo2 = new ClassInfo();

                                    myclassinfo.ClassType = temp3[0];
                                    myclassinfo.group = temp3[1];
                                    myclassinfo.Venue = temp3[4];
                                    myclassinfo.WeekSpan = CourseUtils.FindWeekSpan(temp3[5]);
                                    var temp4 = temp3[3].Split("-");
                                    myclassinfo.Row_Time = CourseUtils.FindRow_Time(temp4[0]);
                                    myclassinfo.RowSpan_Duration = CourseUtils.FindRowSpan_Duration(temp4[0], temp4[1]);
                                    myclassinfo.Col_day = CourseUtils.FindCol_day(temp3[2]);
                                    JObject myobject2 = (JObject)JToken.FromObject(myclassinfo);
                                    myclassinfoarray.Add(myobject2);
                                }
                                else
                                    break;
                            }

                            mycourseinfo.ClassArray = myclassinfoarray;

                            //Add Exam Info 

                            var matchedexam = examInfoList.FirstOrDefault(match => (match.Course.ToUpper() == mycourseinfo.CourseCode.ToUpper()));
                            if (matchedexam != null) {
                                mycourseinfo.ExamInfo = "FINAL EXAM: "+ matchedexam.Date + " " + matchedexam.Day + " " + matchedexam.Time + " " + matchedexam.Duration + "h";
                            }

                            mycourseinfolist.Add(mycourseinfo);



                        }


                    }

                    foreach (var item in mycourseinfolist)
                    {
                        JObject mycourse = (JObject)JToken.FromObject(item);
                        mycourseinfoarray.Add(mycourse);
                    }

                    string aaa = mycourseinfoarray.ToString();
                    mycourseinfotextbox.Text = "SUCCESS";
                    await FileIO.WriteTextAsync(storagefile, aaa);

                    ContentDialog mydialog2 = new ContentDialog();
                    mydialog2.Title = "Parsing Successful!";
                    mydialog2.Content = "Go back to calendar view and check ur timetable for current week";
                    mydialog2.CloseButtonText = "OK";
                    await mydialog2.ShowAsync();
                }
                catch (FormatException)
                {
                    ContentDialog mydialog = new ContentDialog();
                    mydialog.Title = "Parsing Failed!";
                    mydialog.Content = "Please ensure you have followed the instruction and do not edit the pasted content!";
                    mydialog.CloseButtonText = "Try Again";
                    await mydialog.ShowAsync();
                }
                



            }



            
        }

        private async void addCOurseButtonClick(object sender, RoutedEventArgs e)
        {


            if (courseIndexBox.Text != null)
            {

                var courseNameplusIndex = courseIndexBox.Text.Split("/");
                if (courseNameplusIndex.Length != 2)
                {
                    displayCourseEntered(false);
                    return;
                    
                }
                if (!this.coursecode.Contains(courseNameplusIndex[0].ToUpper()) && !this.courseindex.Contains(courseNameplusIndex[1].ToUpper()))
                {
                    this.coursecode.Add(courseNameplusIndex[0].ToUpper());
                    this.courseindex.Add(courseNameplusIndex[1].ToUpper());
                    displayCourseEntered(true);
                    clearButton.IsEnabled = false;

                    ProgressRing ring = new ProgressRing
                    {
                        IsActive = true,
                        Margin = new Thickness(10, 0, 0, 0),
                        Height = 40,
                        Width = 40,
                        Visibility = Visibility.Visible,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    addingcoursepenal.Children.Add(ring);


                    WebRequest webRequest = new WebRequest(coursecode);
                    CourseInfo courseinfo = await webRequest.startTimetableParsingAsync(courseNameplusIndex[0].ToUpper(), courseNameplusIndex[1].ToUpper());
                    var matchedexam = examInfoList.FirstOrDefault(match => (match.Course.ToUpper() == courseinfo.CourseCode.ToUpper()));
                    if (matchedexam != null)
                    {
                        courseinfo.ExamInfo = "FINAL EXAM: " + matchedexam.Date + " " + matchedexam.Day + " " + matchedexam.Time + " " + matchedexam.Duration + "h";
                    }
                    mycourseinfolist.Add(courseinfo);

                    ring.Visibility = Visibility.Collapsed;
                    clearButton.IsEnabled = true;

                }
                    
            }
            

        }

        public void displayCourseEntered(bool format)
        {
            if (format)
            {
                string courseListString = string.Join(", ", this.coursecode.ToArray());
                string courseIndexString = string.Join(", ", this.courseindex.ToArray());
                enteredCoursecode.Text = "Entered Code: " + courseListString + "  Entered Index: " + courseIndexString; //Display course code
            }
            else
            {
                enteredCoursecode.Text = "Wrong Input Format";
            }
           
        }


        private void ClearCourse(object sender, RoutedEventArgs e)
        {
            coursecode.Clear();
            courseindex.Clear();
            mycourseinfolist.Clear();
            displayCourseEntered(true);
        }

        private async void generateTimetable(object sender, RoutedEventArgs e)
        {
            if (mycourseinfolist.Count == 0) {
                return;
            }

            StorageFile storagefile = await ApplicationData.Current.LocalFolder.GetFileAsync("mycourse.json");

            foreach (var item in mycourseinfolist)
            {
                JObject mycourse = (JObject)JToken.FromObject(item);
                mycourseinfoarray.Add(mycourse);
            }

            string aaa = mycourseinfoarray.ToString();
            mycourseinfotextbox.Text = "SUCCESS";
            await FileIO.WriteTextAsync(storagefile, aaa);

            ContentDialog mydialog2 = new ContentDialog();
            mydialog2.Title = "Parsing Successful!";
            mydialog2.Content = "Go back to calendar view and check ur timetable for current week";
            mydialog2.CloseButtonText = "OK";
            await mydialog2.ShowAsync();
        }
    }
}
