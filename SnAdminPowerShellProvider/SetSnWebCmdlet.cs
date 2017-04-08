using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace SnAdminPowerShellProvider
{
    [Cmdlet(VerbsCommon.Set, "SnWeb")]
    public class SetSnWebCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string Location { get; set; }

        protected override void ProcessRecord()
        {
            var path = Path.GetFullPath(Path.Combine(this.SessionState.Path.CurrentFileSystemLocation.Path, Location));
            SnAdminPsProvider.AddSnDrive(Name, path);
            CheckSnWeb(path);
            //WriteObject($"{Name} --> {path}");
        }

        private void CheckSnWeb(string path)
        {
            if(!Directory.Exists(path))
                throw new DirectoryNotFoundException("Location is not found.");

            var webconfig = File.Exists(Path.Combine(path, "web.config"));
            var appData = Directory.Exists(Path.Combine(path, "app_data"));
            var snAdminExe = File.Exists(Path.Combine(path, "admin/bin/snadmin.exe"));

            if (!(File.Exists(Path.Combine(path, "web.config"))
                && Directory.Exists(Path.Combine(path, "app_data"))
                && File.Exists(Path.Combine(path, "admin/bin/snadmin.exe"))))
                throw new ArgumentException("Location is not a valid Sense/Net web directory");
        }
    }

    [RunInstaller(true)]
    public class GetProcPSSnapIn01 : PSSnapIn
    {
        public GetProcPSSnapIn01() : base() { }

        public override string Name { get { return "SetDrive"; } }

        public override string Vendor { get { return "Gyebi"; } }

        public override string VendorResource { get { return "SetDrive,Gyebi"; } }

        public override string Description { get { return "This is a PowerShell snap-in that includes the get-proc cmdlet."; } }
    }
}
