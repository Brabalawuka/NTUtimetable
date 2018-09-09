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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NTUTimetable_v1._0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    
    public sealed partial class Addcourse : Page
    {


        public Addcourse()
        {
            this.InitializeComponent();

            
            
        }

        

        private async void SubmitButton_ClickAsync(object sender, RoutedEventArgs e)
        {



            //open the current coursefile
            
            StorageFile storagefile =await ApplicationData.Current.LocalFolder.GetFileAsync("mycourse.json");
            
            
            //courses are stored in a Copurse_info object format, in a json array
            //var mycoursescurrent = JsonConvert.DeserializeObject<List<Course_info>>(content);
            JArray mycourseinfoarray = new JArray();

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

                    var a = mycourseinfostring.Split("\r");
                    var b = new List<string>();

                    Exception exception = new Exception();

                    for (int i = 11; i < a.Length; i++)
                    {
                        var temp = a[i].Split("\t");
                        if (temp.Length >= 6 && temp[temp.Length - 1][0] == 'T')
                            b.Add(a[i]);

                    }
                    if (b.Count <= 18)
                        throw new FormatException();
                    List<Course_info> mycourseinfolist = new List<Course_info>();

                    for (int i = 0; i < b.Count; i++)
                    {
                        var temp = b[i].Split("\t");
                        if (temp.Length == 15)
                        {
                            Course_info mycourseinfo = new Course_info();
                            JArray myclassinfoarray = new JArray();
                            mycourseinfo.CourseCode = temp[0];
                            mycourseinfo.CourseIndex = temp[6];
                            Class_info myclassinfo = new Class_info();
                            myclassinfo.CourseType = temp[9];
                            myclassinfo.group = temp[10];
                            myclassinfo.Venue = temp[13];
                            myclassinfo.WeekSpan = findcourse.FindWeekSpan(temp[14]);
                            var temp2 = temp[12].Split("-");
                            myclassinfo.Row_Time = findcourse.FindRow_Time(temp2[0]);
                            myclassinfo.RowSpan_Duration = findcourse.FindRowSpan_Duration(temp2[0], temp2[1]);
                            myclassinfo.Col_day = findcourse.FindCol_day(temp[11]);
                            JObject myobject = (JObject)JToken.FromObject(myclassinfo);
                            myclassinfoarray.Add(myobject);


                            for (int k = i + 1; k < b.Count; k++)
                            {
                                var temp3 = b[k].Split("\t");
                                if (temp3.Length == 6)
                                {
                                    Class_info myclassinfo2 = new Class_info();

                                    myclassinfo.CourseType = temp3[0];
                                    myclassinfo.group = temp3[1];
                                    myclassinfo.Venue = temp3[4];
                                    myclassinfo.WeekSpan = findcourse.FindWeekSpan(temp3[5]);
                                    var temp4 = temp3[3].Split("-");
                                    myclassinfo.Row_Time = findcourse.FindRow_Time(temp4[0]);
                                    myclassinfo.RowSpan_Duration = findcourse.FindRowSpan_Duration(temp4[0], temp4[1]);
                                    myclassinfo.Col_day = findcourse.FindCol_day(temp3[2]);
                                    JObject myobject2 = (JObject)JToken.FromObject(myclassinfo);
                                    myclassinfoarray.Add(myobject2);
                                }
                                else
                                    break;
                            }

                            mycourseinfo.ClassArray = myclassinfoarray;
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
    }
}
