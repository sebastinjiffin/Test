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
    public class StockReconcilationBO : BOBase
    {
        public string ReconcileDate { get; set; }
        public int StoreId { get; set; }
        public int UserId { get; set; }
        public string ProductIdData { get; set; }
        public string ReasonIdData { get; set; }
        public string RateData { get; set; }
        public string QuantityData { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public int GpsSource { get; set; }
        public bool UnloadStatus { get; set; }
        public int StockReconcileHeaderId { get; set; }
        public string SyncDate { get; set; }
        public string mobileDate { get; set; }
        public string MobileRefNo { get; set; }
        public string UnitData { get; set; }
        public string Remarks { get; set; }

        StockReconcilationDAO stockReconcilationDAO = new StockReconcilationDAO();

        public int UpdateStockReconcilation()
        {
            return stockReconcilationDAO.UpdateStockReconcilation(this);
        }
        public int UpdateVanUnloadStatus()
        {
            return stockReconcilationDAO.UpdateVanUnloadStatus(this);
        }

        public DataSet GetVanUnloadDetails()
        {
            return stockReconcilationDAO.GetVanUnloadDetails(this);
        }
    }
}
