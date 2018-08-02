using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public  class ComplaintEntryBO:BOBase
   {
       ComplaintEntryDAO complaintEntryDAO = new ComplaintEntryDAO();

       #region Properties
       public int ComplaintEntryId { get; set; }
       public int ShopId { get; set; }
       public int ReportedBy { get; set; }
       public DateTime ReportedDate { get; set; }
       public string Complaint { get; set; }
       public int ComplaintId { get; set; }
       public string Latitude { get; set; }
       public string Longitude { get; set; }
       public string ProcessName { get; set; }
       public string mobileTransactionDate { get; set; }
       public string mobileSyncDate { get; set; }
       public string mobileDate { get; set; }
       public string mobileReferenceNo { get; set; }
       public int GpsSource { get; set; }
       public string MobileSyncDate { get; set; }
       public string MobileDate { get; set; }
       public string networkProvider { get; set; }
       public string signalStrength { get; set; }
       #endregion

    #region Methods
           public int UpdateComplaint()
           {
               return complaintEntryDAO.UpdateComplaint(this);
           }
        public DataSet getComplaintInfo()
        {
            return complaintEntryDAO.getComplaintInfo(this);
        }
        #endregion

    }
}
