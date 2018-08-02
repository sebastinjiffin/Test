using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class StockReconcilationDAO
    {
        internal int UpdateStockReconcilation(StockReconcilationBO stockReconcilationBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockReconcilationBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "UpdateStockReconcile";
            sqlHelper.AddParameter(command, "@UserId", stockReconcilationBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StoreId", stockReconcilationBO.StoreId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ReconcileDate", stockReconcilationBO.ReconcileDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProductIdData", stockReconcilationBO.ProductIdData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@QuantityData", stockReconcilationBO.QuantityData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ReturnReasonId", stockReconcilationBO.ReasonIdData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RateData", stockReconcilationBO.RateData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", stockReconcilationBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", stockReconcilationBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", stockReconcilationBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UnloadStatus", stockReconcilationBO.UnloadStatus, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", stockReconcilationBO.GpsSource, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UpdatedDate", stockReconcilationBO.ReconcileDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UnitData", stockReconcilationBO.UnitData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Remarks", stockReconcilationBO.Remarks, ParameterDirection.Input);
            if (stockReconcilationBO.SyncDate != null)
            {
                sqlHelper.AddParameter(command, "@SyncDate", stockReconcilationBO.SyncDate, ParameterDirection.Input);
            }
            if (stockReconcilationBO.mobileDate != null && stockReconcilationBO.mobileDate != string.Empty && stockReconcilationBO.mobileDate != "")
            {
                sqlHelper.AddParameter(command, "@mobileSyncDate", stockReconcilationBO.mobileDate, ParameterDirection.Input);
            }
            if (stockReconcilationBO.MobileRefNo != null && stockReconcilationBO.MobileRefNo != string.Empty && stockReconcilationBO.MobileRefNo != "")
            {
                sqlHelper.AddParameter(command, "@mobileRefNumber", stockReconcilationBO.MobileRefNo, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal int UpdateVanUnloadStatus(StockReconcilationBO stockReconcilationBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockReconcilationBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "UpdateVanUnloadStatus";
            sqlHelper.AddParameter(command, "@Id", stockReconcilationBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StoreId", stockReconcilationBO.StoreId, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal DataSet GetVanUnloadDetails(StockReconcilationBO stockReconcilationBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockReconcilationBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetVanUnloadDetailsAndroid";
            sqlHelper.AddParameter(command, "@StoreId", stockReconcilationBO.StoreId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }


    }
}
