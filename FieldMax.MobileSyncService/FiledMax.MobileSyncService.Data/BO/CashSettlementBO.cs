using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;


namespace FieldMax.MobileSyncService.Data.BO
{
    public class CashSettlementBO : BOBase 
    {
        #region Properties

        public string Name { get; set; }
        public string UserId { get; set; }
        public string Cash { get; set; }
        public string PaymentHeaderId { get; set; }
        public string ChequeAmount { get; set; }
        public string Mode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string MobileTransactionDate { get; set; }
        public string MobileReferenceNo { get; set; }
        public string gpsSource { get; set; }
        public string RemittedBy { get; set; }

        public string SignalStrength { get; set; }
        public string NetworkProvider { get; set; }

        public string InstrumentNo { get; set; }


        #endregion

        CashSettlementDAO cashSettlementDAO = new CashSettlementDAO();

        public int UpdateCashSettlementUser()
        {
            return cashSettlementDAO.UpdateCashSettlementUser(this);
        }


        public string MobileSyncDate { get; set; }

        public string ServerSyncDate { get; set; }
    }
}
