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

        private async void feedbackDialog(string args)
        {
            

            ContentDialog dialog = new ContentDialog
            {
                Title = "Nice to meet you",
                Content = args,
                CloseButtonText = "Close",
                PrimaryButtonText = "Email",
                SecondaryButtonText = "GitHub"
                

            };

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var emailUri = new Uri("mailto:lyuz0001@e.ntu.edu.sg");
                await Windows.System.Launcher.LaunchUriAsync(emailUri);

            }
            else if (result == ContentDialogResult.Secondary)
            {
                var githubUri = new Uri("https://github.com/Brabalawuka/NTUtimetable/issues");
                await Windows.System.Launcher.LaunchUriAsync(githubUri);
            }

        }

        public MainPage()
        {
            this.InitializeComponent();
            //curretnweek method

            CurrentWeek myweek = new CurrentWeek();
            CurrentWeek.Content ="We are in Week "+ myweek.week.ToString()+ " now";
          



            MainPageFrame.Navigate(typeof(CalendarView));
        }

        private void Menu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                MainPageFrame.Navigate(typeof(Setting));
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                switch (item.Tag.ToString())
                {
                    case "CalendarView":
                        MainPageFrame.Navigate(typeof(CalendarView));
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
            feedbackDialog("Please email to lyuz0001@e.ntu.edu.sg or go to Github Page for report and suggestion!");

        }


        

        
    }
}
