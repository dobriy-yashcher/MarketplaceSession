using ProductDelivery.ADO;
using System.Windows;
using System.Windows.Controls;

namespace ProductDelivery.Components
{
    static class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Window CurrentWindow { get; set; }
        public static User AuthorizedUser { get; set; }
        public static Cart CurrentCart { get; set; }
    }
}
