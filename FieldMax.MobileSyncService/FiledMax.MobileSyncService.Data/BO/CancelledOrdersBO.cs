using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class CancelledOrdersBO : BOBase
    {
        #region Properties

        public int OrderTakenBy { get; set; }
        public string Mode { get; set; }
        public string orderIds { get; set; }
        public string ShopId { get; set; }

        #endregion

        CancelledOrdersDAO cancelledOrdersDAO = new CancelledOrdersDAO();

        #region Methods
        public DataSet GetCancelledOrdersDetails()
        {
            return cancelledOrdersDAO.GetCancelledOrdersDetails(this);
        }
        
        public DataSet GetProductList(int orderId)
        {
            return cancelledOrdersDAO.getProductList(this, orderId);
        }

        public void CancelOrders()
        {
             cancelledOrdersDAO.CancelOrders(this);
        }

        #endregion
    }
}
