using Newtonsoft.Json.Linq;
using SenseNet.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SnAdminPowerShellProvider
{
    internal class SnWeb
    {
        static SnWeb()
        {
            ClientContext.Initialize(new[] { new ServerContext { Url = "http://localhost", Username = "admin", Password = "admin" } });
        }

        private ServerContext _server;

        public SnWeb(string url)
        {
            _server = new ServerContext { Url = url, Username = "admin", Password = "admin" };
        }

        public IEnumerable<PSObject> GetChildren(string snPathOrNullOrEmpty)
        {
            if (string.IsNullOrEmpty(snPathOrNullOrEmpty))
            {
                var root = GetContent(null); // Content.LoadAsync(GetHeadRequest("/Root"), _server).Result;
                return new[] { root };
            }

            if (!snPathOrNullOrEmpty.StartsWith("/root", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid path: '{snPathOrNullOrEmpty}'");

            //var list = Content.LoadCollectionAsync(GetHeadRequest(snPathOrNullOrEmpty), _server).Result
            //    .Select(c => new SnContent(c))
            //    .ToArray();
            //return list;

            var responseJson = (JObject)RESTCaller.GetResponseJsonAsync(GetRequest(snPathOrNullOrEmpty, true), _server).Result;
            var container = (JObject)responseJson.Children().First().Children().First();
            var jContents = (JArray)container["results"];
            var contents = jContents.Select(c => GetPsObject((JObject)c)).ToArray();

            return contents;
        }

        internal bool Exists(string snPath)
        {
            var req = new ODataRequest
            {
                Path = snPath,
                Select = new[] { "Id", "ParentId", "Name", "Path" },
                Metadata = MetadataFormat.None,

                SiteUrl = _server.Url,
            };

            var content = Content.LoadAsync(req, _server).Result;
            return content != null;
        }

        internal PSObject GetContent(string snPathOrNullOrEmpty)
        {
            string snPath = snPathOrNullOrEmpty;
            if (string.IsNullOrEmpty(snPath))
                snPath = "/Root";

            if (!snPath.StartsWith("/root", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid path: '{snPathOrNullOrEmpty}'");

            var responseJson = (JObject)RESTCaller.GetResponseJsonAsync(GetRequest(snPath), _server).Result;
            var contentJson = (JObject)responseJson.Children().First().Children().First();
            return GetPsObject(contentJson);
        }

        private PSObject GetPsObject(JObject jObject)
        {
            var psObject = new PSObject();
            foreach (var item in jObject.Properties())
            {
                var member = new PSNoteProperty(item.Name, GetValue(item.Value));
                psObject.Properties.Add(member);
            }
            return psObject;
        }
        private object GetValue(object input)
        {
            var jValue = input as JValue;
            if (jValue != null)
                return jValue.Value;
            var jArray = input as JArray;
            if (jArray != null)
                return string.Join(", ", jArray.Select(i => i.ToString()).ToArray());
            return input;
        }

        private ODataRequest GetHeadRequest(string path)
        {
            var req = GetRequest(path, false, MetadataFormat.None);
            req.Select = new[] { "Id", "ParentId", "Name", "Path", "Type" };
            return req;
        }
        private ODataRequest GetRequest(string path, bool isCollection = false, MetadataFormat meta = MetadataFormat.None)
        {
            var req = new ODataRequest
            {
                SiteUrl = _server.Url,
                IsCollectionRequest = isCollection,
                Metadata = meta,
            };

            if (path != null)
                req.Path = path;

            return req;
        }
    }
}
