using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class PriceRangeBO : BOBase
    {
        PriceRangeDAO priceRangeDAO = new PriceRangeDAO();

        #region Methods
        public DataSet GetPriceRange()
        {
            return priceRangeDAO.GetPriceRange(this);
        }

        #endregion
    }
}
