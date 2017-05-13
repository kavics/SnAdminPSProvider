using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnAdminPowerShellProvider.Views
{
    internal class ContentHead
    {
        private SnContent _content;

        public int Id { get { return _content.Id; } }
        public int ParentId { get { return _content.ParentId; } }
        public string Name { get { return _content.Name; } }
        public string Type { get { return _content.Type; } }

        public ContentHead(SnContent content)
        {
            _content = content;
        }
    }
}
