using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class SalesPromotionBO : BOBase
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string ProductData { get; set; }
        public int ShopId { get; set; }
        public int UserId { get; set; }
        public string quantity { get; set; }
        public string Narration { get; set; }
        public string mobileTransactionDate { get; set; }
        public string UnitId { get; set; }
        public string SyncDate { get; set; }
        public string mobileReferenceNo { get; set; }
        public int GpsSource { get; set; }
        public string mobileDate { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }



        SalesPromotionDAO salesPromotionDAO = new SalesPromotionDAO();
        public int UpdateSalesPromotion()
        {
            return salesPromotionDAO.UpdateSalesPromotion(this);
        }
    }
}
