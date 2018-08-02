using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class SampleDAO
    {
        public int UpdateSampleRequest(SampleBO samplRequestBO)
        {
            try
            {
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(samplRequestBO.ConString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "uspUpdateSample";
                sqlHelper.AddParameter(command, "@UserId", samplRequestBO.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ShopId", samplRequestBO.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProductData", samplRequestBO.ProductData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@RequestedQty", samplRequestBO.RequestedQty, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@DeliveredQty", samplRequestBO.DeliveredQty, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@latitude", samplRequestBO.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@longitude", samplRequestBO.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@processName", samplRequestBO.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SubmissionDate", samplRequestBO.SubmissionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@MobileTransactionDate", samplRequestBO.mobileTransactionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SyncDate", samplRequestBO.SyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@mobileReferenceNo", samplRequestBO.mobileReferenceNo, ParameterDirection.Input);
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
