using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;
using System.IO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class CancelledOrdersDAO
    {
        public DataSet GetCancelledOrdersDetails(CancelledOrdersBO cancelledOrdersBO)
        {
            DataSet dt = new DataSet();
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(cancelledOrdersBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspCancelledOrderDetails";
            sqlHelper.AddParameter(command, "@Mode", cancelledOrdersBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderTakenBy", cancelledOrdersBO.OrderTakenBy, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", cancelledOrdersBO.ShopId, ParameterDirection.Input);
            dt = sqlHelper.ExecuteDataSet(command);
            return dt;
        }

        internal DataSet getProductList(CancelledOrdersBO cancelledOrdersBO, int orderId)
        {
            DataSet dt = new DataSet();
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(cancelledOrdersBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspCancelledOrderDetails";
            sqlHelper.AddParameter(command, "@Mode", "2" , ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderId", orderId, ParameterDirection.Input);
            dt = sqlHelper.ExecuteDataSet(command);
            return dt;
        }

        internal void CancelOrders(CancelledOrdersBO cancelledOrdersBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(cancelledOrdersBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspCancelledOrderDetails";
            sqlHelper.AddParameter(command, "@Mode", "3", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderId", cancelledOrdersBO.orderIds, ParameterDirection.Input);
            sqlHelper.ExecuteNonQuery(command);
          
        }
    }
}
