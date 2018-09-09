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


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NTUTimetable_v1._0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CalendarView : Page
    {
        public CalendarView()
        {
            this.InitializeComponent();

            createbuttonsAsync();



            //Mycourse("fdhIAWFHIEHIFA", "CE5356", "SEP2", "LEC/THE", "LT2A", "BlueColor", 3, 5, 2);
        }



        public async Task createbuttonsAsync ()
        {
            currentweek myweek = new currentweek();
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

            List<Course_info> mycourseinfolist = new List<Course_info>();
            foreach (var item in mycoursearray)
            {
                mycourseinfolist.Add(item.ToObject<Course_info>());
            }

            int colornum = 1;

            foreach (var mycourse in mycourseinfolist)
            {
                
                JArray myclassarray = mycourse.ClassArray;
                foreach (var myclass in myclassarray)
                {
                    Class_info myclassinfo = myclass.ToObject<Class_info>();
                    if ( myclassinfo.WeekSpan.Contains(myweek.week))
                    {
                        Mycourse(mycourse.CourseIndex, mycourse.CourseCode, myclassinfo.group, myclassinfo.CourseType, myclassinfo.Venue, colornum.ToString(), myclassinfo.Row_Time, myclassinfo.Col_day, myclassinfo.RowSpan_Duration);
                        int setopacitycount = myclassinfo.RowSpan_Duration;
                        int setopacityrow = myclassinfo.Row_Time;
                        int setopacitycol = myclassinfo.Col_day;
                        while (setopacitycount > 0)
                        {
                            string bordername = "border_" + setopacityrow.ToString() + "_" + setopacitycol.ToString();
                            Object myborder = mygrid.FindName(bordername);
                            Border a = (Border)myborder;
                            a.Opacity = 0;
                            setopacityrow++;
                            setopacitycount--;
                            
                        }
                    }
                }
                colornum++;
            }
            


        }







        public void Mycourse(string index, string CourseID, string groupname, string coursetype, string venue, string buttoncolor, int rownum, int colnum, int rowspan)
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
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
            };

            TextBlock VenueText = new TextBlock
            {
                Text = venue,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
            };

            TextBlock CourseTypeText = new TextBlock
            {
                Text = coursetype,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
            };

            TextBlock GroupNameText = new TextBlock
            {
                Text = groupname,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))
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




        }


    }
}
