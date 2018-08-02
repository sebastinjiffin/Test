using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ExpenseDAO //: DAOBase
    {
        internal int GetExpense(ExpenseBO expenseBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(expenseBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetPreviousDayOrdersCount";
            sqlHelper.AddParameter(command, "@UserId", expenseBO.UserId, ParameterDirection.Input);
            //sqlHelper.AddParameter(command, "@OrderDate", expenseBO.OrderDate, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int UpdateExpense(ExpenseBO expenseBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(expenseBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspExpense";
            sqlHelper.AddParameter(command, "@Mode", expenseBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@QuantityList", expenseBO.Quantity, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", expenseBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Parameters", expenseBO.Parameters, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", expenseBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", expenseBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", expenseBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", expenseBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileReferenceNo", expenseBO.mobilereferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Remarks", expenseBO.Remarks, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", expenseBO.GpsSource, ParameterDirection.Input);

            if (!string.IsNullOrEmpty(expenseBO.Field1))
            {
                sqlHelper.AddParameter(command, "@Field1List", expenseBO.Field1, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(expenseBO.Field2))
            {
                sqlHelper.AddParameter(command, "@Field2List", expenseBO.Field2, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(expenseBO.Field3))
            {
                sqlHelper.AddParameter(command, "@Field3List", expenseBO.Field3, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(expenseBO.Field4))
            {
                sqlHelper.AddParameter(command, "@Field4List", expenseBO.Field4, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(expenseBO.Field5))
            {
                sqlHelper.AddParameter(command, "@Field5List", expenseBO.Field5, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(expenseBO.ExpenseDate))
            {
                sqlHelper.AddParameter(command, "@ExpenseDateList", expenseBO.ExpenseDate, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(expenseBO.UniqueKey))
            {
                sqlHelper.AddParameter(command, "@UniqueKeyList", expenseBO.UniqueKey, ParameterDirection.Input);
            }
            if (expenseBO.ServerSyncDate != "" && expenseBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", expenseBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", expenseBO.ServerSyncDate, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal string GetTodaysExpense(ExpenseBO expenseBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(expenseBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT ISNULL(SUM(AMOUNT),0) FROM expenseentry where CONVERT(varchar(100),ExpenseDate,103) = CONVERT(varchar(100),getdate(),103) and userId = " + expenseBO.UserId;
            return Convert.ToString(sqlHelper.ExecuteScalar(command));
        }

        public void InsertTransactionData(ExpenseBO expenseBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(expenseBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "InsertExpenseTransaction";
            sqlHelper.AddParameter(command, "@UserId", expenseBO.UserId, ParameterDirection.Input);
            sqlHelper.ExecuteNonQuery(command);
        }
    }
}
