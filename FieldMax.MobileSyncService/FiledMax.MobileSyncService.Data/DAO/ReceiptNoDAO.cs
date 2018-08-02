﻿using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ReceiptNoDAO
    {
        internal DataSet GetReceiptNoDetails(ReceiptNoBO receiptNoBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(receiptNoBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_GetReceiptNo";
            sqlHelper.AddParameter(command, "@UserId", receiptNoBO.UserId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal int UpdateReceiptNo(ReceiptNoBO receiptNoBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(receiptNoBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateRecieptNo";
            sqlHelper.AddParameter(command, "@UserId", receiptNoBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RecieptNo", receiptNoBO.ReceiptNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", receiptNoBO.Flag, ParameterDirection.Input);

            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
