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
    public class StockDAO
    {
        internal int UpdateStock(StockBO stockBO)
        {
            try
            {
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockBO.ConString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "StockCapture";
                sqlHelper.AddParameter(command, "@UserId", stockBO.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ShopID", stockBO.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProductData", stockBO.ProductData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@quantitydata", stockBO.StockData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@UnitData", stockBO.UnitData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@latitude", stockBO.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@longitude", stockBO.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@processName", stockBO.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@mobileTransactionDate", stockBO.mobileTransactionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SyncDate", stockBO.SyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@GpsSource", stockBO.GpsSource, ParameterDirection.Input);
                if (stockBO.MobileReferenceNo != string.Empty)
                {
                    sqlHelper.AddParameter(command, "@mobileReferenceNo", stockBO.MobileReferenceNo, ParameterDirection.Input);
                }
                if (stockBO.mobileDate != null && stockBO.mobileDate != string.Empty && stockBO.mobileDate != "")
                {
                    sqlHelper.AddParameter(command, "@mobileSyncDate", stockBO.mobileDate, ParameterDirection.Input);
                }
                if (stockBO.signalStrength != null && stockBO.signalStrength != string.Empty && stockBO.signalStrength != "")
                {
                    sqlHelper.AddParameter(command, "@signalStrength", stockBO.signalStrength, ParameterDirection.Input);
                }
                if (stockBO.networkProvider != null && stockBO.networkProvider != string.Empty && stockBO.networkProvider != "")
                {
                    sqlHelper.AddParameter(command, "@networkProvider", stockBO.networkProvider, ParameterDirection.Input);
                }
                return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            }
            catch (Exception e)
            {
                string ex = e.Message;

            }
            return 0;
        }


        internal int UpdateVanStockRequest(StockBO stockBO)
        {
            try
            {
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockBO.ConString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "UpdateVanStockRequest";
                sqlHelper.AddParameter(command, "@UserId", stockBO.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@StoreId", stockBO.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@StockStatus", 0, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProductData", stockBO.ProductData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@StockData", stockBO.StockData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@UnitData", stockBO.UnitData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Latitude", stockBO.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Longitude", stockBO.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProcessName", stockBO.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@MobileTransactionDate", stockBO.mobileTransactionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SyncDate", stockBO.SyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@GpsSource", stockBO.GpsSource, ParameterDirection.Input);
                if (stockBO.MobileReferenceNo != string.Empty)
                {
                    sqlHelper.AddParameter(command, "@MobileReferenceNo", stockBO.MobileReferenceNo, ParameterDirection.Input);
                }
                return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            }
            catch (Exception e)
            {
                string ex = e.Message;

            }
            return 0;
        }

        internal int GetUserStoreId(StockBO stockBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            try
            {
                command.CommandText = "SELECT st.storeid from store st INNER JOIN Organization org ON org.organizationId = st.organizationId INNER JOIN UserOrganization uo ON uo.organizationid = org.organizationid and uo.userid = '" + stockBO.UserId + "'";
                return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        internal int UpdateRecievedStock(StockBO stockBO)
        {
            
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockBO.ConString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "UpdateReceivedQuantityPharma";
                sqlHelper.AddParameter(command, "@UserId", stockBO.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ShopID", stockBO.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProductData", stockBO.ProductData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@quantitydata", stockBO.StockData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@UnitData", stockBO.UnitData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@latitude", stockBO.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@longitude", stockBO.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@processName", stockBO.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@mobileTransactionDate", stockBO.mobileTransactionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SyncDate", stockBO.SyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@GpsSource", stockBO.GpsSource, ParameterDirection.Input);
                if (stockBO.MobileReferenceNo != string.Empty)
                {
                    sqlHelper.AddParameter(command, "@mobileReferenceNo", stockBO.MobileReferenceNo, ParameterDirection.Input);
                }
                if (stockBO.mobileDate != null && stockBO.mobileDate != string.Empty && stockBO.mobileDate != "")
                {
                    sqlHelper.AddParameter(command, "@mobileSyncDate", stockBO.mobileDate, ParameterDirection.Input);
                }
                return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            
           
        }
        internal int UpdateLoadedStock(StockBO stockBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspStock";
            sqlHelper.AddParameter(command, "@Mode", stockBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", stockBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProductData", stockBO.ProductData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StockData", stockBO.StockData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UnitData", stockBO.UnitData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StockHeaderId", stockBO.StockHeaderId, ParameterDirection.Input);

            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));


        }


        internal DataSet GetFinalSettlementDetails(StockBO stockBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(stockBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspFinancialClosure";
            sqlHelper.AddParameter(command, "@Mode", stockBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", stockBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StoreId", stockBO.StoreId, ParameterDirection.Input);

            return sqlHelper.ExecuteDataSet(command);
             
        }
         
    }
}
