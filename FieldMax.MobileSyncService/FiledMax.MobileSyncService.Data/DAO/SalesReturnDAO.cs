using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class SalesReturnDAO //: DAOBase
    {
        internal int GetSalesReturn(SalesReturnBO salesReturnBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(salesReturnBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetPreviousDayOrdersCount";
            sqlHelper.AddParameter(command, "@UserId", salesReturnBO.UserId, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int UpdateSalesReturn(SalesReturnBO salesReturnBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(salesReturnBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspSalesReturn";
            sqlHelper.AddParameter(command, "@Mode", salesReturnBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ReturnedBy", salesReturnBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopID", salesReturnBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProductAttributeId", salesReturnBO.ProductAttributeId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Quantity", salesReturnBO.Quantity, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Amount", salesReturnBO.Amount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@BatchNo", salesReturnBO.BatchNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@unitId", salesReturnBO.UnitId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ReceiptNo", salesReturnBO.ReceiptNo, ParameterDirection.Input);
            if (salesReturnBO.PkdDate != "")
            {
                sqlHelper.AddParameter(command, "@PkdDate", salesReturnBO.PkdDate, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@latitude", salesReturnBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", salesReturnBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", salesReturnBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", salesReturnBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Rate", salesReturnBO.Rate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", salesReturnBO.GpsSouce, ParameterDirection.Input);
            if (salesReturnBO.returnReason != string.Empty)
            {
                sqlHelper.AddParameter(command, "@ReturnReason", salesReturnBO.returnReason, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@mobileReferenceNo", salesReturnBO.mobileReferenceNo, ParameterDirection.Input);
            if (salesReturnBO.MobileSyncDate != "" && salesReturnBO.ServerSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", salesReturnBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", salesReturnBO.ServerSyncDate, ParameterDirection.Input);
            }
            if (salesReturnBO.SchemeId != 0)
            {
                sqlHelper.AddParameter(command, "@SchemeId", salesReturnBO.SchemeId, ParameterDirection.Input);
            }
            if (salesReturnBO.signalStrength != null && salesReturnBO.signalStrength != string.Empty && salesReturnBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", salesReturnBO.signalStrength, ParameterDirection.Input);
            }
            if (salesReturnBO.networkProvider != null && salesReturnBO.networkProvider != string.Empty && salesReturnBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", salesReturnBO.networkProvider, ParameterDirection.Input);
            }
            if (salesReturnBO.Remark != null && salesReturnBO.Remark != string.Empty && salesReturnBO.Remark != "")
            {
                sqlHelper.AddParameter(command, "@ReasonData", salesReturnBO.Remark, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
