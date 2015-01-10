using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
namespace DayArticle.Common
{
    public class ControlInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private HttpClient httpClient;
        private HttpBaseProtocolFilter filter;


        private ControlInfo()
        {
            ContentFontSize = 22;
            filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            httpClient = new HttpClient(filter);
        }
        private static ControlInfo _instance = new ControlInfo();
        public static ControlInfo Instance { get { return _instance; } }
        /// <summary>
        /// 背景模式
        /// </summary>
        public bool LightModeTheme { get; set; }


        private int contentFontSize;
        /// <summary>
        /// 字体大小
        /// </summary>
        public int ContentFontSize
        {
            get { return contentFontSize; }
            set
            {
                if(value != this.contentFontSize)
                {
                    this.contentFontSize = value;
                    NotifyPropertyChanged("ContentFontSize");
                }
            }
        }
        /// <summary>
        /// 固定标题
        /// </summary>
        public bool NotFixedHead { get; set; }
        public string FontSizeDisPlay { get { return "正文大小：" + ContentFontSize; } }

        private Regex headRegex = new Regex("<h1>([\\s\\S]+)</h1>");
        private Regex authorRegex = new Regex("<p class=\"article_author\">.*?</p>");
        private Regex articleRegex = new Regex("<div class=\"article_text\">([\\s\\S]*?)</div>");

        private string replaceText = "</?[^>]+/?>";
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string head = string.Empty;
        public string Head { get { return head; } set
        {
            if(value != this.head)
            {
                this.head = value;
                NotifyPropertyChanged("Head");
            }
        } }

        private string author = string.Empty;
        public string Author
        {
            get { return author; }
            set
            {
                if(value != this.author)
                {
                    this.author = value;
                    NotifyPropertyChanged("Author");
                }
            }
        }
        private string content;
        public string Content { get { return content; } set {
            if(value != this.content){
                this.content = value;
                NotifyPropertyChanged("Content");
            }
        } }

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

        public void GetText(string getText)
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
