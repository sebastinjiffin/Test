using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class PopDAO
    {
        internal int updatePop(PopBO popBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(popBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspPopEntry";
            sqlHelper.AddParameter(command, "@UserId", popBO.userId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", popBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Parameters", popBO.popIdSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@QuantityList", popBO.quantitySet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProcessName", popBO.processName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Latitude", popBO.lattitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Longitude", popBO.longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", popBO.shopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", popBO.mobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobRefNo", popBO.mobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@remarkSet", popBO.remarkSet, ParameterDirection.Input);
            if (popBO.ServerSyncDate != "" && popBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", popBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", popBO.ServerSyncDate, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
