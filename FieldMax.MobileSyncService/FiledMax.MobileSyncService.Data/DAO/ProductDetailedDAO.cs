using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ProductDetailedDAO
    {
        public int UpdateProductDetailed(ProductDetailedBO productDetailedBO)
        {
            try
            {
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(productDetailedBO.ConString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "uspUpdateProductDetailed";
                sqlHelper.AddParameter(command, "@UserId", productDetailedBO.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ShopId", productDetailedBO.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProductData", productDetailedBO.ProductData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@latitude", productDetailedBO.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@longitude", productDetailedBO.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@processName", productDetailedBO.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@DetailedDate", productDetailedBO.DetailedDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@MobileTransactionDate", productDetailedBO.mobileTransactionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SyncDate", productDetailedBO.SyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@mobileReferenceNo", productDetailedBO.mobileReferenceNo, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Remark", productDetailedBO.remark, ParameterDirection.Input);
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
