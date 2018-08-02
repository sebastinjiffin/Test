using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System.Data.SqlClient;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class SMSconfigDAO
    {
        public DataSet getSmsConfiguration(SMSConfigBO smsConfigBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(smsConfigBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_getSmsConfiguation";
            if (smsConfigBO.ProcessName != null && smsConfigBO.ProcessName != string.Empty && smsConfigBO.ProcessName != "")
            {
                sqlHelper.AddParameter(command, "@Process", smsConfigBO.ProcessName, ParameterDirection.Input);
            }
            if (smsConfigBO.ActivityId != null && smsConfigBO.ActivityId != string.Empty && smsConfigBO.ActivityId != "")
            {
                sqlHelper.AddParameter(command, "@ActivityId", smsConfigBO.ActivityId, ParameterDirection.Input);
            }            
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
