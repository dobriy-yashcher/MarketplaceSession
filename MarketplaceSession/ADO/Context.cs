using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceSession.ADO
{
    public partial class MarketplaceSessionEntities
    {
        private static MarketplaceSessionEntities _context;

        public static MarketplaceSessionEntities GetContext()
        {
            if (_context == null)
                _context = new MarketplaceSessionEntities();

            return _context;
        }
    }
}
