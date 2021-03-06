﻿using Brook.DuDuRiBao.Authorization;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using XPHttp;

namespace BackgroundComponent
{
    public sealed class LiveTileTask : IBackgroundTask
    {
        const string TileTemplateFile = "Assets/TileTemplate.xml";
        const string BodyTag = "{{body}}";
        const string ImageTag = "{{image}}";
        const string CircleTag = "{{Circle}}";

        string _tileTemplate;
        TileUpdater _tileUpdater;

        public LiveTileTask()
        {
            StorageInfo.IsApplication = false;
            InitTemplate();
            InitHttpClient();
            InitTileUpdater();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            await GetLatestNews();
            deferral.Complete();
        }

        private async Task GetLatestNews()
        {
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
            if (story == null || string.IsNullOrEmpty(_tileTemplate))
                return "";

            var content = _tileTemplate;
            content = content.Replace(BodyTag, story.Title);
            content = content.Replace(ImageTag, (story.Images != null && story.Images.Count > 0) ? story.Images[0] : ""); 
            content = content.Replace(CircleTag, (story.Posts != null && story.Posts.Count > 0 && story.Posts[0].Circle != null) ? story.Posts[0].Circle.Name : "");
            return content;
        }

        private void UpdatePrimaryTile(List<string> xmls)
        {
            if (xmls == null || !xmls.Any())
                return;

            try
            {
                _tileUpdater.Clear();
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
        }

        private void InitTemplate()
        {
            _tileTemplate = File.ReadAllText(Path.Combine(Package.Current.InstalledLocation.Path, TileTemplateFile));
        }

        private void InitHttpClient()
        {
            XPHttpClient.DefaultClient.HttpConfig
                .SetBaseUrl(Urls.BaseUrl)
                .SetUseHttpCache(false)
                .SetDefaultHeaders("x-client-id", "4")
                .ApplyConfig();

            var token = Storager.GetLoginInfo();
            if (token != null)
                XPHttpClient.DefaultClient.HttpConfig.SetAuthorization("Bearer", token);
        }
    }
}
