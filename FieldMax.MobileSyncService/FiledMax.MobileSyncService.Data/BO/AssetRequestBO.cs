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
    public class AssetRequestBO : BOBase
    {
        public int UserId { get; set; }
        public int ShopId { get; set; }
        public int AssetNameId { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4{ get; set; }
        public string Field5{ get; set; }
        public int GpsSource{ get; set; }
        public string Latitude{ get; set; }
        public string Longitude{ get; set; }
        public string ProcessName{ get; set; }
        public string MobileDate { get; set; }
        public string MobileTransactionDate{ get; set; }
        public string MobileReferenceNo{ get; set; }
        public string Requesttype { get; set; }
        public string AssetNo { get; set; }

        AssetRequestDAO AssetRequestDAO = new AssetRequestDAO();

        public int UpdateAssetRequest()
        {
            return AssetRequestDAO.UpdateAssetRequest(this);
        }
    }
}
