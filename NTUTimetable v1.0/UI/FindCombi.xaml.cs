using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NTUTimetable_v1._0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FindCombi : Page
    {

        List<string> courseName = new List<string>();
        List<Combination> allcombination;
        string courseListString;
        public FindCombi()
        {
            this.InitializeComponent();
        }

        private void ClearCourse(object sender, RoutedEventArgs e)
        {
            courseName.Clear();
            displayCourseEntered();
        }


        private void addCOurseButtonClick(object sender, RoutedEventArgs e)
        {
            if (courseIndexBox.Text != null)
            {

                string courseName = courseIndexBox.Text.ToUpper();
                if (!this.courseName.Contains(courseName))
                    this.courseName.Add(courseName);
                
            }
            displayCourseEntered();

           
        }
        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (courseIndexBox.Text != null)
                {

                    string courseName = courseIndexBox.Text.ToUpper();
                    if ( !this.courseName.Contains(courseName))
                        this.courseName.Add(courseName);

                }
                displayCourseEntered();
            }
        }


        public void displayCourseEntered() {
            courseListString = string.Join(", ", this.courseName.ToArray());
            enteredCoursecode.Text = "Entered Code: " + courseListString; //Display course code
        }

        private async void generateCombi(object sender, RoutedEventArgs e)
        {
            generatedCombination.Children.Clear();
            allcombination = null;
            if (courseName.Count < 4)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Less Than Four Courses? Do it yourself!",
                    Content = "I think it should be simple to find a suitable combi on STARTS Planner",
                    CloseButtonText = "Close",
                    PrimaryButtonText = "Complain",

                };
            }
            else {

                ProgressRing ring = new ProgressRing {
                    IsActive = true,
                    Height = 40,
                    Width = 40,
                    Visibility = Visibility.Visible,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                generatedCombination.Children.Add(ring);

                WebRequest webRequest = new WebRequest(courseName);

                foreach (var item in courseName)
                {
                    Debug.WriteLine(item);
                }
                
                allcombination = await webRequest.startCombinatnionParsingAsync();


                ring.Visibility = Visibility.Collapsed;

                if (allcombination.Count >= 30)
                {
                    var top10 = allcombination.Take(30);
                    generatedCombination.Children.Add(new TextBlock { Text = "We have found following combi for you:" });
                    foreach (var item in top10)
                    {
                        string indexcombi = "Index Combi: " + string.Join(", ", item.indexCombi.Select(p => $"{p.courseName}: {p.name}"));
                        indexcombi = indexcombi + "Conflict Hours: " + item.conflict.ToString();
                        generatedCombination.Children.Add(new TextBlock { Text = indexcombi, TextWrapping = TextWrapping.WrapWholeWords });

                    }

                }
                else if (allcombination.Count == 0) {
                    TextBlock textBlock = new TextBlock { Text = "Sorry, there is no possible way of arranging these courses without conflict" };
                }
                else
                {
                    generatedCombination.Children.Add(new TextBlock { Text = "We have found following combi for you:", FontWeight = FontWeights.Bold});
                    foreach (var item in allcombination)
                    {
                        string indexcombi = "Index Combi: " + string.Join(", ", item.indexCombi.Select(p => $"{p.courseName}: {p.name}"));
                        indexcombi = indexcombi + "Conflict Hours: " + item.conflict.ToString();
                        generatedCombination.Children.Add(new TextBlock { Text = indexcombi, TextWrapping = TextWrapping.WrapWholeWords });

                    }
                }




            }

        }
    }
}
