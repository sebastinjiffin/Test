using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class ShopLocationDAO 
    {
        public DataSet GetShopLocation(ShopLocationBO shopLocationBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopLocationBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "mdr_getShopLocationsForAndroid";
            sqlHelper.AddParameter(command, "@UserId", shopLocationBO.UserId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal void SaveShopLocation(ShopLocationBO shopLocationBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopLocationBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateShopLocation";
            sqlHelper.AddParameter(command, "@UserId", shopLocationBO.UserId, ParameterDirection.Input);
            
            sqlHelper.AddParameter(command, "@lat", shopLocationBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@long", shopLocationBO.longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@currrentLat", shopLocationBO.currentLatitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@currrentLong", shopLocationBO.currentlongitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", shopLocationBO.GPSSource, ParameterDirection.Input);
            if (shopLocationBO.ShopId != null && shopLocationBO.ShopId != 0)
            {
                sqlHelper.AddParameter(command, "@ShopId", shopLocationBO.ShopId, ParameterDirection.Input);
            }
            if (shopLocationBO.mobileDate != null && shopLocationBO.mobileDate != string.Empty && shopLocationBO.mobileDate != "")
            {
                sqlHelper.AddParameter(command, "@mobileSyncDate", shopLocationBO.mobileDate, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileRefNo", shopLocationBO.MobileReferenceNo, ParameterDirection.Input);

            if (shopLocationBO.signalStrength != null && shopLocationBO.signalStrength != string.Empty && shopLocationBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", shopLocationBO.signalStrength, ParameterDirection.Input);
            }
            if (shopLocationBO.networkProvider != null && shopLocationBO.networkProvider != string.Empty && shopLocationBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", shopLocationBO.networkProvider, ParameterDirection.Input);
            }
            if (shopLocationBO.TempShopId != null && shopLocationBO.TempShopId != string.Empty && shopLocationBO.TempShopId != "")
            {
                sqlHelper.AddParameter(command, "@TempShopId", shopLocationBO.TempShopId, ParameterDirection.Input);
            }
            sqlHelper.ExecuteDataSet(command);
        }
    }
}
