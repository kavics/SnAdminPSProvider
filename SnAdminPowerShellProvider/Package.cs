using System;
using System.IO;

namespace SnAdminPowerShellProvider
{
    public class Package
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string Description { get; set; }
        public string LocalPath { get; set; }
        public bool IsCompressed { get; set; }

        public static Package Create(string webFolderPath, string localPath)
        {
            return new Package { Name = Path.GetFileName(localPath) };
        }
    }
}
