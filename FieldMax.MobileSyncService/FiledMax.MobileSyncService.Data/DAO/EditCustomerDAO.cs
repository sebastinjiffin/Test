using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class EditCustomerDAO
    {
        internal int updateCustomerDetails(EditCustomerBO editCustomerBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(editCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "[UpdateEditCustomerDetails]";
            sqlHelper.AddParameter(command, "@UserId", editCustomerBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", editCustomerBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ContactPersonName", editCustomerBO.ContactName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileNumber", editCustomerBO.MobileNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@PinCode", editCustomerBO.PinCode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Address", editCustomerBO.Address, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Landmark", editCustomerBO.Landmark, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", editCustomerBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", editCustomerBO.GpsSource, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Latitude", editCustomerBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Longitude", editCustomerBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProcessName", editCustomerBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopPotential", editCustomerBO.ShopPotential, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GstNumber", editCustomerBO.GstNumber, ParameterDirection.Input);
            if (editCustomerBO.MobileSyncDate != null)
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", editCustomerBO.MobileSyncDate, ParameterDirection.Input);
            }

            if (editCustomerBO.MobileReferenceNo != null && editCustomerBO.MobileReferenceNo != string.Empty && editCustomerBO.MobileReferenceNo != "")
            {
                sqlHelper.AddParameter(command, "@mobileRefNumber", editCustomerBO.MobileReferenceNo, ParameterDirection.Input);
            }

            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        public EditCustomerBO GetPendingApprovalsToReportingUser(EditCustomerBO editCustomerBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(editCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "[uspCustomerEditApproval]";
            sqlHelper.AddParameter(command, "@Mode", editCustomerBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", editCustomerBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", editCustomerBO.ShopId, ParameterDirection.Input);
            DataSet ds = sqlHelper.ExecuteDataSet(command);
            DataRow row = ds.Tables[0].Rows[0];
            decimal temp = 0.0M;

            editCustomerBO.ShopName = (row.Field<string>("ShopName") == null ? "" : row.Field<string>("ShopName"));
            editCustomerBO.ReportingUser = (row.Field<string>("ReportingUser") == null ? "" : row.Field<string>("ReportingUser"));
            editCustomerBO.RequestedUser = (row.Field<string>("RequestedUser") == null ? "" : row.Field<string>("RequestedUser"));
            editCustomerBO.UserEmail = (row.Field<string>("ToUserEmail") == null ? "" : row.Field<string>("ToUserEmail"));
            editCustomerBO.ContactName = (row.Field<string>("ContactPerson") == null ? "" : Convert.ToString(row.Field<string>("ContactPerson")));
            editCustomerBO.MobileNo = (row.Field<string>("MobileNo") == null ? "" : Convert.ToString(row.Field<string>("MobileNo")));
            editCustomerBO.PinCode = (row.Field<string>("PINCode") == null ? "" : Convert.ToString(row.Field<string>("PINCode")));
            editCustomerBO.Address = (row.Field<string>("Address") == null ? "" : Convert.ToString(row.Field<string>("Address")));
            editCustomerBO.StreetName = (row.Field<string>("StreetName") == null ? "" : Convert.ToString(row.Field<string>("StreetName")));
            editCustomerBO.ShopPotential = (row.Field<decimal?>("ShopPotential") == null ? temp : Convert.ToDecimal(row.Field<decimal>("ShopPotential")));
            editCustomerBO.GstNumber = (row.Field<string>("GSTIN") == null ? "" : Convert.ToString(row.Field<string>("GSTIN")));
            editCustomerBO.EditedContactName = (row.Field<string>("ContactName") == null ? "" : Convert.ToString(row.Field<string>("ContactName")));
            editCustomerBO.EditedMobile = (row.Field<string>("EditedMobile") == null ? "" : Convert.ToString(row.Field<string>("EditedMobile")));
            editCustomerBO.EditedPinCode = (row.Field<string>("EditedPinCode") == null ? "" : Convert.ToString(row.Field<string>("EditedPinCode")));
            editCustomerBO.EditedAddress = (row.Field<string>("EditedAddress") == null ? "" : Convert.ToString(row.Field<string>("EditedAddress")));
            editCustomerBO.Landmark = (row.Field<string>("Landmark") == null ? "" : Convert.ToString(row.Field<string>("Landmark")));
            editCustomerBO.IsEditCustomerMailReq = (row.Field<bool?>("IsEditCustomerMailReq") == null ? false : Convert.ToBoolean(row.Field<bool>("IsEditCustomerMailReq")));
            editCustomerBO.EditedShopPotential = (row.Field<decimal?>("EditedShopPotential") == null ? temp : Convert.ToDecimal(row.Field<decimal>("EditedShopPotential")));
            editCustomerBO.EditedGstNumber = (row.Field<string>("EditedGstNumber") == null ? "" : Convert.ToString(row.Field<string>("EditedGstNumber")));
            return editCustomerBO;
        }

        
    }
}
