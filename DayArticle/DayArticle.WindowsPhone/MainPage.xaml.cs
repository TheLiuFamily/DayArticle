using DayArticle.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DayArticle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

         private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
         private static readonly Uri HomeUrl = new Uri("http://meiriyiwen.com/", UriKind.Absolute);
        private static readonly Uri RandomUrl = new Uri("http://meiriyiwen.com/random/", UriKind.Absolute);
        ControlInfo settings = ControlInfo.Instance;
        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            navigationHelper.HideStatusBar();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.DataContext = settings;

            GoHome();
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //if (e.PageState != null)
            //{
            //    if (e.PageState.ContainsKey("LightModeTheme"))
            //    {
            //        settings.LightModeTheme = (bool)e.PageState["LightModeTheme"];
            //    }
            //    if (e.PageState.ContainsKey("ContentFontSize"))
            //    {
            //        settings.ContentFontSize = (int)e.PageState["ContentFontSize"];
            //    }
            //    if (e.PageState.ContainsKey("NotFixedHead"))
            //    {
            //        settings.NotFixedHead = (bool)e.PageState["NotFixedHead"];
            //    }
            //}
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

            //e.PageState["LightModeTheme"] = settings.LightModeTheme;
            //e.PageState["ContentFontSize"] = settings.ContentFontSize;
            //e.PageState["NotFixedHead"] = settings.NotFixedHead;
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            Init();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private void Init()
        {
            this.RequestedTheme = settings.LightModeTheme ? ElementTheme.Light : ElementTheme.Dark;
            ContentField.FontSize = settings.ContentFontSize;
            if (!settings.NotFixedHead)
            {
                HeadField.Visibility = Windows.UI.Xaml.Visibility.Visible;
                AuthorField.Visibility = Windows.UI.Xaml.Visibility.Visible;
                HeadField1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                AuthorField1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                HeadField.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                AuthorField.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                HeadField1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                AuthorField1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void GoHome()
        {
            Reflesh(HomeUrl);
        }

        private void GoRandom()
        {
            Reflesh(RandomUrl);
        }
        private async void Reflesh(Uri url)
        {
            await settings.Reflesh(url);
            //刷新后滚动到行头
            scrollViewer.ChangeView(null, 0, null, false);
        }


        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            GoRandom();
        }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            GoHome();
        }

        private void setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Setting));
        }
    }
}
