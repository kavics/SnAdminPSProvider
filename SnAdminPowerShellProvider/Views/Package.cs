using System;

namespace SnAdminPowerShellProvider.Views
{
    public class Package : IView
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string Description { get; set; }
        public string LocalPath { get; set; }
        public int PhaseCount { get; set; }
        public string UsesRepo { get; set; }

        public bool IsValid { get; set; }
        public bool IsCompressed { get; set; }
    }
}
