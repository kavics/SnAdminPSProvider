using Newtonsoft.Json.Linq;
using SenseNet.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnAdminPowerShellProvider
{
    internal class SnContent : ICustomTypeDescriptor
    {
        public Content Content { get; private set; }
        public JObject JObject { get; private set; }
        public int Id
        {
            get
            {
                if (Content != null)
                    return Content.Id;
                if (JObject != null)
                    return (int)JObject["Id"];
                return default(int);
            }
        }
        public int ParentId
        {
            get
            {
                if (Content != null)
                    return Content.ParentId;
                if (JObject != null)
                    return (int)JObject["ParentId"];
                return default(int);
            }
        }

        public string Name
        {
            get
            {
                if (Content != null)
                    return Content.Name;
                if (JObject != null)
                    return (string)JObject["Name"];
                return default(string);
            }
        }
        public string Path
        {
            get
            {
                if (Content != null)
                    return Content.Path;
                if (JObject != null)
                    return (string)JObject["Path"];
                return default(string);
            }
        }
        public string Type { get; private set; }

        public SnContent(Content clientContent)
        {
            this.Content = clientContent;
            this.Type = (clientContent["Type"] as JValue)?.Value as string;
        }
        public SnContent(JObject obj)
        {
            JObject = (JObject)obj.Children().First().Children().First();
            PropertyDescriptors = new PropertyDescriptorCollection(JObject.Properties().Select(p => new JPropertyDescriptor(p.Name)).ToArray());
            this.Type = (string)JObject["Type"];
        }

        // #region // ========================================= ICustomTypeDescriptor

        public PropertyDescriptorCollection PropertyDescriptors { get; set; }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return new AttributeCollection(null);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(null);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return PropertyDescriptors ?? GetContentProperties();
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }



        private PropertyDescriptorCollection GetContentProperties()
        {
return null;

            //var props = new List<PropertyDescriptor>();

            //foreach (var field in this.Fields.Values)
            //{
            //    var fs = FieldSetting.GetRoot(field.FieldSetting);

            //    props.Add(new FieldSettingPropertyDescriptor(field.Name, field.Name, fs));
            //    props.Add(new FieldSettingPropertyDescriptor(fs.BindingName, field.Name, fs));
            //}

            //return new PropertyDescriptorCollection(props.ToArray());
        }

        // #endregion
    }

    /*
    public class FieldSettingPropertyDescriptor : PropertyDescriptor
    {
        private FieldSetting _fieldSetting;
        private readonly string _fieldName;

        public FieldSettingPropertyDescriptor(string bindingName, string fieldName, FieldSetting fieldSetting)
            : base(bindingName, null)
        {
            this._fieldSetting = FieldSetting.GetRoot(fieldSetting);
            this._fieldName = fieldName;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(Content); }
        }

        public override object GetValue(object component)
        {
            var content = component as Content;

            if (content == null)
                throw new ArgumentException("Component must be a content!", "component");

            if (!content.Fields.ContainsKey(_fieldName))
                return null;

            if (_fieldSetting == null && _fieldName.StartsWith("#"))
            {
                // this is a contentlist field. We can find the
                // appropriate field setting for it now, when we have
                // the exact content list
                var cl = content.ContentHandler.LoadContentList() as ContentList;

                if (cl != null)
                {
                    var listFs = cl.FieldSettings.FirstOrDefault(clfs => string.Compare(clfs.Name, _fieldName, StringComparison.InvariantCulture) == 0);
                    if (listFs != null)
                        _fieldSetting = listFs.GetEditable();
                }
            }

            var fs = FieldSetting.GetRoot(content.Fields[_fieldName].FieldSetting);
            object result;

            if (_fieldSetting == null || fs == null)
            {
                // we have not enough info for fullname check
                result = content[_fieldName];
            }
            else
            {
                // return the value only if fieldname refers to
                // the same field (not just a field with the same name)
                result = string.Compare(_fieldSetting.FullName, fs.FullName, StringComparison.InvariantCulture) != 0 ? null : content[_fieldName];
            }

            // format or change the value based on its type

            // CHOICE
            var sList = result as List<string>;
            if (sList != null)
            {
                var chf = _fieldSetting as ChoiceFieldSetting;

                if (chf != null)
                {
                    result = new ChoiceOptionValueList<string>(sList, chf);
                }
                else
                {
                    result = new StringValueList<string>(sList);
                }

                return result;
            }

            // REFERENCE
            var nodeList = result as List<Node>;
            if (nodeList != null)
            {
                return new NodeValueList<Node>(nodeList);
            }

            // NUMBER
            if (result != null && content.Fields[_fieldName] is NumberField)
            {
                if ((decimal)result == ActiveSchema.DecimalMinValue)
                    return null;
            }

            // INTEGER
            if (result != null && content.Fields[_fieldName] is IntegerField)
            {
                if ((int)result == int.MinValue)
                    return null;
            }

            // HYPERLINK
            if (result != null && content.Fields[_fieldName] is HyperLinkField)
            {
                var linkData = result as HyperLinkField.HyperlinkData;
                if (linkData == null)
                    return null;

                var sb = new StringBuilder();
                sb.Append("<a");
                if (linkData.Href != null)
                    sb.Append(" href=\"").Append(linkData.Href).Append("\"");
                if (linkData.Target != null)
                    sb.Append(" target=\"").Append(linkData.Target).Append("\"");
                if (linkData.Title != null)
                    sb.Append(" title=\"").Append(linkData.Title).Append("\"");
                sb.Append(">");
                sb.Append(linkData.Text ?? "");
                sb.Append("</a>");
                return sb.ToString();
            }

            return result;
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return _fieldSetting == null ? typeof(object) : _fieldSetting.FieldDataType; }
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {

        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
    */
}
