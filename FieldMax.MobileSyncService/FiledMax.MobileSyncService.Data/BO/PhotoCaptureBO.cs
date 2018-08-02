using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public class PhotoCaptureBO:BOBase
    {
       PhotoCaptureDAO photoCaptureDAO = new PhotoCaptureDAO();
        #region Properties

        public int UserId { get; set; }        
        public string ShopId { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string ImageDescription { get; set; }
        public string DateTime { get; set; }
        public string emails { get; set; }
        public string mobReferenceNumber { get; set; }
        public string imageData { get; set; }
        public int mode { get; set; }
        public int descriptionTypeId { get; set; }
        public int ProcessId { get; set; }
        public int ProcessDetailsId { get; set; }
        public string SyncDate { get; set; }
        public int GpsSource { get; set; }
        public string TempShopId { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        #endregion

        #region Methods

        public int UpdatePhotoCapture()
        {
            return photoCaptureDAO.UpdatePhotoCapture(this);
        }

        public DataSet GetImagePath()
        {
            return photoCaptureDAO.GetImagePath(this);
        }
        #endregion 


    }
}
