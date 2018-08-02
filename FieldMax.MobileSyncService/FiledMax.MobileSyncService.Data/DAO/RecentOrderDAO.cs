using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class RecentOrderDAO
    {
        internal DataSet GetRecentOrders(RecentOrderBO recentOrderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(recentOrderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetRecentOrderDetailsValue";
            sqlHelper.AddParameter(command, "@ShopId", recentOrderBO.ShopId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
  

        internal DataSet GetOrdersToConfirm(RecentOrderBO recentOrderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(recentOrderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetOrdersToConfirmAndroid";
            sqlHelper.AddParameter(command, "@UserId", recentOrderBO.UserId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetOrderDetails(RecentOrderBO recentOrderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(recentOrderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetOrdersDetailsToConfirmAndroid";
            sqlHelper.AddParameter(command, "@OrderId", recentOrderBO.OrderId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal int updateOrderStatus(RecentOrderBO recentOrderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(recentOrderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "UpdateOrderStatus";
            sqlHelper.AddParameter(command, "@OrderId", recentOrderBO.UpdateOrderId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderStatus", recentOrderBO.Status, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
