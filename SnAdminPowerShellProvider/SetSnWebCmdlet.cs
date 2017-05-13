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
            throw new DirectoryNotFoundException("Location is not found.");
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
