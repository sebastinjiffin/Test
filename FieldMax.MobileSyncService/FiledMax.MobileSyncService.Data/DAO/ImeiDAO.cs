using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ImeiDAO
    {
        internal int UpdateImei(ImeiBO imeiBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(imeiBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspImeiLog";
            sqlHelper.AddParameter(command, "@userId", imeiBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@imeiNo", imeiBO.Imei, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@androidVersionNo", imeiBO.AndroidVersion, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
        internal int LogImei(ImeiBO imeiBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(imeiBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "InsertUserLog";
            sqlHelper.AddParameter(command, "@UserId", imeiBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@IMEI", imeiBO.Imei, ParameterDirection.Input);
            if (imeiBO.SignalStrength != 0)
            {
                sqlHelper.AddParameter(command, "@SignalStrength", imeiBO.SignalStrength, ParameterDirection.Input);
            }
            if (imeiBO.networkProvider != null && imeiBO.networkProvider != string.Empty && imeiBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", imeiBO.networkProvider, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));

        }
    }
}
