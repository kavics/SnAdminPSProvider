using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SnAdminPowerShellProvider.Views
{
    internal class ContentHead
    {
        public int Id { get; }
        public int ParentId { get; }
        public string Name { get; }
        public string Type { get; }

        public ContentHead(PSObject content)
        {
            Id = Convert.ToInt32(content.Properties["Id"].Value);
            ParentId = Convert.ToInt32(content.Properties["ParentId"].Value);
            Name = (string)content.Properties["Name"].Value;
            Type = (string)content.Properties["Type"].Value;
        }
    }
}
