using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class ProductTransactionDAO
    {
        internal DataSet GetProductQty(ProductTransactionBO productTransactionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(productTransactionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            sqlHelper.AddParameter(command, "@UserId", productTransactionBO.UserId, ParameterDirection.Input);
            command.CommandText = "getOrderedQtyForAndroid";
           return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetDeliveredProductQty(ProductTransactionBO productTransactionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(productTransactionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            sqlHelper.AddParameter(command, "@UserId", productTransactionBO.UserId, ParameterDirection.Input);
            command.CommandText = "getDeliveredQtyForAndroid";
            return sqlHelper.ExecuteDataSet(command);
        }
        internal DataSet GetReturnedQty(ProductTransactionBO productTransactionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(productTransactionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            sqlHelper.AddParameter(command, "@UserId", productTransactionBO.UserId, ParameterDirection.Input);
            command.CommandText = "getReturnedQtyForAndroid";
            return sqlHelper.ExecuteDataSet(command);
        }

        internal void InsertInto(ProductTransactionBO productTransactionBO)
        {
            DataAccessSqlHelper helper = new DataAccessSqlHelper(productTransactionBO.ConString);
            SqlCommand command = helper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspInsertIntoProductTransaction";
            helper.AddParameter(command, "@productId",productTransactionBO.productId,ParameterDirection.Input);
            helper.AddParameter(command, "@orderedQty", productTransactionBO.orderedQty, ParameterDirection.Input);
            helper.AddParameter(command, "@organizationId", productTransactionBO.organizationId, ParameterDirection.Input);
            helper.AddParameter(command, "@UnitId", productTransactionBO.unitId, ParameterDirection.Input);
            helper.ExecuteNonQuery(command);
        }
        internal void deleteData(ProductTransactionBO productTransactionalBO)
        {
            DataAccessSqlHelper dataAccesshelper = new DataAccessSqlHelper(productTransactionalBO.ConString);
            SqlCommand command = dataAccesshelper.CreateCommand(CommandType.Text);
            command.CommandText = "Delete from ProductTransaction";
            dataAccesshelper.ExecuteNonQuery(command);
        }
    }
}
