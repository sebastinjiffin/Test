using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
using System.Data;

namespace FieldMax.MobileSyncService.Data.BO
{
   public class StockAgingBO :BOBase
    {
       #region Properties

       public int UserId { get; set; }
       public int ShopId { get; set; }
       public string mobileTransactionDate { get; set; }
       public string unitSet { get; set; }
       public string productData { get; set; }
       public string quantitydata { get; set; }
       public string batchNoData { get; set; }
       public string manufactureddate { get; set; }
       public string expiryDate { get; set; }
       public string age { get; set; }
       public string remarks { get; set; }
       public string mobileReferenceNo { get; set; }
       public string ProcessName { get; set; }
       public string Latitude { get; set; }
       public string Longitude { get; set; }
       public int GpsSource { get; set; }
       public string SyncDate { get; set; }
       public string networkProvider { get; set; }
       public string signalStrength { get; set; }
       #endregion

       StockAgingDAO StockAgingDAO = new StockAgingDAO();

       #region Methods
       public int UpdateStockAging()
       {
           return StockAgingDAO.UpdateStockAging(this);
       }
       #endregion
    }
}
