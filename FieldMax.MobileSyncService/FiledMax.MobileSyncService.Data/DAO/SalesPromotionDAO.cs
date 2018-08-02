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
    public class SalesPromotionDAO
    {
        internal int UpdateSalesPromotion(SalesPromotionBO salesPromotionBo)
        {
            try
            {
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(salesPromotionBo.ConString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "uspUpdateSales";
                sqlHelper.AddParameter(command, "@UserId", salesPromotionBo.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ShopID", salesPromotionBo.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ProductData", salesPromotionBo.ProductData, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@QuantityData", salesPromotionBo.quantity, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@latitude ", salesPromotionBo.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@longitude", salesPromotionBo.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@processName", salesPromotionBo.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Narration", salesPromotionBo.Narration, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@MobileTransactionDate", salesPromotionBo.mobileTransactionDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Unit", salesPromotionBo.UnitId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SyncDate", salesPromotionBo.SyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@mobileReferenceNo", salesPromotionBo.mobileReferenceNo, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@GpsSource", salesPromotionBo.GpsSource, ParameterDirection.Input);
                if (salesPromotionBo.mobileDate != null && salesPromotionBo.mobileDate != string.Empty && salesPromotionBo.mobileDate != "")
                {
                    sqlHelper.AddParameter(command, "@mobileSyncDate", salesPromotionBo.mobileDate, ParameterDirection.Input);
                }
                if (salesPromotionBo.signalStrength != null && salesPromotionBo.signalStrength != string.Empty && salesPromotionBo.signalStrength != "")
                {
                    sqlHelper.AddParameter(command, "@signalStrength", salesPromotionBo.signalStrength, ParameterDirection.Input);
                }
                if (salesPromotionBo.networkProvider != null && salesPromotionBo.networkProvider != string.Empty && salesPromotionBo.networkProvider != "")
                {
                    sqlHelper.AddParameter(command, "@networkProvider", salesPromotionBo.networkProvider, ParameterDirection.Input);
                }
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
