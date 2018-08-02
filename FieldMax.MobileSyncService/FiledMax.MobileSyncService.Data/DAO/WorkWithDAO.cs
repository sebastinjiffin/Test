using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class WorkWithDAO
    {
        public DataSet GetWorkWithMasters(WorkWithBO workWithBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(workWithBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspMobileWorkingWith";
            sqlHelper.AddParameter(command, "@UserId", workWithBO.userId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", 1, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal int updateWorkWith(WorkWithBO workWithBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(workWithBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspMobileWorkingWith";
            sqlHelper.AddParameter(command, "@Mode", "2", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", workWithBO.userId, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(workWithBO.userIdSet))
            {
                sqlHelper.AddParameter(command, "@UserIdSet", workWithBO.userIdSet, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileTransactionDate", workWithBO.mobileCaptureDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProcessName", workWithBO.processName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Latitude", workWithBO.lattitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Longitude", workWithBO.longitude, ParameterDirection.Input);
            if (workWithBO.shopId.Trim().Length > 0)
            {
                sqlHelper.AddParameter(command, "@ShopId", workWithBO.shopId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@mobRefNo", workWithBO.mobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", workWithBO.gpsSource, ParameterDirection.Input);
            if (workWithBO.ServerSyncDate != "" && workWithBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", workWithBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", workWithBO.ServerSyncDate, ParameterDirection.Input);
            }
            if (workWithBO.signalStrength != null && workWithBO.signalStrength != string.Empty && workWithBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", workWithBO.signalStrength, ParameterDirection.Input);
            }
            if (workWithBO.networkProvider != null && workWithBO.networkProvider != string.Empty && workWithBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", workWithBO.networkProvider, ParameterDirection.Input);
            }
            if (workWithBO.Others != null && workWithBO.Others != string.Empty && workWithBO.Others != "")
            {
                sqlHelper.AddParameter(command, "@othersSet", workWithBO.Others, ParameterDirection.Input);
            }
            if (workWithBO.DepartmentIdSet != null && workWithBO.DepartmentIdSet != string.Empty && workWithBO.DepartmentIdSet != "")
            {
                sqlHelper.AddParameter(command, "@departmentSet", workWithBO.DepartmentIdSet, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
