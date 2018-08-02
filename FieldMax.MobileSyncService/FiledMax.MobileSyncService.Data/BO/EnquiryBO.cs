using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public  class EnquiryBO:BOBase
   {
       EnquiryDAO enquiryDAO = new EnquiryDAO();
       #region Properties

       public int UserId { get; set; }
       public int EnquiryId { get; set; }
       public int ShopId { get; set; }
       public int ProductId { get; set; }    
       public DateTime EnquiredDate { get; set; }
       public int ActivityId { get; set; }
       public string Remarks { get; set; }
       public string Latitude { get; set; }
       public string Longitude { get; set; }
       public string ProcessName { get; set; }
       public string MobileTransactionDate { get; set; }
       public string MobileSyncDate { get; set; }
       public string MobileDate { get; set; }
       public string MobileReferenceNo { get; set; }
       public int GpsSource { get; set; }
       public string TempShopId { get; set; }
       public string networkProvider { get; set; }
       public string signalStrength { get; set; }
       #endregion

       #region Methods
       public int UpdateEnquiry()
       {
           return enquiryDAO.UpdateEnquiry(this);
       }
       public int getSMSAlertSetting()
       {
           return enquiryDAO.getSMSAlertSetting(this);
       }
       public DataSet getSMSData()
       {
           return enquiryDAO.getSMSData(this);
       }
       #endregion
   }
}
