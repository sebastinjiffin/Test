using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;
using System.IO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class OrderDAO //: DAOBase
    {
        internal int GetPreviousDayOrdersCount(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetPreviousDayOrdersCount";
            sqlHelper.AddParameter(command, "@UserId", orderBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderDate", orderBO.OrderDate, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal string UpdateOrder(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "OrderTransaction";
            command.CommandTimeout = orderBO.CommandTimeout;
            sqlHelper.AddParameter(command, "@Mode", orderBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", orderBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopID", orderBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProductData", orderBO.ProductData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@quantitydata", orderBO.QuantityData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UnitData", orderBO.UnitData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Rate", orderBO.Rate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", orderBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", orderBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", orderBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderDate", orderBO.OrderDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@AmountFromMobile", orderBO.TotalAmount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", orderBO.mobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", orderBO.mobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Priority", orderBO.Priority, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Length", orderBO.length, ParameterDirection.Input);
            if (orderBO.SpecialInstnSet != null && orderBO.SpecialInstnSet != string.Empty && orderBO.SpecialInstnSet != "")
            {
                sqlHelper.AddParameter(command, "@SpecialInstnSet", orderBO.SpecialInstnSet, ParameterDirection.Input);
            }
            if (orderBO.SchemeId != null && orderBO.SchemeId > 0)
            {
                sqlHelper.AddParameter(command, "@SchemeId", orderBO.SchemeId, ParameterDirection.Input);
            }
            if (orderBO.OtherInstn != null && orderBO.OtherInstn != string.Empty && orderBO.OtherInstn != "")
            {
                sqlHelper.AddParameter(command, "@OtherInstn", orderBO.OtherInstn, ParameterDirection.Input);
            }

            if (orderBO.SiteAddress != null && orderBO.SiteAddress != string.Empty && orderBO.SiteAddress != "")
            {
                sqlHelper.AddParameter(command, "@SiteAddress", orderBO.SiteAddress, ParameterDirection.Input);
            }
            if (orderBO.ContactPerson != null && orderBO.ContactPerson != string.Empty && orderBO.ContactPerson != "")
            {
                sqlHelper.AddParameter(command, "@ContactPerson", orderBO.ContactPerson, ParameterDirection.Input);
            }
            if (orderBO.Phone != null && orderBO.Phone != string.Empty && orderBO.Phone != "")
            {
                sqlHelper.AddParameter(command, "@Phone", orderBO.Phone, ParameterDirection.Input);
            }
            if (orderBO.FirstDiscount != null && orderBO.FirstDiscount != string.Empty && orderBO.FirstDiscount != "")
            {
                sqlHelper.AddParameter(command, "@FirstDiscount", orderBO.FirstDiscount, ParameterDirection.Input);
            }
            if (orderBO.SecondDiscount != null && orderBO.SecondDiscount != string.Empty && orderBO.SecondDiscount != "")
            {
                sqlHelper.AddParameter(command, "@SecondDiscount", orderBO.SecondDiscount, ParameterDirection.Input);
            }

            if (orderBO.BankName != null && orderBO.BankName != string.Empty && orderBO.BankName != "")
            {
                sqlHelper.AddParameter(command, "@BankName", orderBO.BankName, ParameterDirection.Input);
            }
            if (orderBO.unitDiscount != null && orderBO.unitDiscount != string.Empty && orderBO.unitDiscount != "")
            {
                sqlHelper.AddParameter(command, "@UnitDisc", orderBO.unitDiscount, ParameterDirection.Input);
            }
            if (orderBO.freeQuantity != null && orderBO.freeQuantity != string.Empty && orderBO.freeQuantity != "")
            {
                sqlHelper.AddParameter(command, "@FreeQty", orderBO.freeQuantity, ParameterDirection.Input);
            }
            if (orderBO.mobileDiscFlag != null && orderBO.mobileDiscFlag != string.Empty && orderBO.mobileDiscFlag != "")
            {
                sqlHelper.AddParameter(command, "@MobileDiscFlag", orderBO.mobileDiscFlag, ParameterDirection.Input);
            }
            if (orderBO.orderDisc != null && orderBO.orderDisc != string.Empty && orderBO.orderDisc != "")
            {
                sqlHelper.AddParameter(command, "@OrderDisc", orderBO.orderDisc, ParameterDirection.Input);
            }

            if (orderBO.source != null && orderBO.source != string.Empty && orderBO.source != "")
            {
                sqlHelper.AddParameter(command, "@GpsSource", Convert.ToInt32(orderBO.source), ParameterDirection.Input);
            }
            if (orderBO.userRef != null && orderBO.userRef != string.Empty && orderBO.userRef != "")
            {
                sqlHelper.AddParameter(command, "@UserRef", orderBO.userRef, ParameterDirection.Input);
            }

            if (orderBO.paymentReferenceNo > 0)
            {
                sqlHelper.AddParameter(command, "@PaymentNo", orderBO.paymentReferenceNo, ParameterDirection.Input);
            }
            if (Convert.ToInt32(orderBO.distributorId) != 0)
            {
                sqlHelper.AddParameter(command, "@DistributorIdFromMobile", Convert.ToInt32(orderBO.distributorId), ParameterDirection.Input);
            }

            if (!orderBO.syncDate.Equals(String.Empty))
            {
                sqlHelper.AddParameter(command, "@SyncDate", orderBO.syncDate, ParameterDirection.Input);
            }

            if (orderBO.orderDiscountIds != null && orderBO.orderDiscountIds != string.Empty && orderBO.orderDiscountIds != "")
            {
                sqlHelper.AddParameter(command, "@orderDiscountIds", orderBO.orderDiscountIds, ParameterDirection.Input);
            }
            if (orderBO.orderDiscountVals != null && orderBO.orderDiscountVals != string.Empty && orderBO.orderDiscountVals != "")
            {
                sqlHelper.AddParameter(command, "@OrderDiscountValues", orderBO.orderDiscountVals, ParameterDirection.Input);
            }
            if (orderBO.deliveryDate != null && orderBO.deliveryDate != string.Empty && orderBO.deliveryDate != "")
            {
                sqlHelper.AddParameter(command, "@DeliveryDate", orderBO.deliveryDate, ParameterDirection.Input);

            }
            if (orderBO.schemeFromMobile != null && orderBO.schemeFromMobile != string.Empty && orderBO.schemeFromMobile != "")
            {
                sqlHelper.AddParameter(command, "@schemeids", orderBO.schemeFromMobile, ParameterDirection.Input);

            }
            if (orderBO.paymentMode > 0)
            {
                sqlHelper.AddParameter(command, "@PaymentMode", orderBO.paymentMode, ParameterDirection.Input);
            }
            if (orderBO.mobileDate != null && orderBO.mobileDate != string.Empty && orderBO.mobileDate != "")
            {
                sqlHelper.AddParameter(command, "@mobileSyncDate", orderBO.mobileDate, ParameterDirection.Input);
            }
            if (orderBO.signalStrength != null && orderBO.signalStrength != string.Empty && orderBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", orderBO.signalStrength, ParameterDirection.Input);
            }            
            if (orderBO.RequestedQuantityData != null && orderBO.RequestedQuantityData != string.Empty && orderBO.RequestedQuantityData != "")
            {
                sqlHelper.AddParameter(command, "@RequestedQuantityData", orderBO.RequestedQuantityData, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(orderBO.TempShopId))
            {
                sqlHelper.AddParameter(command, "@TempShopId", orderBO.TempShopId, ParameterDirection.Input);
            }
            if (orderBO.TotalDiscount != null && orderBO.TotalDiscount != string.Empty && orderBO.TotalDiscount != "")
            {
                sqlHelper.AddParameter(command, "@TotalDiscountData", orderBO.TotalDiscount, ParameterDirection.Input);
            }
            if (orderBO.TaxAmount != null && orderBO.TaxAmount != string.Empty && orderBO.TaxAmount != "")
            {
                sqlHelper.AddParameter(command, "@TaxAmountData", orderBO.TaxAmount, ParameterDirection.Input);
            }
            //New
            if (orderBO.networkProvider != null && orderBO.networkProvider != string.Empty && orderBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", orderBO.networkProvider, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(orderBO.InvoiceNo))
            {
                sqlHelper.AddParameter(command, "@InvoiceNo", orderBO.InvoiceNo, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(orderBO.IsDefaultDistributor))
            {
                sqlHelper.AddParameter(command, "@IsDefaultDistributor", orderBO.IsDefaultDistributor, ParameterDirection.Input);
            }
            SqlParameter outparam = command.Parameters.Add("@OutputMessage", SqlDbType.VarChar);
            outparam.Size = 50;
            outparam.Direction = ParameterDirection.Output;
            sqlHelper.ExecuteNonQuery(command);
            string result = command.Parameters["@OutputMessage"].Value.ToString();
            string id = result.Split(':')[0];
            int returnData = 0;
            try
            {
                returnData = Convert.ToInt32(id);
            }
            catch
            {
                returnData = 0;
            }
            return result;
        }
        internal string ModifyOrder(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "UpdateOrderTransaction";
            sqlHelper.AddParameter(command, "@UserId", orderBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopID", orderBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProductData", orderBO.ProductData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@quantitydata", orderBO.QuantityData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UnitData", orderBO.UnitData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Rate", orderBO.Rate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", orderBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", orderBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", orderBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderDate", orderBO.OrderDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@AmountFromMobile", orderBO.TotalAmount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", orderBO.mobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", orderBO.mobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", Convert.ToInt32(orderBO.source), ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderId", Convert.ToInt32(orderBO.orderId), ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Priority", orderBO.Priority, ParameterDirection.Input);
            
            if (orderBO.SpecialInstnSet != null && orderBO.SpecialInstnSet != string.Empty && orderBO.SpecialInstnSet != "")
            {
                sqlHelper.AddParameter(command, "@SpecialInstnSet", orderBO.SpecialInstnSet, ParameterDirection.Input);
            }
            if (orderBO.SchemeId != null && orderBO.SchemeId > 0)
            {
                sqlHelper.AddParameter(command, "@SchemeId", orderBO.SchemeId, ParameterDirection.Input);
            }
            if (orderBO.OtherInstn != null && orderBO.OtherInstn != string.Empty && orderBO.OtherInstn != "")
            {
                sqlHelper.AddParameter(command, "@OtherInstn", orderBO.OtherInstn, ParameterDirection.Input);
            }

            if (orderBO.SiteAddress != null && orderBO.SiteAddress != string.Empty && orderBO.SiteAddress != "")
            {
                sqlHelper.AddParameter(command, "@SiteAddress", orderBO.SiteAddress, ParameterDirection.Input);
            }
            if (orderBO.ContactPerson != null && orderBO.ContactPerson != string.Empty && orderBO.ContactPerson != "")
            {
                sqlHelper.AddParameter(command, "@ContactPerson", orderBO.ContactPerson, ParameterDirection.Input);
            }
            if (orderBO.Phone != null && orderBO.Phone != string.Empty && orderBO.Phone != "")
            {
                sqlHelper.AddParameter(command, "@Phone", orderBO.Phone, ParameterDirection.Input);
            }
            if (orderBO.FirstDiscount != null && orderBO.FirstDiscount != string.Empty && orderBO.FirstDiscount != "")
            {
                sqlHelper.AddParameter(command, "@FirstDiscount", orderBO.FirstDiscount, ParameterDirection.Input);
            }
            if (orderBO.SecondDiscount != null && orderBO.SecondDiscount != string.Empty && orderBO.SecondDiscount != "")
            {
                sqlHelper.AddParameter(command, "@SecondDiscount", orderBO.SecondDiscount, ParameterDirection.Input);
            }

            if (orderBO.BankName != null && orderBO.BankName != string.Empty && orderBO.BankName != "")
            {
                sqlHelper.AddParameter(command, "@BankName", orderBO.BankName, ParameterDirection.Input);
            }
            if (orderBO.unitDiscount != null && orderBO.unitDiscount != string.Empty && orderBO.unitDiscount != "")
            {
                sqlHelper.AddParameter(command, "@UnitDisc", orderBO.unitDiscount, ParameterDirection.Input);
            }
            if (orderBO.freeQuantity != null && orderBO.freeQuantity != string.Empty && orderBO.freeQuantity != "")
            {
                sqlHelper.AddParameter(command, "@FreeQty", orderBO.freeQuantity, ParameterDirection.Input);
            }
            if (orderBO.mobileDiscFlag != null && orderBO.mobileDiscFlag != string.Empty && orderBO.mobileDiscFlag != "")
            {
                sqlHelper.AddParameter(command, "@MobileDiscFlag", orderBO.mobileDiscFlag, ParameterDirection.Input);
            }
            if (orderBO.orderDisc != null && orderBO.orderDisc != string.Empty && orderBO.orderDisc != "")
            {
                sqlHelper.AddParameter(command, "@OrderDisc", orderBO.orderDisc, ParameterDirection.Input);
            }
            if (orderBO.userRef != null && orderBO.userRef != string.Empty && orderBO.userRef != "")
            {
                sqlHelper.AddParameter(command, "@UserRef", orderBO.userRef, ParameterDirection.Input);
            }
            if (orderBO.paymentReferenceNo > 0)
            {
                sqlHelper.AddParameter(command, "@PaymentNo", orderBO.paymentReferenceNo, ParameterDirection.Input);
            }
            if (Convert.ToInt32(orderBO.distributorId) != 0)
            {
                sqlHelper.AddParameter(command, "@DistributorIdFromMobile", Convert.ToInt32(orderBO.distributorId), ParameterDirection.Input);
            }

            if (!orderBO.syncDate.Equals(String.Empty))
            {
                sqlHelper.AddParameter(command, "@SyncDate", orderBO.syncDate, ParameterDirection.Input);
            }

            if (orderBO.orderDiscountIds != null && orderBO.orderDiscountIds != string.Empty && orderBO.orderDiscountIds != "")
            {
                sqlHelper.AddParameter(command, "@orderDiscountIds", orderBO.orderDiscountIds, ParameterDirection.Input);
            }
            if (orderBO.orderDiscountVals != null && orderBO.orderDiscountVals != string.Empty && orderBO.orderDiscountVals != "")
            {
                sqlHelper.AddParameter(command, "@OrderDiscountValues", orderBO.orderDiscountVals, ParameterDirection.Input);
            }
            if (orderBO.deliveryDate != null && orderBO.deliveryDate != string.Empty && orderBO.deliveryDate != "")
            {
                sqlHelper.AddParameter(command, "@DeliveryDate", orderBO.deliveryDate, ParameterDirection.Input);

            }
            if (orderBO.paymentMode > 0)
            {
                sqlHelper.AddParameter(command, "@PaymentMode", orderBO.paymentMode, ParameterDirection.Input);
            }
            SqlParameter outparam = command.Parameters.Add("@OutputMessage", SqlDbType.VarChar);
            outparam.Size = 50;
            outparam.Direction = ParameterDirection.Output;
            sqlHelper.ExecuteNonQuery(command);
            string result = command.Parameters["@OutputMessage"].Value.ToString();
            string id = result.Split(':')[0];
            int returnData = 0;
            try
            {
                returnData = Convert.ToInt32(id);
            }
            catch
            {
                returnData = 0;
            }
            return result;


        }
        internal Int64 GetActualAmount(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetActualAmount";
            sqlHelper.AddParameter(command, "@UserId", orderBO.OrderTakenBy, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderDate", orderBO.OrderDate, ParameterDirection.Input);
            return Convert.ToInt64(sqlHelper.ExecuteScalar(command));
        }

        internal DataSet GetInvoiceData(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "getInvoiceorCollectionDetails";
            sqlHelper.AddParameter(command, "@Mode", orderBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", orderBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", orderBO.ShopId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetOrderInvoiceDetails(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "getInvoiceorCollectionDetails";
            sqlHelper.AddParameter(command, "@Mode", orderBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@InvoiceNo", orderBO.InvoiceNo, ParameterDirection.Input);            
            return sqlHelper.ExecuteDataSet(command);
        }
        //private void writeLog(String data)
        //{
        //    String errorMsg = data;
        //    String serverPath = AppDomain.CurrentDomain.BaseDirectory;
        //    string dd=DateTime.Now.ToString().Replace(':',' ');
        //    dd = dd.Replace('/',' ');
        //    data=data.Replace(':',' ');
        //    String strXmlFilePath = serverPath + @"\" + data + dd + ".txt";
        //    File.WriteAllText(strXmlFilePath, errorMsg);
        //}

        internal DataSet GetSMSExistingCustomersOrder(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "OrderTransaction";

            command.Parameters.Add(new SqlParameter("@shopId", orderBO.ShopId));
            command.Parameters.Add(new SqlParameter("@Mode", orderBO.Mode));
            command.Parameters.Add(new SqlParameter("@ProductData", orderBO.ProductData));
            command.Parameters.Add(new SqlParameter("@QuantityData", orderBO.QuantityData));
            command.Parameters.Add(new SqlParameter("@UnitData", orderBO.UnitData));

            return sqlHelper.ExecuteDataSet(command);

        }

        internal DataSet GetOrderDispatchDetails(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspOrderDispatchDetails";
            sqlHelper.AddParameter(command, "@Mode", orderBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", orderBO.UserId, ParameterDirection.Input);            
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet UpdateDispatchDetails(OrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspOrderDispatchDetails";
            sqlHelper.AddParameter(command, "@Mode", orderBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", orderBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderId", orderBO.orderId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Priority", orderBO.Priority, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
