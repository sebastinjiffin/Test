using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class FeaturePoolBO:BOBase
    {
        FeaturePoolDAO featurePoolDao = new FeaturePoolDAO();

        #region Properties

        public int UserId { get; set; }
        public int ShopId { get; set; }
        public string FeaturePoolMasterDataId { get; set; }
        public string FeaturePoolMasterDetailsId { get; set; }
        public string FeaturePoolCaptureData { get; set; }
        public DateTime? MobileTransactinDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public int GpsSource { get; set; }
        public string MobileReferenceNo { get; set; }
        #endregion


        #region Methods
        public int UpdateFeaturePoolData()
        {
            return featurePoolDao.UpdateFeaturePoolData(this);
        }

      
        #endregion
        
    }
}
