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
    public class PaymentDAO 
    {
        internal String UpdatePaymentResponse(PaymentBO PaymentBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(PaymentBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspInsertPaymentResponse";
            sqlHelper.AddParameter(command, "@ResponseCode", PaymentBO.ResponseCode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Response", PaymentBO.Response, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MerRefNo", PaymentBO.MerRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RRN", PaymentBO.RRN, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@TransactionDate", PaymentBO.TransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RemittorName", PaymentBO.RemittorName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@CheckSum", PaymentBO.CheckSum, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", PaymentBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@USerId", PaymentBO.userId, ParameterDirection.Input);
            SqlParameter outparam = command.Parameters.Add("@OutputMessage", SqlDbType.VarChar);
            outparam.Size = 50;
            outparam.Direction = ParameterDirection.Output;
            sqlHelper.ExecuteNonQuery(command);
            string result = command.Parameters["@OutputMessage"].Value.ToString();
            return result;
        }

        internal DataSet getShopPaymentParam(PaymentBO paymentBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(paymentBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "getCustomerPaymentParam";
            sqlHelper.AddParameter(command, "@ShopId", paymentBO.ShopId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal void updateCustomerParam(PaymentBO paymentBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(paymentBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateCustomerParam";
            sqlHelper.AddParameter(command, "@MMID", paymentBO.MMID, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@phoneNo", paymentBO.phoneNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", paymentBO.ShopId, ParameterDirection.Input);
            sqlHelper.ExecuteNonQuery(command);
        }
    }
}
