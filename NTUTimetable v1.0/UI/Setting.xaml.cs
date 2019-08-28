using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class Setting : Page


    {
       
        public Setting()
        {
            this.InitializeComponent();
        }


      
        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
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
            await storagefile.DeleteAsync();

            ContentDialog dialog = new ContentDialog
            {
                
                Content = "Reset done",
                CloseButtonText = "Get it!"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var githubUri = new Uri("https://brabalawuka.github.io/NTUtimetable");
            await Windows.System.Launcher.LaunchUriAsync(githubUri);
        }
    }
}
