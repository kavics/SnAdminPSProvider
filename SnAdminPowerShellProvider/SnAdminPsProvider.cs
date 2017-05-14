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
            return true; //TODO: Eliminates performance problems but not-existent item can be current..

            //path = path.TrimEnd('\\', '/');
            //if (path == "")
            //    return true;

            //path = path.Replace('\\', '/');
            //if (!path.StartsWith("/"))
            //    path = "/" + path;
            //if (!path.StartsWith("/root", StringComparison.OrdinalIgnoreCase))
            //    return false;

            //if (path == _currentContentPath)
            //    return true;
            //return SnWeb.Exists(path);
        }
        protected override bool IsItemContainer(string path)
        {
            return true;
        }

        protected override void GetItem(string path)
        {
            //UNDONE: need to write all fields.
            var content = SnWeb.GetContent(SnPath(path));
            WriteItemObject(content, content.Path, true);
            WritePropertyObject("asdf", path);
            _currentContentPath = null;
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            foreach (var content in SnWeb.GetChildren(SnPath(path)))
            {
                _currentContentPath = content.Path;
                WriteItemObject(new ContentHead(content), content.Path, true);
                _currentContentPath = null;
            }
            return;
        }

        protected override string[] ExpandPath(string path)
        {
            //var x = SenseNet.Client.RepositoryPath.GetParentPath(path);
            var segments = path.Split('\\', '/');
            if (segments.Length == 1)
                return new[] { "Root" };

            string namePrefix;
            var namePattern = segments.Last();
            if (namePattern.EndsWith("*"))
                namePrefix = namePattern.TrimEnd('*');
            else
                throw new NotSupportedException();

            var snPath = "/" + string.Join("/", segments.Take(segments.Length - 1).ToArray());

            var result = SnWeb.GetChildren(snPath)
                .Where(c => c.Name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Name)
                .OrderBy(s => s)
                .Select(s => string.Join("/", segments.Take(segments.Length - 1).ToArray()) + "\\" + s)
                .ToArray();

            return result;
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            base.CopyItem(path, copyPath, recurse);
        }
        protected override void MoveItem(string path, string destination)
        {
            base.MoveItem(path, destination);
        }

        /* =================================================================================*/

        public static string SnPath(string path)
        {
            var snPath = path.Replace('\\', '/');
            snPath = snPath.TrimEnd('/');
            if (!snPath.StartsWith("/"))
                snPath = "/" + snPath;

            if (snPath == "/") // drive root (parent of /Root)
                return null;

            if (snPath.StartsWith("/Root", StringComparison.OrdinalIgnoreCase))
                return snPath;

            throw new ApplicationException($"Invalid path: '{path}'");
        }
    }
}
