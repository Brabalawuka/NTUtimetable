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
using Windows.UI.Text;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Resources;
using Windows.Storage;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using Windows.UI;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NTUTimetable_v1._0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CalendarView : Page
    {
        CurrentWeek currentWeek;
        List<CourseInfo> mycourseinfolist;
        public CalendarView()
        {
            this.InitializeComponent();

            



            //Mycourse("fdhIAWFHIEHIFA", "CE5356", "SEP2", "LEC/THE", "LT2A", "BlueColor", 3, 5, 2);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            currentWeek = (CurrentWeek)e.Parameter;
            currentWeekSlider.Value = currentWeek.week;
            CreateButtonsAsync(currentWeek);
        }



        private async Task CreateButtonsAsync(CurrentWeek myweek)
        {
            
            StorageFile storagefile;
            
           
            try
            {
                storagefile = await ApplicationData.Current.LocalFolder.GetFileAsync("mycourse.json");
                
            }
            catch (FileNotFoundException)
            {

                storagefile = await ApplicationData.Current.LocalFolder.CreateFileAsync("mycourse.json");
            }
            ///await storagefile.DeleteAsync();
            string mycourses = await FileIO.ReadTextAsync(storagefile);
            JArray mycoursearray = JArray.Parse(mycourses);

            

            mycourseinfolist = new List<CourseInfo>();
            foreach (var item in mycoursearray)
            {
                mycourseinfolist.Add(item.ToObject<CourseInfo>());
            }



            int colornum = 1;

            foreach (var mycourse in mycourseinfolist)
            {
                //Debug.WriteLine("I am here");
                
                JArray myclassarray = mycourse.ClassArray;
                foreach (var myclass in myclassarray)
                {
                    ClassInfo myclassinfo = myclass.ToObject<ClassInfo>();
                    if ( myclassinfo.weekSpan.Contains(myweek.week))
                    {
                        //Debug.WriteLine("I am here2");
                        Mycourse(mycourse.courseIndex, mycourse.courseCode, myclassinfo.group, myclassinfo.classType, myclassinfo.venue, colornum.ToString(), myclassinfo.rowTime, myclassinfo.colDay, myclassinfo.rowSpanDuration);
                        int setopacitycount = myclassinfo.rowSpanDuration;
                        int setopacityrow = myclassinfo.rowTime;
                        int setopacitycol = myclassinfo.colDay;
                        while (setopacitycount > 0)
                        {
                            
                            string bordername = "border_" + setopacityrow.ToString() + "_" + setopacitycol.ToString();
                            //Debug.WriteLine(bordername);
                            Object myborder = mygrid.FindName(bordername);
                            Border a = (Border)myborder;
                            a.Opacity = 0;
                            setopacityrow++;
                            setopacitycount--;
                            
                        }
                    }
                }
                if (colornum < 9)
                    colornum++;
                else
                    colornum = 1;
            }
            


        }







        private void Mycourse(string index, string CourseID, string groupname, string coursetype, string venue, string buttoncolor, int rownum, int colnum, int rowspan)
        {

            StackPanel ButtonStackPenal = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,

            };

            TextBlock CourseIDText = new TextBlock
            {
                Text = CourseID,
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(-2),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
            };

            TextBlock VenueText = new TextBlock
            {
                Text = venue,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
            };

            TextBlock CourseTypeText = new TextBlock
            {
                Text = coursetype,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
            };

            TextBlock GroupNameText = new TextBlock
            {
                Text = groupname,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
            };
            ButtonStackPenal.Children.Add(CourseIDText);
            ButtonStackPenal.Children.Add(CourseTypeText);
            ButtonStackPenal.Children.Add(GroupNameText);
            ButtonStackPenal.Children.Add(VenueText);


            




            Button mybutton = new Button
            {
                Name = CourseID,

                Content = ButtonStackPenal,
                BorderThickness = new Thickness(1.5),
                BorderBrush = (RevealBorderBrush)Resources["MyLightBorderBrush"],
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = (SolidColorBrush)Resources[buttoncolor],
                
            };
            Grid.SetColumn(mybutton, colnum);
            Grid.SetRow(mybutton, rownum);
            Grid.SetRowSpan(mybutton, rowspan);
            mygrid.Children.Add(mybutton);
            

            ToolTip toolTip = new ToolTip();
            toolTip.Content = mycourseinfolist.FirstOrDefault(course => course.courseCode == CourseID).ExamInfo;
            ToolTipService.SetToolTip(mybutton, toolTip);




        }
       

        private void ChangeViewingWeek_Click(object sender, RoutedEventArgs e)
        {
            currentWeek.week = (int)currentWeekSlider.Value;
            this.Frame.Navigate(typeof(CalendarView), currentWeek);

        }
    }
}
