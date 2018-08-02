using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;
using System.Globalization;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class NewCustomerDAO //: DAOBase
    {
        internal int GetNewCustomer(NewCustomerBO newCustomerBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(newCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetPreviousDayOrdersCount";
            sqlHelper.AddParameter(command, "@UserId", newCustomerBO.reportedBy, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderDate", newCustomerBO.reportedDate, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal DataSet GetSMSDetails(NewCustomerBO newCustomerBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(newCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetMailId";
            sqlHelper.AddParameter(command, "@DistributorId", newCustomerBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", newCustomerBO.Mode, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet UpdateNewCustomer(NewCustomerBO newCustomerBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(newCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspNewCustomer";
            if (newCustomerBO.NewCustomerConfig == "1")
            {
                sqlHelper.AddParameter(command, "@Mode", "1", ParameterDirection.Input);
            }
            else
            {
                sqlHelper.AddParameter(command, "@Mode", "6", ParameterDirection.Input);
            }
            //sqlHelper.AddParameter(command, "@OrderDate", newCustomerBO.OrderDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", newCustomerBO.reportedBy, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopName", newCustomerBO.ShopName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Phone", newCustomerBO.Phone, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(newCustomerBO.Location))
            {
                sqlHelper.AddParameter(command, "@location", newCustomerBO.Location, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@district", newCustomerBO.District, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@lattitude", newCustomerBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", newCustomerBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", newCustomerBO.ProcesName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@shopTempId", newCustomerBO.TempShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ContactName", newCustomerBO.contactName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@pincode", newCustomerBO.pinCode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@shortName", newCustomerBO.shortName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@length", newCustomerBO.Length, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(newCustomerBO.isSMSAlertRequired))
            {
                sqlHelper.AddParameter(command, "@isSMSAlertRequired", newCustomerBO.isSMSAlertRequired, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(newCustomerBO.ShopType))
            {
                sqlHelper.AddParameter(command, "@shopType", newCustomerBO.ShopType, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(newCustomerBO.ShopType))
            {
                sqlHelper.AddParameter(command, "@shopClass", newCustomerBO.ShopClass, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@Address", newCustomerBO.Address, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@City", newCustomerBO.City, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@State", newCustomerBO.State, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Country", newCustomerBO.Country, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Continent", newCustomerBO.Continent, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RouteId", newCustomerBO.RouteId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@DistributorId", newCustomerBO.DistributorId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Email", newCustomerBO.Email, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@TinNo", newCustomerBO.TinNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@CSTNo", newCustomerBO.CSTNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ThirdpartyName", newCustomerBO.ThirdParty, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(newCustomerBO.CustomerGroup) && newCustomerBO.CustomerGroup!="0")
            {
                sqlHelper.AddParameter(command, "@CustomerGroup", newCustomerBO.CustomerGroup, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(newCustomerBO.CreditLimit))
            {
                sqlHelper.AddParameter(command, "@CreditLimit", newCustomerBO.CreditLimit, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@Doorno", newCustomerBO.DoorNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Transporter", newCustomerBO.Transporter, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StreetName", newCustomerBO.StreetName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ParentShopId", newCustomerBO.ParentShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field1", newCustomerBO.Field1, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field2", newCustomerBO.Field2, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field3", newCustomerBO.Field3, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field4", newCustomerBO.Field4, ParameterDirection.Input);

            sqlHelper.AddParameter(command, "@field5", newCustomerBO.Field5, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field6", newCustomerBO.Field6, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field7", newCustomerBO.Field7, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field8", newCustomerBO.Field8, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field9", newCustomerBO.Field9, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field10", newCustomerBO.Field10, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field11", newCustomerBO.Field11, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field12", newCustomerBO.Field12, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field13", newCustomerBO.Field13, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field14", newCustomerBO.Field14, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field15", newCustomerBO.Field15, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field16", newCustomerBO.Field16, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobRefNo", newCustomerBO.MobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", newCustomerBO.GpsSource, ParameterDirection.Input);
            if (newCustomerBO.CustomerCategoryid != 0)
            {
                sqlHelper.AddParameter(command, "@CustomerCategoryid", newCustomerBO.CustomerCategoryid, ParameterDirection.Input);
            }
            if (newCustomerBO.reportedDate != null)
            {
                //sqlHelper.AddParameter(command, "@ReportedDate", Convert.ToDateTime(newCustomerBO.reportedDate, CultureInfo.InvariantCulture), ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ReportedDate", newCustomerBO.reportedDate, ParameterDirection.Input);
            }
            if (newCustomerBO.gstin != null)
            {
                sqlHelper.AddParameter(command, "@GstIn", newCustomerBO.gstin, ParameterDirection.Input);
            }
            return sqlHelper.ExecuteDataSet(command);
        }
        public int getNewCustomerId(NewCustomerBO newCustomerBO,string id)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(newCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "select NewCustomerId from NewCustomer where TempMShopId ='"+id+"'";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal DataSet UpdateNewCustomerSync(NewCustomerBO newCustomerBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(newCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspNewCustomer";
            sqlHelper.AddParameter(command, "@Mode", 5, ParameterDirection.Input);
            //sqlHelper.AddParameter(command, "@OrderDate", newCustomerBO.OrderDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", newCustomerBO.reportedBy, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopName", newCustomerBO.ShopName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Phone", newCustomerBO.Phone, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@location", newCustomerBO.Location, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@district", newCustomerBO.District, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@length", newCustomerBO.Length, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(newCustomerBO.Latitude))
            {
                sqlHelper.AddParameter(command, "@lattitude", newCustomerBO.Latitude, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(newCustomerBO.Longitude))
            {
                sqlHelper.AddParameter(command, "@longitude", newCustomerBO.Longitude, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@processName", newCustomerBO.ProcesName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@shopTempId", newCustomerBO.TempShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ContactName", newCustomerBO.contactName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@pincode", newCustomerBO.pinCode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@shortName", newCustomerBO.shortName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@shopType", newCustomerBO.ShopType, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@shopClass", newCustomerBO.ShopClass, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Address", newCustomerBO.Address, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@City", newCustomerBO.City, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@State", newCustomerBO.State, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Country", newCustomerBO.Country, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Continent", newCustomerBO.Continent, ParameterDirection.Input);
            if (newCustomerBO.RouteId != null && newCustomerBO.RouteId != 0)
            {
                sqlHelper.AddParameter(command, "@RouteId", newCustomerBO.RouteId, ParameterDirection.Input);
            }
            if (newCustomerBO.DistributorId != null && newCustomerBO.DistributorId != 0)
            {
                sqlHelper.AddParameter(command, "@DistributorId", newCustomerBO.DistributorId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@Email", newCustomerBO.Email, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@TinNo", newCustomerBO.TinNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@CSTNo", newCustomerBO.CSTNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ThirdpartyName", newCustomerBO.ThirdParty, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(newCustomerBO.CustomerGroup) && newCustomerBO.CustomerGroup != "0")
            {
                sqlHelper.AddParameter(command, "@CustomerGroup", newCustomerBO.CustomerGroup, ParameterDirection.Input);
            }

            if (!string.IsNullOrEmpty(newCustomerBO.CreditLimit))
            {
                sqlHelper.AddParameter(command, "@CreditLimit", newCustomerBO.CreditLimit, ParameterDirection.Input);
            }            
            sqlHelper.AddParameter(command, "@Doorno", newCustomerBO.DoorNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Transporter", newCustomerBO.Transporter, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StreetName", newCustomerBO.StreetName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ParentShopId", newCustomerBO.ParentShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field1", newCustomerBO.Field1, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field2", newCustomerBO.Field2, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field3", newCustomerBO.Field3, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field4", newCustomerBO.Field4, ParameterDirection.Input);

            sqlHelper.AddParameter(command, "@field5", newCustomerBO.Field5, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field6", newCustomerBO.Field6, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field7", newCustomerBO.Field7, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field8", newCustomerBO.Field8, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field9", newCustomerBO.Field9, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field10", newCustomerBO.Field10, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field11", newCustomerBO.Field11, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field12", newCustomerBO.Field12, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field13", newCustomerBO.Field13, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field14", newCustomerBO.Field14, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field15", newCustomerBO.Field15, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@field16", newCustomerBO.Field16, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobRefNo", newCustomerBO.MobRefNo, ParameterDirection.Input);
            if (newCustomerBO.GpsSource != null && newCustomerBO.GpsSource!=0)
            {
                sqlHelper.AddParameter(command, "@GpsSource", newCustomerBO.GpsSource, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileSyncDate", newCustomerBO.MobileSyncDate, ParameterDirection.Input);

            if (newCustomerBO.reportedDate != null)
            {
                //sqlHelper.AddParameter(command, "@ReportedDate", Convert.ToDateTime(newCustomerBO.reportedDate, CultureInfo.InvariantCulture), ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ReportedDate", newCustomerBO.reportedDate, ParameterDirection.Input);
            }
            if (newCustomerBO.gstin != null)
            {
                sqlHelper.AddParameter(command, "@GstIn", newCustomerBO.gstin, ParameterDirection.Input);
            }
            if (newCustomerBO.CustomerCategoryid != 0)
            {
                sqlHelper.AddParameter(command, "@CustomerCategoryid", newCustomerBO.CustomerCategoryid, ParameterDirection.Input);
            }
            return sqlHelper.ExecuteDataSet(command);
        }

        internal int IsEmailAlertRequiredNewCustomer(NewCustomerBO newCustomerBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(newCustomerBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspNewCustomer";
            sqlHelper.AddParameter(command, "@Mode", 8, ParameterDirection.Input);
            SqlParameter outparam = command.Parameters.Add("@NewCustId", SqlDbType.Int);
            outparam.Direction = ParameterDirection.Output;
            sqlHelper.ExecuteNonQuery(command);
            return Convert.ToInt32(outparam.Value);
        }

       
    }
}
