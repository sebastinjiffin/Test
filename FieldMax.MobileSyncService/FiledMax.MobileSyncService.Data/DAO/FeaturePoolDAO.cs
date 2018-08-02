using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class FeaturePoolDAO
    {
        internal int UpdateFeaturePoolData(FieldMax.MobileSyncService.Data.BO.FeaturePoolBO featurePoolBo)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(featurePoolBo.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspFeaturePooldataEntry";
            sqlHelper.AddParameter(command, "@UserId", featurePoolBo.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", featurePoolBo.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@FeaturePoolMasterDataId", featurePoolBo.FeaturePoolMasterDataId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@FeaturePoolMasterDetailsId", featurePoolBo.FeaturePoolMasterDetailsId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@FeaturePoolCaptureData", featurePoolBo.FeaturePoolCaptureData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", featurePoolBo.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", featurePoolBo.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", featurePoolBo.MobileTransactinDate.ToString(), ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", featurePoolBo.MobileTransactinDate.ToString(), ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", featurePoolBo.MobileReferenceNo, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            
        }

    }
}
