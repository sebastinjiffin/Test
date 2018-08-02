using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ShopInAndOutDAO
    {

        internal int UpdateShopInAndOut(ShopInAndOutBO shopInAndOutBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopInAndOutBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "updateShopInAndOut";
            sqlHelper.AddParameter(command, "@ShopId", shopInAndOutBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", shopInAndOutBO.UserId, ParameterDirection.Input);
            if (shopInAndOutBO.ShopOut.Equals(""))
            {
                sqlHelper.AddParameter(command, "@ShopIn", shopInAndOutBO.ShopIn, ParameterDirection.Input);
            }
            else
            {
                sqlHelper.AddParameter(command, "@ShopOut", shopInAndOutBO.ShopOut, ParameterDirection.Input);
            }
            
            sqlHelper.AddParameter(command, "@Latitude", shopInAndOutBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Longitude", shopInAndOutBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProcessName", shopInAndOutBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", shopInAndOutBO.gpsSource, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileRefNo", shopInAndOutBO.MobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", shopInAndOutBO.MobileTransactionDate, ParameterDirection.Input);

            if (shopInAndOutBO.ServerSyncDate != "" && shopInAndOutBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", shopInAndOutBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", shopInAndOutBO.ServerSyncDate, ParameterDirection.Input);
            }

            if (shopInAndOutBO.signalStrength != null && shopInAndOutBO.signalStrength != string.Empty && shopInAndOutBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", shopInAndOutBO.signalStrength, ParameterDirection.Input);
            }
            if (shopInAndOutBO.networkProvider != null && shopInAndOutBO.networkProvider != string.Empty && shopInAndOutBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", shopInAndOutBO.networkProvider, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(shopInAndOutBO.IsGpsForciblyEnabled))
            {
                sqlHelper.AddParameter(command, "@IsGpsForciblyEnabled", shopInAndOutBO.IsGpsForciblyEnabled, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
