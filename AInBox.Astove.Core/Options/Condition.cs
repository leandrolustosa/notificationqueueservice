using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AInBox.Astove.Core.Options
{
    public class Condition
    {
        public string Index { get { return string.Empty; } }
        public string Property { get { return string.Empty; } }
        public string DisplayName { get { return string.Empty; } }
        public string DefaultOperator { get { return string.Empty; } }
        public string DisplyOperator { get { return string.Empty; } }
        public string[] Operators { get { return new string[0]; } }
        public string DefaultValue { get { return string.Empty; } }
        public string[] DomainValues { get { return new string[0]; } }
        public string DisplayValue { get { return string.Empty; } }
        public string FilterType { get { return string.Empty; } }
        public string Exists { get { return string.Empty; } }
        public string Mask { get { return string.Empty; } }
        public string Css { get { return string.Empty; } }
        public int Width { get { return 0; } }
        public int Length { get { return 0; } }
    }
}
