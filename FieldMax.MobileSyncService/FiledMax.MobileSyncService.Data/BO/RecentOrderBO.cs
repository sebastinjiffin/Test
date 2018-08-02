using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;



namespace FieldMax.MobileSyncService.Data.BO
{
   public class RecentOrderBO:BOBase
    {
        RecentOrderDAO recentOrderDAO = new RecentOrderDAO();

        #region Properties

        public string ShopId{ get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string UpdateOrderId { get; set; }
        
        #endregion

        #region Methods

        public DataSet GetRecentOrders()
        {
            return recentOrderDAO.GetRecentOrders(this);
        }

        public DataSet GetOrdersToConfirm()
        {
            return recentOrderDAO.GetOrdersToConfirm(this);
        }

        public DataSet GetOrderDetails()
        {
            return recentOrderDAO.GetOrderDetails(this);
        }

        public int updateOrderStatus()
        {
            return recentOrderDAO.updateOrderStatus(this);
        }
        #endregion
    }
}
