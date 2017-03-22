using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace AInBox.Astove.Core.Enums
{
    public class Utility
    {
        [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        public class EnumOrderAttribute : Attribute
        {
            public int Order { get; set; }

            public EnumOrderAttribute(int order)
            { 
                this.Order = order; 
            }
        }

        public class EnumPosition
        {
            public int Order { get; set; }
            public int Value { get; set; }
            public string Text { get; set; }
        }


        public static string GetEnumText(System.Enum value)
        {
            return GetEnumText(value.GetType(), value.ToString());
        }

        public static string GetEnumText(Type type, int value)
        {
            string name = System.Enum.GetName(type, value);
            return GetEnumText(type, name);
        }

        public static string GetEnumText(Type type, string name)
        {
            FieldInfo fi = type.GetField(name);
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes
                (typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : name;
        }

        public static int GetEnumOrder(Type type, string name)
        {
            FieldInfo fi = type.GetField(name);
            EnumOrderAttribute[] attributes =
                (EnumOrderAttribute[])fi.GetCustomAttributes
                (typeof(EnumOrderAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Order : 0;
        }

        public static Dictionary<int, string> GetEnumTexts(Type type)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();


            List<EnumPosition> orderlist = new List<EnumPosition>();

            foreach (string name in System.Enum.GetNames(type))
                orderlist.Add(new EnumPosition() { Order = GetEnumOrder(type, name), Value = Convert.ToInt32(System.Enum.Parse(type, name)), Text = GetEnumText(type, name) });

            foreach (var ee in (from en in orderlist
                                orderby en.Order, en.Value
                                select en))
                list.Add(ee.Value, ee.Text);

            return list;
        }
    }
}
