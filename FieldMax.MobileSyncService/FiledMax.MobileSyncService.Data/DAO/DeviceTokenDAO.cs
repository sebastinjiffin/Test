using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class DeviceTokenDAO
    {
       internal int UpdateDeviceToken(DeviceTokenBO deviceBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(deviceBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "InsertToDeviceToken";
           sqlHelper.AddParameter(command, "@userId", deviceBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@deviceToken", deviceBO.DeviceId, ParameterDirection.Input);          
           return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
       }

       internal DataSet GetDeviceId(DeviceTokenBO deviceBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(deviceBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "ReadFromDeviceToken";
           sqlHelper.AddParameter(command, "@userId", deviceBO.UserId, ParameterDirection.Input);
           return sqlHelper.ExecuteDataSet(command);
       }
    }
}
