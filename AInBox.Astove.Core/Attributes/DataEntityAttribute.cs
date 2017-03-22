using System;
using System.Linq;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataEntityAttribute : Attribute
    {
        public const string PAGE_SIZES = "6, 12, 24, 48, 96, 192, 384";
        public static int[] DefaultPageSizes 
        {
            get
            {
                string[] sizes = PAGE_SIZES.Split(new string[] {","}, StringSplitOptions.None);
                return (from s in sizes select int.Parse(s)).ToArray();
            }
        }

        public string BaseUriTemplate { get; set; }
        public string DisplayName { get; set; }
        public string EntityName { get; set; }
        public string[] Include { get; set; }
        public int[] PageSizes { get; set; }
        public string[] ViewNames { get; set; }
        public string[] ViewProperties { get; set; }
        public string[] WhereClauses { get; set; }
        
        private string whereOperator;
        public string WhereOperator 
        { 
            get 
            {
                if (string.IsNullOrEmpty(whereOperator))
                    WhereOperator = "&&";

                return whereOperator; 
            } 
            set 
            {
                if (!string.IsNullOrEmpty(value) && (value.Trim().Equals("||") || value.Trim().Equals("&&")))
                    whereOperator = string.Format(" {0} ", value.Trim());
            } 
        }
        
        public DataEntityAttribute() { this.PageSizes = DefaultPageSizes; }
        public DataEntityAttribute(string displayName) { this.DisplayName = displayName; this.PageSizes = DefaultPageSizes; }
        public DataEntityAttribute(string displayName, int[] pageSizes) { this.DisplayName = displayName; this.PageSizes = pageSizes; }
    }
}