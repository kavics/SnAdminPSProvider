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
            { "snweb1", @"D:\Dev10\web\site1" },
            { "snweb2", @"D:\Dev10\web\site2" },
            { "snweb3", @"D:\Dev10\web\site3" }
        }
        ;
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
            PSDriveInfo drive = new PSDriveInfo("SnAdmin", this.ProviderInfo, "", "", null);
            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>() { drive };
            return drives;
        }

        protected override bool ItemExists(string path)
        {
            if (path == "")
                return true;

            var segments = path.Split('\\');
            if (segments.Length == 1)
            {
                return SnWebs.ContainsKey(segments[0].ToLowerInvariant());
            }
            if (segments.Length == 2)
            {
                return IsPackageExist(segments[0].ToLowerInvariant(), segments[1]);
            }
            return false;
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
            if (string.IsNullOrEmpty(path))
            {
                foreach (var item in SnWebs)
                    WriteItemObject(new WebFolder { Name = item.Key, Path = item.Value }, item.Key, true);
                return;
            }
            var segments = path.Split('\\');
            if(segments.Length == 1)
            {
                var webName = segments[0];
                var packages = FindPackages(webName);
                foreach (var package in packages)
                    WriteItemObject(package.GetView(), $"{path}\\{package.Name}", false);
                return;
            }
            WriteItemObject("not implemented", path, true);
        }

        private static string[] PackageBlacklist = new[] { "bin", "run", "tools" };
        private Package[] FindPackages(string webName)
        {
            var path = SnWebs[webName];
            var adminPath = $"{path}\\Admin";
            var toolsPath = $"{path}\\Admin\\tools";

            var packageNames = System.IO.Directory.GetDirectories(adminPath)
                .Select(System.IO.Path.GetFileName)
                .Except(PackageBlacklist, StringComparer.OrdinalIgnoreCase);
            var toolNames = System.IO.Directory.GetDirectories(toolsPath)
                .Select(System.IO.Path.GetFileName)
                .Except(packageNames);
            var packages = packageNames.Select(n=>Package.Create(path, n))
                .Union(toolNames.Select(n => Package.Create(path, $"tools\\{n}")))
                .ToArray();

            return packages;
        }
        private bool IsPackageExist(string webName, string packageName)
        {
            if (PackageBlacklist.Contains(packageName, StringComparer.OrdinalIgnoreCase))
                return false;

            var path = SnWebs[webName];
            var pkgPath = $"{path}\\Admin\\{packageName}";
            if (System.IO.Directory.Exists(pkgPath))
                return true;

            pkgPath = $"{path}\\Admin\\tools\\{packageName}";
            return System.IO.Directory.Exists(pkgPath);
        }

        protected override string[] ExpandPath(string path)
        {
            return SnWebs.Keys.ToArray();

            return base.ExpandPath(path);

            //return TagsFromPath(path);
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            base.CopyItem(path, copyPath, recurse);
        }
        protected override void MoveItem(string path, string destination)
        {
            base.MoveItem(path, destination);
        }


        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            base.GetChildNames(path, returnContainers);
        }
    }
}
