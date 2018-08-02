using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public class SalesmanTargetBO:BOBase
    {
       SalesmanTargetDAO targetDAO = new SalesmanTargetDAO();

        #region Properties

        public string SalesManId { get; set; }
        public DateTime Month { get; set; }
        public string Amount { get; set; }
        public string Quantity { get; set; }

        #endregion

        #region Methods

        public int GetTargetAmount()
        {
            return targetDAO.GetTargetAmount(this);
        }

        #endregion
    }
}
