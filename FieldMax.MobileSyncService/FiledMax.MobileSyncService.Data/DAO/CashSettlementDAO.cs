using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;


namespace FieldMax.MobileSyncService.Data.DAO
{
    public class CashSettlementDAO
    {
        internal int UpdateCashSettlementUser(CashSettlementBO cashSettlementBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(cashSettlementBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspCashSettlement";
            sqlHelper.AddParameter(command, "@Mode", cashSettlementBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", cashSettlementBO.UserId, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(cashSettlementBO.PaymentHeaderId))
            {
                sqlHelper.AddParameter(command, "@PaymentHeaderId", cashSettlementBO.PaymentHeaderId, ParameterDirection.Input);
            }

            if (!String.IsNullOrEmpty(cashSettlementBO.Cash))
            {
                sqlHelper.AddParameter(command, "@CashAmount", cashSettlementBO.Cash, ParameterDirection.Input);
            }
            else
            {
                sqlHelper.AddParameter(command, "@CashAmount", "0", ParameterDirection.Input);
            }

            sqlHelper.AddParameter(command, "@MobileReferenceNo", cashSettlementBO.MobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", cashSettlementBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", cashSettlementBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", cashSettlementBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", cashSettlementBO.gpsSource, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(cashSettlementBO.SignalStrength))
            {
                sqlHelper.AddParameter(command, "@SignalStrength", cashSettlementBO.SignalStrength, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(cashSettlementBO.NetworkProvider))
            {
                sqlHelper.AddParameter(command, "@NetworkProvider", cashSettlementBO.NetworkProvider, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@InstrumentNo", cashSettlementBO.InstrumentNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RemittedBy", cashSettlementBO.RemittedBy, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(cashSettlementBO.ServerSyncDate))
            {
                sqlHelper.AddParameter(command, "@ServerSyncDate", cashSettlementBO.ServerSyncDate, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(cashSettlementBO.MobileSyncDate))
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", cashSettlementBO.MobileSyncDate, ParameterDirection.Input);
            }

            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

         
    }
}
