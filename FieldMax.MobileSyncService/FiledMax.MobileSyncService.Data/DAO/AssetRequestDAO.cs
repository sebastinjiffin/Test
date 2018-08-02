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
     public class AssetRequestDAO
    {
         internal int UpdateAssetRequest(AssetRequestBO assetRequestBO)
         {
             DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(assetRequestBO.ConString);
             SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
             command.CommandText = "UpdateAssetRequest";
             sqlHelper.AddParameter(command, "@UserId", assetRequestBO.UserId, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@ShopId", assetRequestBO.ShopId, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@AssetNameId", assetRequestBO.AssetNameId, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Field1", assetRequestBO.Field1, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Field2", assetRequestBO.Field2, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Field3", assetRequestBO.Field3, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Field4", assetRequestBO.Field4, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Field5", assetRequestBO.Field5, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@GpsSource", assetRequestBO.GpsSource, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Latitude", assetRequestBO.Latitude, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Longitude", assetRequestBO.Longitude, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@ProcessName", assetRequestBO.ProcessName, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@MobileReferenceNo", assetRequestBO.MobileReferenceNo, ParameterDirection.Input);
            
             if (assetRequestBO.AssetNo != null)
             {
                 sqlHelper.AddParameter(command, "@AssetNo", assetRequestBO.AssetNo, ParameterDirection.Input);
             }
             if (assetRequestBO.Requesttype != null)
             {
                 sqlHelper.AddParameter(command, "@Requesttype", assetRequestBO.Requesttype, ParameterDirection.Input);
             }
             if (assetRequestBO.MobileTransactionDate != null)
             {
                 sqlHelper.AddParameter(command, "@MobileTransactionDate", assetRequestBO.MobileTransactionDate, ParameterDirection.Input);
             }
             if (assetRequestBO.MobileDate != null)
             {
                 sqlHelper.AddParameter(command, "@MobileSyncDate", assetRequestBO.MobileDate, ParameterDirection.Input);
             }
             return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
         }
    }
}
