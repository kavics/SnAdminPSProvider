using Microsoft.PowerShell.Commands;
using SnAdminPowerShellProvider.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SnAdminPowerShellProvider
{
    [CmdletProvider("SnAdminPowerShellProvider", ProviderCapabilities.ExpandWildcards)]
    public class SnAdminPsProvider : NavigationCmdletProvider
    {
        private static Dictionary<string, string> SnWebs { get; } = new Dictionary<string, string>()
        {
            { "SnDemo", @"https://demo.sensenet.com" }
        };

        private static SnWeb SnWeb => new SnWeb(SnWebs["SnDemo"]);

        public static void AddSnDrive(string name, string path)
        {
            SnWebs[name.ToLowerInvariant()] = path;
        }

        protected override bool IsValidPath(string path)
        {
            return true;  // All paths are valid for now
        }

        protected override Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            PSDriveInfo drive = new PSDriveInfo("SnDemo", this.ProviderInfo, "", "", null);
            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>() { drive };
            return drives;
        }

        private string _currentContentPath;
        protected override bool ItemExists(string path)
        {
            return true;

            path = path.TrimEnd('\\', '/');
            if (path == "")
                return true;

            path = path.Replace('\\', '/');
            if (!path.StartsWith("/"))
                path = "/" + path;
            if (!path.StartsWith("/root", StringComparison.OrdinalIgnoreCase))
                return false;

            if (path == _currentContentPath)
                return true;
            return SnWeb.Exists(path);
        }
        protected override void GetItem(string path)
        {
            // only on the first level
            WriteItemObject(path, path, true);
        }

        protected override bool IsItemContainer(string path)
        {
            return true;
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            path = path.Replace('\\', '/');
            path = path.TrimEnd('/');
            if (!path.StartsWith("/"))
                path = "/" + path;
            if (!path.StartsWith("/root", StringComparison.OrdinalIgnoreCase))
            {
                WriteItemObject($"Invalid path: '{path}'", path, true);
                return;
            }

            foreach (var content in SnWeb.GetChildren(path))
            {
                _currentContentPath = content.Path;
                WriteItemObject(new ContentHead(content), content.Path, true);
                _currentContentPath = null;
            }
            //WriteItemObject("not implemented", path, true);
        }

        //protected override string[] ExpandPath(string path)
        //{
        //    return SnWebs.Keys.ToArray();
        //    return base.ExpandPath(path);
        //    //return TagsFromPath(path);
        //}

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            base.CopyItem(path, copyPath, recurse);
        }
        protected override void MoveItem(string path, string destination)
        {
            base.MoveItem(path, destination);
        }

    }
}
