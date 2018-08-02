using System;
using System.Collections.Generic;
using System.Linq;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
using System.Data;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class StockBO : BOBase 
    {
        public int UserId { get; set; }
        public int ShopId { get; set; }
        public string ProductData { get; set; }
        public string StockData { get; set; }
        public string UnitData { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string mobileTransactionDate { get; set; }
        public string SyncDate { get; set; }
        public string MobileReferenceNo { get; set; }
        public int GpsSource { get; set; }
        public string mobileDate { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        public int StockHeaderId { get; set; }
        public string Mode { get; set; }
        public string StoreId { get; set; }

        StockDAO stockDAO = new StockDAO();

        public int UpdateStock()
        {
            return stockDAO.UpdateStock(this);
        }

        public int UpdateVanStockRequest()
        {
            return stockDAO.UpdateVanStockRequest(this);
        }

        public int GetUserStoreId()
        {
            return stockDAO.GetUserStoreId(this);
        }

        public int UpdateRecievedStock()
        {
            return stockDAO.UpdateRecievedStock(this);
        }
        public int UpdateLoadedStock()
        {
            return stockDAO.UpdateLoadedStock(this);
        }
        public DataSet GetFinalSettlementDetails()
        {
            return stockDAO.GetFinalSettlementDetails(this);
        }
       
    }
}
