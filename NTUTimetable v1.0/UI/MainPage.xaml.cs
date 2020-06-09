using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace NTUTimetable_v1._0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CurrentWeek myweek = new CurrentWeek();


        public void setWeek(int week) {
            myweek.week = week;
        }

        public int getWeek() {
            return myweek.week;
        }
        private async void feedbackDialog(string args)
        {
            

            ContentDialog dialog = new ContentDialog
            {
                Title = "Nice to meet you",
                Content = args,
                CloseButtonText = "Close",
                PrimaryButtonText = "Email",
                SecondaryButtonText = "Review",
                
               
                
                

            };

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var emailUri = new Uri("mailto:lyuz0001@e.ntu.edu.sg");
                await Windows.System.Launcher.LaunchUriAsync(emailUri);

            }
            else if (result == ContentDialogResult.Secondary)
            {
                var reviewUri = new Uri("ms-windows-store://review/?ProductId=9P98CRS9Z1BJ");
                //var githubUri = new Uri("https://github.com/Brabalawuka/NTUtimetable/issues");
                await Windows.System.Launcher.LaunchUriAsync(reviewUri);
            } 

        }

        public MainPage()
        {
            this.InitializeComponent();
            //curretnweek method


            CurrentWeek.Content = "AY2020 S1 Week " + myweek.week.ToString();




            MainPageFrame.Navigate(typeof(CalendarView), myweek);
        }

        private void Menu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                MainPageFrame.Navigate(typeof(Setting), this);
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                switch (item.Tag.ToString())
                {
                    case "CalendarView":
                        MainPageFrame.Navigate(typeof(CalendarView), myweek);
                        break;
                    case "ListView":
                        MainPageFrame.Navigate(typeof(ListView));
                        break;
                    case "AddCourse":
                        MainPageFrame.Navigate(typeof(AddCourse));
                        break;
                    case "FindCombi":
                        MainPageFrame.Navigate(typeof(FindCombi));
                        break;
                    


                }
            }
        }

        private void FeedbackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            feedbackDialog("Please email to lyuz0001@e.ntu.edu.sg or go to Store Page for review and suggestion!");

        }




        


        

        
    }
}
