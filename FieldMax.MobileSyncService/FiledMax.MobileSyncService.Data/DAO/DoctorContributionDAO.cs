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
    public class DoctorContributionDAO
    {
        public int UpdateDoctorContribution(DoctorContributionBO doctorContributionBO)
        {
            try
            {
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(doctorContributionBO.ConString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "uspUpdateContribution";
                sqlHelper.AddParameter(command, "@UserId", doctorContributionBO.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ShopId", doctorContributionBO.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProductData", doctorContributionBO.ProductData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Qty", doctorContributionBO.Qty, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ContributedBy", doctorContributionBO.ContributedBy, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@latitude", doctorContributionBO.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@longitude", doctorContributionBO.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@processName", doctorContributionBO.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Date", doctorContributionBO.Date, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@MobileTransactionDate", doctorContributionBO.mobileTransactionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SyncDate", doctorContributionBO.SyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@mobileReferenceNo", doctorContributionBO.mobileReferenceNo, ParameterDirection.Input);
                return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            }
            catch (Exception e)
            {
                string ex = e.Message;

            }
            return 0;
        }
    }
}
