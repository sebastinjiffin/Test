using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FieldMax.MobileSyncService.Data.BO;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class EditPlanDAO
    {
        public DataSet GetAssignedBeats(EditPlanBO editPlanBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(editPlanBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_GetAssignedBeats";
            sqlHelper.AddParameter(command, "@UserId", editPlanBO.UserId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
        public DataSet GetCustomerForBeat(EditPlanBO editPlanBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(editPlanBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_GetCustomerForBeat";
            sqlHelper.AddParameter(command, "@BeatPalnId", editPlanBO.BeatPlanId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@date", editPlanBO.DateSet, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        public void UpdateEditPlan(EditPlanBO editPlanBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(editPlanBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_UpdateEditplan";
            sqlHelper.AddParameter(command, "@CustomerIdSet", editPlanBO.CustomerIdSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@DateSet", editPlanBO.DateSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StatusSet", editPlanBO.StatusSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserIdSet", editPlanBO.UserIdSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ApprovedDateSet", editPlanBO.ApprovedDateSet, ParameterDirection.Input);
            sqlHelper.ExecuteDataSet(command);
        }
    }
}
