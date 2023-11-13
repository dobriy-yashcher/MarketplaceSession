using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDelivery.ADO
{
    public partial class ProductDeliveryEntities
    {
        private static ProductDeliveryEntities _context;

        public static ProductDeliveryEntities GetContext()
        {
            if (_context == null)
                _context = new ProductDeliveryEntities();

            return _context;
        }
    }
}
