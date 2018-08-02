using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class CollectionDAO //: DAOBase
    {
        internal int GetCollectionCount(CollectionBO collectionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(collectionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetCollectionCount";
            sqlHelper.AddParameter(command, "@UserId", collectionBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderDate", collectionBO.InstrumentDate.ToString(), ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int UpdateCollection(CollectionBO collectionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(collectionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);

            if (collectionBO.BillNo.Equals(""))
            {
                command.CommandText = "uspPaymentHeader"; //"uspWSUpdateCollectionDetails";
                sqlHelper.AddParameter(command, "@Mode", collectionBO.Mode, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ShopID", collectionBO.ShopId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@CollectedBy", collectionBO.CollectedBy, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Amount", collectionBO.TotalAmount, ParameterDirection.Input);
                //sqlHelper.AddParameter(command, "@InvoiceNo", collectionBO.InvoiceNo, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ReceiptNo", collectionBO.ReceiptNo, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@PaymentModeId", collectionBO.PaymentModeId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@InstrumentNo", collectionBO.InstrumentNo, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Narration", collectionBO.Narration, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@InstrumentDate", collectionBO.InstrumentDate.ToString(), ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@lattitude", collectionBO.Latitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@longitude", collectionBO.Longitude, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@processName", collectionBO.ProcessName, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@mobilePaymentDate", collectionBO.MobilePaymentDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@MobileReferenceNo", collectionBO.MobileReferenceNo, ParameterDirection.Input);
                if (collectionBO.signalStrength != null && collectionBO.signalStrength != string.Empty && collectionBO.signalStrength != "")
                {
                    sqlHelper.AddParameter(command, "@signalStrength", collectionBO.signalStrength, ParameterDirection.Input);
                }
                if (collectionBO.networkProvider != null && collectionBO.networkProvider != string.Empty && collectionBO.networkProvider != "")
                {
                    sqlHelper.AddParameter(command, "@networkProvider", collectionBO.networkProvider, ParameterDirection.Input);
                }
                if (collectionBO.CollectionDiscount != "")
                {
                    sqlHelper.AddParameter(command, "@CollectionDiscount", collectionBO.CollectionDiscount, ParameterDirection.Input);
                }
                
                sqlHelper.AddParameter(command, "@GpsSource", collectionBO.GpsSource, ParameterDirection.Input);
                if (collectionBO.BankId != 0)
                {
                    sqlHelper.AddParameter(command, "@BankId", collectionBO.BankId, ParameterDirection.Input);
                }
                if (collectionBO.IsRemitted.Equals("1"))
                {
                    
                    if (!collectionBO.RemittedAt.Equals(""))
                    {
                        sqlHelper.AddParameter(command, "@IsRemitted", collectionBO.IsRemitted, ParameterDirection.Input);
                        sqlHelper.AddParameter(command, "@RemittedAt", collectionBO.RemittedAt, ParameterDirection.Input);
                        sqlHelper.AddParameter(command, "@RemittedDate", DateTime.Now, ParameterDirection.Input);
                    }
                }
                if (collectionBO.MobileSyncDate != "" && collectionBO.MobileDate != "")
                {
                    sqlHelper.AddParameter(command, "@MobileSyncDate", collectionBO.MobileDate, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@ServerSyncDate", collectionBO.MobileSyncDate, ParameterDirection.Input);
                }
                if(!string.IsNullOrEmpty(collectionBO.TempShopId))
                {
                    sqlHelper.AddParameter(command, "@TempShopId", collectionBO.TempShopId, ParameterDirection.Input);
                }
               
            }
            else
            {
                if (collectionBO.IsMultipleDiscountCollection.Equals("true"))
                {
                    command.CommandText = "setBillWiseCollectionWithDiscount";
                    sqlHelper.AddParameter(command, "@Mode", 4, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@ShopId", collectionBO.ShopId, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@CollectedBy", collectionBO.CollectedBy, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@AmountSet", collectionBO.Amount, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@ReceiptNo", collectionBO.ReceiptNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@PaymentModeId", collectionBO.PaymentModeId, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@InstrumentNo", collectionBO.InstrumentNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@Narration", collectionBO.Narration, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@InstrumentDate", collectionBO.InstrumentDate.ToString(), ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@lattitude", collectionBO.Latitude, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@longitude", collectionBO.Longitude, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@processName", collectionBO.ProcessName, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@BillNoSet", collectionBO.BillNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@discountSet", collectionBO.Discount, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@OsBalanceSet", collectionBO.OsBalance, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@Amount", collectionBO.TotalAmount, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@rdDiscountSet", collectionBO.Discount1, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@sdDiscountSet", collectionBO.Discount2, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@cdDiscountSet", collectionBO.Discount3, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@mobilePaymentDate", collectionBO.MobilePaymentDate, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@MobileReferenceNo", collectionBO.MobileReferenceNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@GpsSource", collectionBO.GpsSource, ParameterDirection.Input);
                    if (collectionBO.signalStrength != null && collectionBO.signalStrength != string.Empty && collectionBO.signalStrength != "")
                    {
                        sqlHelper.AddParameter(command, "@signalStrength", collectionBO.signalStrength, ParameterDirection.Input);
                    }
                    if (collectionBO.networkProvider != null && collectionBO.networkProvider != string.Empty && collectionBO.networkProvider != "")
                    {
                        sqlHelper.AddParameter(command, "@networkProvider", collectionBO.networkProvider, ParameterDirection.Input);
                    }
                    //sqlHelper.AddParameter(command, "@mobilePaymentDate", DateTime.Now.ToString(), ParameterDirection.Input);
                    if (collectionBO.BankId != 0)
                    {
                        sqlHelper.AddParameter(command, "@BankIdValue", collectionBO.BankId, ParameterDirection.Input);
                    }
                    if (collectionBO.MobileSyncDate != "" && collectionBO.MobileDate != "")
                    {
                        sqlHelper.AddParameter(command, "@MobileSyncDate", collectionBO.MobileDate, ParameterDirection.Input);
                        sqlHelper.AddParameter(command, "@ServerSyncDate", collectionBO.MobileSyncDate, ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(collectionBO.TempShopId))
                    {
                        sqlHelper.AddParameter(command, "@TempShopId", collectionBO.TempShopId, ParameterDirection.Input);
                    }

                }
                else
                {
                    command.CommandText = "getBillNoForBillWiseCollection";
                    sqlHelper.AddParameter(command, "@Mode", 2, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@ShopID", collectionBO.ShopId, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@CollectedBy", collectionBO.CollectedBy, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@AmountSet", collectionBO.Amount, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@ReceiptNo", collectionBO.ReceiptNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@PaymentModeId", collectionBO.PaymentModeId, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@InstrumentNo", collectionBO.InstrumentNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@Narration", collectionBO.Narration, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@InstrumentDate", collectionBO.InstrumentDate.ToString(), ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@lattitude", collectionBO.Latitude, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@longitude", collectionBO.Longitude, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@processName", collectionBO.ProcessName, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@BillNoSet", collectionBO.BillNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@discountSet", collectionBO.Discount, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@OsBalanceSet", collectionBO.OsBalance, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@Amount", collectionBO.TotalAmount, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@mobilePaymentDate", collectionBO.MobilePaymentDate, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@MobileReferenceNo", collectionBO.MobileReferenceNo, ParameterDirection.Input);
                    sqlHelper.AddParameter(command, "@GpsSource", collectionBO.GpsSource, ParameterDirection.Input);
                    
                    //sqlHelper.AddParameter(command, "@mobilePaymentDate", DateTime.Now.ToString(), ParameterDirection.Input);
                    if (collectionBO.BankId != 0)
                    {
                        sqlHelper.AddParameter(command, "@BankId", collectionBO.BankId, ParameterDirection.Input);
                    }
                    if (collectionBO.MobileSyncDate != "" && collectionBO.MobileDate != "")
                    {
                        sqlHelper.AddParameter(command, "@MobileSyncDate", collectionBO.MobileDate, ParameterDirection.Input);
                        sqlHelper.AddParameter(command, "@ServerSyncDate", collectionBO.MobileSyncDate, ParameterDirection.Input);
                    }
                    if (!string.IsNullOrEmpty(collectionBO.TempShopId))
                    {
                        sqlHelper.AddParameter(command, "@TempShopId", collectionBO.TempShopId, ParameterDirection.Input);
                    }
                }
                
            }

           
            //sqlHelper.AddParameter(command, "@Discount", collectionBO.Discount, ParameterDirection.Input); ;
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
        internal string GetTodaysCashCollection(CollectionBO collectionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(collectionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT  ISNULL(SUM(Amount),0) Amount from paymentheader where CONVERT(varchar(100),paymentdate,103) = CONVERT(varchar(100),getdate(),103) and paymentModeId = 1 and CollectedBy = "+collectionBO.CollectedBy;
            return Convert.ToString(sqlHelper.ExecuteScalar(command));
        }

        internal DataSet GetCollectionData(CollectionBO collectionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(collectionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "getInvoiceorCollectionDetails";
            sqlHelper.AddParameter(command, "@Mode", collectionBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", collectionBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", collectionBO.ShopId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetCollectionDetails(CollectionBO collectionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(collectionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "getInvoiceorCollectionDetails";
            sqlHelper.AddParameter(command, "@Mode", collectionBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ReceiptNo", collectionBO.ReceiptNo, ParameterDirection.Input);            
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
