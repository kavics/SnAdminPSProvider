using Newtonsoft.Json.Linq;
using SenseNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnAdminPowerShellProvider
{
    internal class SnContent
    {
        public Content Content { get; private set; }
        public int Id { get { return Content.Id; } }
        public int ParentId { get { return Content.ParentId; } }
        public string Name { get { return Content.Name; } }
        public string Path { get { return Content.Path; } }
        public string Type { get; private set; }

        public SnContent(Content clientContent)
        {
            this.Content = clientContent;
            this.Type = (clientContent["Type"] as JValue)?.Value as string;
        }
    }

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

        public IEnumerable<SnContent> GetChildren(string snPathOrNullOrEmpty)
        {
            if (string.IsNullOrEmpty(snPathOrNullOrEmpty))
            {
                var root = Content.LoadAsync(GetHeadRequest("/Root"), _server).Result;
                return new[] { new SnContent(root) };
            }

            if (!snPathOrNullOrEmpty.StartsWith("/root", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid path: '{snPathOrNullOrEmpty}'");

            var list = Content.LoadCollectionAsync(GetHeadRequest(snPathOrNullOrEmpty), _server).Result
                .Select(c => new SnContent(c))
                .ToArray();
            return list;
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

        internal Content GetContent(string snPathOrNullOrEmpty)
        {
            string snPath = snPathOrNullOrEmpty;
            if (string.IsNullOrEmpty(snPath))
                snPath = "/Root";

            if (!snPath.StartsWith("/root", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid path: '{snPathOrNullOrEmpty}'");

            return Content.LoadAsync(GetRequest(snPath), _server).Result;
        }

        private ODataRequest GetHeadRequest(string path)
        {
            var req = GetRequest(path, MetadataFormat.None);
            req.Select = new[] { "Id", "ParentId", "Name", "Path", "Type" };
            return req;
        }
        private ODataRequest GetRequest(string path, MetadataFormat meta = MetadataFormat.None)
        {
            var req = new ODataRequest
            {
                SiteUrl = _server.Url,
                Metadata = meta,
            };

            if (path != null)
                req.Path = path;

            return req;
        }
    }
}
