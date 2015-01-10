using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
namespace DayArticle.Common
{
    public class ControlInfo : INotifyPropertyChanged
    {
        private static ControlInfo _instance = new ControlInfo();
        public static ControlInfo Instance { get { return _instance; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private HttpClient httpClient;
        private HttpBaseProtocolFilter filter;
        private ApplicationDataContainer roamingSettings = null;
        private ApplicationDataCompositeValue composite = null;
        private ControlInfo()
        {
            filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            httpClient = new HttpClient(filter);

            roamingSettings = ApplicationData.Current.RoamingSettings;

            composite = (ApplicationDataCompositeValue)roamingSettings.Values[StoreData.StoreDataInfo];
            if(composite == null)
            {
                composite = new ApplicationDataCompositeValue();
                StoreData.CanSaveStoreData = true;
                ContentFontSize = 22;
            }
            else
            {
                if (composite.ContainsKey(StoreData.NotFixedHead))
                {
                    NotFixedHead = (bool)composite[StoreData.NotFixedHead];
                }
                if(composite.ContainsKey(StoreData.ContentFontSize))
                {
                    ContentFontSize = (int)composite[StoreData.ContentFontSize];
                }
                if(composite.ContainsKey(StoreData.LightModeTheme))
                {
                    LightModeTheme = (bool)composite[StoreData.LightModeTheme];
                }
                StoreData.CanSaveStoreData = true;
            }
        }
        
        public static class StoreData
        {
            public static string LightModeTheme = "LightModeTheme";
            public static string ContentFontSize = "ContentFontSize";
            public static string NotFixedHead = "NotFixedHead";
            public static string StoreDataInfo = "StoreDataInfo";
            public static bool CanSaveStoreData;
        }
        private void SaveSettingValue(string name, object val)
        {
            if (StoreData.CanSaveStoreData)
            {
                composite[name] = val;
                roamingSettings.Values[StoreData.StoreDataInfo] = composite;
            }
        }
        #region 背景
        private bool lightModeTheme;

        public bool LightModeTheme
        {
            get { return lightModeTheme; }
            set
            {
                if (value != this.lightModeTheme)
                {
                    this.lightModeTheme = value;
                    SaveSettingValue(StoreData.LightModeTheme, value);
                }
            }
        }
        #endregion

        #region 字体大小
        private int contentFontSize;
        public int ContentFontSize
        {
            get { return contentFontSize; }
            set
            {
                if (value != this.contentFontSize)
                {
                    this.contentFontSize = value;
                    NotifyPropertyChanged("ContentFontSize");
                    SaveSettingValue(StoreData.ContentFontSize, value);
                }
            }
        }
        #endregion

        #region 固定标题
        private bool notFixedHead;
        public bool NotFixedHead
        {
            get { return notFixedHead; }
            set
            {
                if (value != this.notFixedHead)
                {
                    this.notFixedHead = value;
                    NotifyPropertyChanged("NotFixedHead");
                    SaveSettingValue(StoreData.NotFixedHead, value);
                }
            }
        }
        #endregion
      
        public string FontSizeDisPlay { get { return "正文大小：" + ContentFontSize; } }

        #region 内容显示
        private Regex headRegex = new Regex("<h1>([\\s\\S]+)</h1>");
        private Regex authorRegex = new Regex("<p class=\"article_author\">.*?</p>");
        private Regex articleRegex = new Regex("<div class=\"article_text\">([\\s\\S]*?)</div>");

        private string replaceText = "</?[^>]+/?>";

        private string head = string.Empty;
        public string Head
        {
            get { return head; }
            set
            {
                if (value != this.head)
                {
                    this.head = value;
                    NotifyPropertyChanged("Head");
                }
            }
        }

        private string author = string.Empty;
        public string Author
        {
            get { return author; }
            set
            {
                if (value != this.author)
                {
                    this.author = value;
                    NotifyPropertyChanged("Author");
                }
            }
        }
        private string content;
        public string Content
        {
            get { return content; }
            set
            {
                if (value != this.content)
                {
                    this.content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }
        #endregion
       
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        

        public async Task Reflesh(Uri url)
        {
            //rootPage.NotifyUser("In progress", NotifyType.StatusMessage);

            try
            {
                string response = await httpClient.GetStringAsync(url);

                GetText(response);

                //rootPage.NotifyUser(
                //    "Completed. Response came from " + response.Source + ".",
                //    NotifyType.StatusMessage);

            }
            catch (TaskCanceledException)
            {
                //rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage);
            }
            catch (Exception ex)
            {
                //rootPage.NotifyUser("Error: " + ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void GetText(string getText)
        {
            Head = Regex.Replace(headRegex.Match(getText).ToString(), replaceText, "");
            Author = Regex.Replace(authorRegex.Match(getText).ToString(), replaceText, "");
            var origin = articleRegex.Match(getText).ToString().Replace(" ", "");

            Content = Regex.Replace(
                Regex.Replace(
                Regex.Replace(origin, "<p>", "        "),
                "</p>", "\n"),
                replaceText, "");
        }
    }
}
