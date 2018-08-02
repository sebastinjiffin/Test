using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class BTLActivityBO: BOBase
    {
        BTLActivityDAO btlActivityDAO = new BTLActivityDAO();

        #region Properties

        public string OrganizationId { get; set; }
        public string Mode { get; set; }
        public int UserId { get; set; }
        public string BTLActivityId { get; set; }              
        public string Longitude { get; set; }
        public string Latitude { get; set; }        
        public string ConfigFieldIds { get; set; }
        public string ConfigFieldValues { get; set; }
        public string MobileReferenceNo { get; set; }
        public string GpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string Attendees { get; set; }
        public string MobileTransactionDate { get; set; }
        #endregion

        #region Methods

        public int UpdateBTLDetails()
        {
            return btlActivityDAO.UpdateBTLDetails(this);
        }
        #endregion
    }
}
