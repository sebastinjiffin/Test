using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class SalesReturnBO : BOBase 
    {
        #region Properties

        public int UserId { get; set; }
        public string Mode { get; set; }
        public int ShopId { get; set; }
        public int ProductAttributeId { get; set; }
        public int UnitId { get; set; }
        public int Quantity { get; set; }
        public float Amount { get; set; }
        public string BatchNo { get; set; }
        public string PkdDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string MobileTransactionDate { get; set; }
        public string mobileReferenceNo { get; set; }
        public double Rate { get; set; }
        public string returnReason { get; set; }
        public int GpsSouce { get; set; }
        public string ServerSyncDate { get; set; }
        public string MobileSyncDate { get; set; }
        public string ReceiptNo { get; set; }
        public int SchemeId { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        public string Remark { get; set; }

        #endregion

        SalesReturnDAO salesReturnDAO = new SalesReturnDAO();

        #region Methods

        public int GetSalesReturn()
        {
            return salesReturnDAO.GetSalesReturn(this);
        }

        public int UpdateSalesReturn()
        {
            return salesReturnDAO.UpdateSalesReturn(this);
        }

        #endregion

    }
}
