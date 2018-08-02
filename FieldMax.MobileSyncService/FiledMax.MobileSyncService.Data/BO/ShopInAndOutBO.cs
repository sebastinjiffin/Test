using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ShopInAndOutBO : BOBase
    {
        #region Properties
        public int ShopId { get; set; }
        public int UserId { get; set; }
        public string ShopIn { get; set; }
        public string ShopOut { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string MobileReferenceNo { get; set; }
        public int gpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string MobileTransactionDate { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        public string IsGpsForciblyEnabled { get; set; }
        #endregion

        ShopInAndOutDAO shopInAndOutDAO = new ShopInAndOutDAO();

        #region Methods
        public int UpdateShopInAndOut()
        {
            return shopInAndOutDAO.UpdateShopInAndOut(this);
        }

        #endregion

    }
}
