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
    public class AppointmentDAO
    {
        public int UpdateAppointment(AppointmentBO AppointmentBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(AppointmentBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_SaveAppointment";
            sqlHelper.AddParameter(command, "@userId", AppointmentBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@AppointmentDate", AppointmentBO.AppointmentDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Description", AppointmentBO.AppointmentDescription, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", AppointmentBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", AppointmentBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", AppointmentBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Source", AppointmentBO.Source, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileSyncDate", AppointmentBO.mobileSyncDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", AppointmentBO.MobReferenceNo, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));

        }
    }
}
