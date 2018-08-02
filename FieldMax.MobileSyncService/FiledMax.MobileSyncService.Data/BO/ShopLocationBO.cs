using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ShopLocationBO : BOBase
    {
        ShopLocationDAO shopLocationDAO = new ShopLocationDAO();

        #region Properties
        public int UserId { get; set; }
        public int ShopId { get; set; }
        public int GPSSource { get; set; }
        public string Latitude { get; set; }
        public string longitude { get; set; }
        public string currentLatitude { get; set; }
        public string currentlongitude { get; set; }
        public string mobileDate { get; set; }
        public string MobileReferenceNo { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        public string TempShopId { get; set; }

        #endregion

        #region Methods
        public DataSet GetShopLocation()
        {
            return shopLocationDAO.GetShopLocation(this);
        }
        public void SaveShopLocation()
        {
            shopLocationDAO.SaveShopLocation(this);
        }
        #endregion
    }
}
