using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class MyActivityBO:BOBase
    {
        #region Properties

        public string Mode { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public string MobileReferenceNo { get; set; }
        public int ActivityDeviationId { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public int gpsSource { get; set; }
        public string MobileSyncDate { get; set; }        
        public string MobileTransactionDate { get; set; }
        public string NetworkProvider { get; set; }
        public string signalStrength { get; set; }
        public string ConfigFieldIds { get; set; }
        public string ConfigFieldValues { get; set; }
        public string ActivityPlannedDate { get; set; }

        #endregion

        MyActivityDAO myActivityDAO = new MyActivityDAO();

        #region Methods

        public int UpdateActivity()
        {
            return myActivityDAO.UpdateActivity(this);
        }

        public int UpdateActivityLog()
        {
            return myActivityDAO.UpdateActivityLog(this);
        }
        #endregion
    }
}
