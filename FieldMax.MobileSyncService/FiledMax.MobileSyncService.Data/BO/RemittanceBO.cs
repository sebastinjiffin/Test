using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class RemittanceBO : BOBase
    {
        #region Properties

        public int UserId { get; set; }
        public string Mode { get; set; }
        public string Amount { get; set; }
        public int BankId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string MobileTransactionDate { get; set; }
        public string MobRefNo { get; set; }
        public int GpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string DenominationIds { get; set; }
        public string Denominations { get; set; }
        public string ApprovedBy { get; set; }
        public string Remarks { get; set; }
        #endregion

        RemittanceDAO remittanceDAO = new RemittanceDAO();

        #region Methods

        public int UpdateRemittance()
        {
            return remittanceDAO.UpdateRemittance(this);
        }
        public string GetTodaysRemittance()
        {
            return remittanceDAO.GetTodaysRemittance(this);
        }

        #endregion

    }
}
