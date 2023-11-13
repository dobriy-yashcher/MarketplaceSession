using MarketplaceSession.ADO;
using System.Windows.Controls;

namespace MarketplaceSession.Components
{
    static class Manager
    {
        public static Frame MainFrame { get; set; }
        public static User AuthorizedUser { get; set; }
        public static Cart CurrentCart { get; set; }
    }
}
