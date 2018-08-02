using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class QuatationDetailsDAO
    {
        internal SqlDataReader getShopDetails(QuatationDetailsBO boObject)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(boObject.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT name,address from shop where shopid = "+boObject.shopId;
            return sqlHelper.ExecuteReader(command);
        }

        internal SqlDataReader getNewCustomerDetails(QuatationDetailsBO boObject)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(boObject.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT CustomerName, address from NewCustomer where TempMShopId like  '" + boObject.TempShopId + "'";
            return sqlHelper.ExecuteReader(command);
        }

        internal SqlDataReader getUserDetails(QuatationDetailsBO boObject)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(boObject.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT u.name,u.address,ur.userRole from [user] u INNER JOIN UserRole ur ON u.userRoleId = ur.userRoleId where userId =" + boObject.userId;
            return sqlHelper.ExecuteReader(command);
        }

        internal string saveQuatation(QuatationDetailsBO boObject)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(boObject.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "QuotationEntry";
            sqlHelper.AddParameter(command, "@UserId", boObject.userId, ParameterDirection.Input);
            if (boObject.shopId != null && boObject.shopId.ToString() != string.Empty && boObject.shopId != 0)
            {
                sqlHelper.AddParameter(command, "@ShopID", boObject.shopId, ParameterDirection.Input);
            }
            
            sqlHelper.AddParameter(command, "@ProductData", boObject.productData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@QuantityData", boObject.quantityData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RateData", boObject.rateData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", boObject.lattitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", boObject.longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", boObject.processName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@TotalAmount", boObject.totalAmount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@EmailAddress", boObject.emailAddress, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", boObject.mobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobRefNo", boObject.mobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@TempShopId", boObject.TempShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", boObject.GpsSource, ParameterDirection.Input);
            if (!boObject.SyncDate.Equals(string.Empty))
            {
                sqlHelper.AddParameter(command, "@SyncDate", boObject.SyncDate, ParameterDirection.Input);
            }
            if (boObject.mobileDate != null && !boObject.MobileSyncDate.Equals(string.Empty))
            {
                sqlHelper.AddParameter(command, "@mobileSyncDate", boObject.MobileSyncDate, ParameterDirection.Input);
            }
            if (boObject.mobileDate != null && boObject.mobileDate != string.Empty && boObject.mobileDate != "")
            {
                sqlHelper.AddParameter(command, "@mobileSyncDate", boObject.mobileDate, ParameterDirection.Input);
            }
            if (boObject.signalStrength != null && boObject.signalStrength != string.Empty && boObject.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", boObject.signalStrength, ParameterDirection.Input);
            }
            if (boObject.networkProvider != null && boObject.networkProvider != string.Empty && boObject.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", boObject.networkProvider, ParameterDirection.Input);
            }
            SqlParameter outparam = command.Parameters.Add("@quotationNo", SqlDbType.VarChar);
            outparam.Size = 50;
            outparam.Direction = ParameterDirection.Output;
            sqlHelper.ExecuteNonQuery(command);
            string result = command.Parameters["@quotationNo"].Value.ToString();
            //Convert.ToInt32(sqlHelper.ExecuteScalar(command));
            //var result = returnParameter.Value;
            return result;

        }

        internal string getHeaderNameForPDF(QuatationDetailsBO boObject, string productId)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(boObject.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "Select Classification from Classification where ClassificationId = (Select ClassificationId from ProductAttribute where ProductAttributeId = " + productId + ")";
            return Convert.ToString(sqlHelper.ExecuteScalar(command));
        }

        internal int isCustomizedImageNeeded(QuatationDetailsBO boObject)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(boObject.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT status FROM MobilePrivilegeControls where ControlId = (select ControlId from MobileControl where ControlName like '%btnLogoForQuotationPDF%')";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }
    }
}
