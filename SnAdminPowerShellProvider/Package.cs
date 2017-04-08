using System;
using System.Diagnostics;
using System.IO;

namespace SnAdminPowerShellProvider
{
    [DebuggerDisplay("{Name} {Version}")]
    public class Package
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string Description { get; set; }
        public string LocalPath { get; set; }
        public int PhaseCount { get; set; }
        public string UsesRepo { get; set; }
        public bool IsValid { get; set; }
        public bool IsCompressed { get; set; }

        public static Package Create(string webFolderPath, string localPath)
        {
            var fullPath = Path.Combine(webFolderPath, "Admin\\" + localPath);
            var manifest = Manifest.Load(fullPath);
            if (manifest == null)
                return new Package
                {
                    Name = Path.GetFileName(localPath),
                    LocalPath = localPath,
                    IsValid = false
                };
            return new Package
            {
                Name = Path.GetFileName(localPath),
                LocalPath = localPath,
                IsValid = true,
                Version = manifest.Version,
                Description = manifest.Desription,
                PhaseCount = manifest.PhaseCount,
                UsesRepo = manifest.UsesRepo ? "yes" : "no"
            };
        }
    }
}
