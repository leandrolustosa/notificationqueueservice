using System;
using System.Collections.Generic;
using System.Linq;
using AInBox.Astove.Core.Filter;

namespace AInBox.Astove.Core.Options
{
    public class ColumnDefinition
    {
        public string Field { get; set; }
        public string DisplayName { get; set; }
        public string CellTemplate { get; set; }
        public bool EnableCellEdit { get; set; }
        public string EditableCellTempate { get; set; }
        public string PageTemplates { get; set; }
        public string DateFormat { get; set; }
        public string Align { get; set; }
        public int Width { get; set; }

        public void CopyPropertiesValue(object value)
        {
            Type c = value.GetType();
            Type t = this.GetType();
            foreach (var propInfo in t.GetProperties())
            {
                if (c.GetProperty(propInfo.Name) != null)
                {
                    if (propInfo.Name.Equals("Field") || propInfo.Name.Equals("DisplayName"))
                    {
                        if (c.GetProperty(propInfo.Name).GetValue(value, null) != null)
                        {
                            propInfo.SetValue(this, c.GetProperty(propInfo.Name).GetValue(value, null), null);
                        }
                    }
                    else
                    {
                        propInfo.SetValue(this, c.GetProperty(propInfo.Name).GetValue(value, null), null);
                    }
                }
            }
        }
    }
}
