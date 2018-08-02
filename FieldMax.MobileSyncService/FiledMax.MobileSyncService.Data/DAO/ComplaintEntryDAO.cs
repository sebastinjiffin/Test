using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public  class ComplaintEntryDAO
    {
       internal int UpdateComplaint(ComplaintEntryBO complaintEntyryBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(complaintEntyryBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspComplain";
           sqlHelper.AddParameter(command, "@Mode", "2", ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ShopId", complaintEntyryBO.ShopId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@UserId", complaintEntyryBO.ReportedBy, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Remarks", complaintEntyryBO.Complaint, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ComplaintId", complaintEntyryBO.ComplaintId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@latitude", complaintEntyryBO.Latitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@longitude", complaintEntyryBO.Longitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@processName", complaintEntyryBO.ProcessName, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@mobileTransactionDate", complaintEntyryBO.mobileTransactionDate, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@mobRefNo", complaintEntyryBO.mobileReferenceNo, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@GpsSource", complaintEntyryBO.GpsSource, ParameterDirection.Input);

           if (complaintEntyryBO.MobileSyncDate != "" && complaintEntyryBO.MobileDate != "")
           {
               sqlHelper.AddParameter(command, "@MobileSyncDate", complaintEntyryBO.mobileDate, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ServerSyncDate", complaintEntyryBO.mobileSyncDate, ParameterDirection.Input);
           }
           if (complaintEntyryBO.signalStrength != null && complaintEntyryBO.signalStrength != string.Empty && complaintEntyryBO.signalStrength != "")
           {
               sqlHelper.AddParameter(command, "@signalStrength", complaintEntyryBO.signalStrength, ParameterDirection.Input);
           }
           if (complaintEntyryBO.networkProvider != null && complaintEntyryBO.networkProvider != string.Empty && complaintEntyryBO.networkProvider != "")
           {
               sqlHelper.AddParameter(command, "@networkProvider", complaintEntyryBO.networkProvider, ParameterDirection.Input);
           }
           return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
       }
        internal DataSet getComplaintInfo(ComplaintEntryBO complaintEntyryBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(complaintEntyryBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspComplain";
            sqlHelper.AddParameter(command, "@Mode", "3", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", complaintEntyryBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", complaintEntyryBO.ReportedBy, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ComplaintId", complaintEntyryBO.ComplaintId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobRefNo", complaintEntyryBO.mobileReferenceNo, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

    }
}
