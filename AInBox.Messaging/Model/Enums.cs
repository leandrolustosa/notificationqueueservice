using System.ComponentModel;

namespace AInBox.Messaging.Model.Enums
{
    public enum EmailContentType
    {
        [Description("html")]
        Html = 0,
        [Description("txt")]
        Text = 1
    }
}
