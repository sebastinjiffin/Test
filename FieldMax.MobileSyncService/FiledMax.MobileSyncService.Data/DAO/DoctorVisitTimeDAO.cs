using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class DoctorVisitTimeDAO
    {
        internal int UpdateDoctorVisitTime(DoctorVisitTimeBO doctorVisitTimeBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(doctorVisitTimeBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);

            command.CommandText = "UpdateDoctorVisitTimeAndroid";
            sqlHelper.AddParameter(command, "@HospitalVisitId", doctorVisitTimeBO.HospitalVisitId, ParameterDirection.Input);
            if (!doctorVisitTimeBO.VisitDay.Equals("0"))
            {
                sqlHelper.AddParameter(command, "@VisitDay", doctorVisitTimeBO.VisitDay, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@FromTime", doctorVisitTimeBO.FromTime, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ToTime", doctorVisitTimeBO.ToTime, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal int UpdatePreferredVisitTime(DoctorVisitTimeBO doctorVisitTimeBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(doctorVisitTimeBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);

            command.CommandText = "UpdatePreferredVisitTimeAndroid";
            sqlHelper.AddParameter(command, "@DoctorId", doctorVisitTimeBO.DoctorId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@VisitDay", doctorVisitTimeBO.VisitDay, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@FromTime", doctorVisitTimeBO.FromTime, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ToTime", doctorVisitTimeBO.ToTime, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
