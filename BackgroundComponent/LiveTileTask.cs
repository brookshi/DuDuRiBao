using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Brook.BackgroundComponent
{
    public sealed class LiveTileTask : IBackgroundTask
    {
        const string TileTemplateFile = "TileTemplate.xml";
        const string BodyTag = "{{body}}";
        const string ImageTag = "{{image}}";

        string _tileTemplate;
        TileUpdater _tileUpdater;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            await GetLatestNews();
            deferral.Complete();
        }

        private async Task GetLatestNews()
        {
            await AuthorizationHelper.AutoLogin();

            var timeLine = await DataRequester.RequestLatestTimeLine();
            UpdatePrimaryTile(BuildAllTileXml(timeLine));
        }

        List<string> BuildAllTileXml(TimeLine timeLine)
        {
            List<string> list = new List<string>();
            if (timeLine == null || timeLine.Items == null || timeLine.Items.Count == 0)
                return list;

            var Count = Math.Min(timeLine.Items.Count, 5);
            for (int i = 0; i < Count; i++)
            {
                list.Add(BuildTileXmlForStory(timeLine.Items[i], i));
            }
            return list;
        }

        string BuildTileXmlForStory(Story story, int index)
        {
            var content = _tileTemplate;
            content = content.Replace(BodyTag, story.Title);
            content = content.Replace(ImageTag, story.Images[0]);
            return content;
        }

        private void UpdatePrimaryTile(List<string> xmls)
        {
            if (xmls == null || !xmls.Any())
            {
                return;
            }

            try
            {
                InitTileUpdater();

                foreach (var xml in xmls)
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(WebUtility.HtmlDecode(xml));
                    _tileUpdater.Update(new TileNotification(doc));
                }
            }
            catch (Exception)
            {
            }
        }

        private void InitTileUpdater()
        {
            if (_tileUpdater == null)
            {
                _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                _tileUpdater.EnableNotificationQueueForWide310x150(true);
                _tileUpdater.EnableNotificationQueueForSquare150x150(true);
                _tileUpdater.EnableNotificationQueueForSquare310x310(true);
                _tileUpdater.EnableNotificationQueue(true);
            }
            _tileUpdater.Clear();
        }
    }
}
