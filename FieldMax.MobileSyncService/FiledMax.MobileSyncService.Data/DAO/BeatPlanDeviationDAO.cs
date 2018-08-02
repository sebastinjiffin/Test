using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class BeatPlanDeviationDAO
    {
        internal int UpdateBeatPlanDeviation(BeatPlanDeviationBO beatPlanDeviationBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(beatPlanDeviationBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateBeatPlanDeviation";
            sqlHelper.AddParameter(command, "@BeatPlanId", beatPlanDeviationBO.BeatPlanId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", beatPlanDeviationBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", beatPlanDeviationBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", beatPlanDeviationBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileRefNo", beatPlanDeviationBO.MobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", beatPlanDeviationBO.source, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@DeviationReasonId", beatPlanDeviationBO.DeviationReasonId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", beatPlanDeviationBO.MobileTransactionDate, ParameterDirection.Input);

            if (!string.IsNullOrEmpty(beatPlanDeviationBO.SyncDate))
            {
                sqlHelper.AddParameter(command, "@SyncDate", beatPlanDeviationBO.SyncDate, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(beatPlanDeviationBO.signalStrength))
            {
                sqlHelper.AddParameter(command, "@signalStrength", beatPlanDeviationBO.signalStrength, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));

        }
    }
}
