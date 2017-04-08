using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SnAdminPowerShellProvider
{
    public class Manifest
    {
        public string ComponentId { get; set; }
        public Version Version { get; set; }
        public string Desription { get; set; }
        public string[] Parameters { get; set; }
        public int PhaseCount { get; set; }
        public bool UsesRepo { get; set; }

        public static Manifest Load(string packageFullPath)
        {
            var files = Directory.GetFiles(packageFullPath);
            if (files.Length != 1)
                return null;

            var xml = new XmlDocument();
            try
            {
                xml.Load(files[0]);
            }
            catch (Exception e)
            {
                return null;
            }

            var manifest = new Manifest
            {
                ComponentId = xml.DocumentElement.SelectSingleNode("ComponentId").InnerText,
                Desription = xml.DocumentElement.SelectSingleNode("Description")?.InnerText,
                Version = Version.Parse(xml.DocumentElement.SelectSingleNode("Version").InnerText),
                Parameters = ParseParameters(xml),
                PhaseCount = Math.Max(1, xml.DocumentElement.SelectNodes("Steps/Phase").Count),
                UsesRepo = xml.DocumentElement.SelectNodes("//StartRepository").Count > 0
            };

            return manifest;
        }

        private static string[] ParseParameters(XmlDocument xml)
        {
            return xml.DocumentElement.SelectNodes("Parameters/Parameter")
                .Cast<XmlElement>()
                .Select(e => e.Attributes["name"].Value.Substring(1))
                .ToArray();
        }
    }
}
