using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;
using System.IO;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class StockAgingDAO
    {
       internal int UpdateStockAging(StockAgingBO StockAgingBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(StockAgingBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "UpdateStockAging";


           sqlHelper.AddParameter(command, "@UserId", StockAgingBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ShopID", StockAgingBO.ShopId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ProductData", StockAgingBO.productData, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@quantitydata", StockAgingBO.quantitydata, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@UnitData", StockAgingBO.unitSet, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Age", StockAgingBO.age, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@batchNoData", StockAgingBO.batchNoData, ParameterDirection.Input);
          
          
           
           sqlHelper.AddParameter(command, "@remarks", StockAgingBO.remarks, ParameterDirection.Input);
      

           sqlHelper.AddParameter(command, "@ProcessName", StockAgingBO.ProcessName, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Latitude", StockAgingBO.Latitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Longitude", StockAgingBO.Longitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@GpsSource", StockAgingBO.GpsSource, ParameterDirection.Input);
           if (StockAgingBO.mobileReferenceNo != string.Empty)
           {
               sqlHelper.AddParameter(command, "@MobileReferenceNo", StockAgingBO.mobileReferenceNo, ParameterDirection.Input);
           }
           if (StockAgingBO.mobileTransactionDate != null && StockAgingBO.mobileTransactionDate != string.Empty && StockAgingBO.mobileTransactionDate != "")
           {
               sqlHelper.AddParameter(command, "@mobileTransactionDate", StockAgingBO.mobileTransactionDate, ParameterDirection.Input);
           }

           if (StockAgingBO.expiryDate != null && StockAgingBO.expiryDate != string.Empty && StockAgingBO.expiryDate != "")
           {
               sqlHelper.AddParameter(command, "@expiryDate", StockAgingBO.expiryDate, ParameterDirection.Input);
           }

           if (StockAgingBO.manufactureddate != null && StockAgingBO.manufactureddate != string.Empty && StockAgingBO.manufactureddate != "")
           {
               sqlHelper.AddParameter(command, "@manufactureddate", StockAgingBO.manufactureddate, ParameterDirection.Input);
           }
           if (StockAgingBO.signalStrength != null && StockAgingBO.signalStrength != string.Empty && StockAgingBO.signalStrength != "")
           {
               sqlHelper.AddParameter(command, "@signalStrength", StockAgingBO.signalStrength, ParameterDirection.Input);
           }
           if (StockAgingBO.networkProvider != null && StockAgingBO.networkProvider != string.Empty && StockAgingBO.networkProvider != "")
           {
               sqlHelper.AddParameter(command, "@networkProvider", StockAgingBO.networkProvider, ParameterDirection.Input);
           }
           return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
       }
    }
}
