using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using Infocean.DataAccessHelper;
using System.Data;
using System.Data.SqlClient;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class JourneyCycleDAO
    {
        internal DataSet GetJourneyCycle(JourneyCycleBO journeyCycleBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(journeyCycleBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetDateRangePharmaAndroid";
            sqlHelper.AddParameter(command, "@UserId", journeyCycleBO.UserId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
