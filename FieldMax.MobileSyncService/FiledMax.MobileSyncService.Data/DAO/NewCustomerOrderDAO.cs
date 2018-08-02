using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
 public  class NewCustomerOrderDAO
    {

        internal int UpdateNewCustomerOrder(NewCustomerOrderBO orderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(orderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "Temp_OrderTransaction";
            sqlHelper.AddParameter(command, "@Mode", orderBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", orderBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopID", orderBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProductData", orderBO.ProductData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@QuantityData", orderBO.QuantityData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UnitData", orderBO.UnitData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Rate", orderBO.Rate, ParameterDirection.Input);
            if (orderBO.Mode == "1")
            {
                sqlHelper.AddParameter(command, "@SpecialInstnSet", orderBO.SpecialInstnSet, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@SiteAddress", orderBO.SiteAddress, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ContactPerson", orderBO.ContactPerson, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Phone", orderBO.Phone, ParameterDirection.Input);
               
            }

            if (orderBO.orderDiscountIds != null && orderBO.orderDiscountIds != string.Empty && orderBO.orderDiscountIds != "")
            {
                sqlHelper.AddParameter(command, "@orderDiscountIds", orderBO.orderDiscountIds, ParameterDirection.Input);
            }
            if (orderBO.orderDiscountVals != null && orderBO.orderDiscountVals != string.Empty && orderBO.orderDiscountVals != "")
            {
                sqlHelper.AddParameter(command, "@OrderDiscountValues", orderBO.orderDiscountVals, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@SchemeId", orderBO.SchemeId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OtherInstn", orderBO.OtherInstn, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Priority", orderBO.Priority, ParameterDirection.Input);
                     
            sqlHelper.AddParameter(command, "@FstDisc", orderBO.FirstDiscount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@secndDisc", orderBO.SecondDiscount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@AmountSet", orderBO.TotalAmount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@StockSet", orderBO.OutletStock, ParameterDirection.Input);
            if (orderBO.orderDisc != null && orderBO.orderDisc != string.Empty && orderBO.orderDisc != "")
            {
                sqlHelper.AddParameter(command, "@OrderDisc", orderBO.orderDisc, ParameterDirection.Input);
            }
            if (orderBO.mobileReferenceNo != null && orderBO.mobileReferenceNo != string.Empty && orderBO.mobileReferenceNo != "")
            {
                sqlHelper.AddParameter(command, "@MobileReferenceNo", orderBO.mobileReferenceNo, ParameterDirection.Input);
            }
            
            if (orderBO.deliveryDate != null && orderBO.deliveryDate != string.Empty && orderBO.deliveryDate != "")
            {
                sqlHelper.AddParameter(command, "@DeliveryDate", orderBO.deliveryDate, ParameterDirection.Input);

            } if (orderBO.distributorId >0)
            {
                sqlHelper.AddParameter(command, "@DistributorIdFromMobile", orderBO.distributorId, ParameterDirection.Input);
            }
            if (orderBO.paymentMode > 0)
            {
                sqlHelper.AddParameter(command, "@PaymentMode", orderBO.paymentMode, ParameterDirection.Input);
            }
            if (orderBO.ReceiptNumber != null && orderBO.ReceiptNumber != string.Empty && orderBO.ReceiptNumber != "")
            {
                sqlHelper.AddParameter(command, "@ReceiptNumber", orderBO.ReceiptNumber, ParameterDirection.Input);
            }
            if (orderBO.Mode == "2")
            {
                if (orderBO.MobileSyncDate != null && orderBO.MobileSyncDate != string.Empty && orderBO.MobileSyncDate != "")
                {
                    sqlHelper.AddParameter(command, "@MobileSyncDate", orderBO.MobileSyncDate, ParameterDirection.Input);
                }
                if (orderBO.OrderDate != null && orderBO.OrderDate != string.Empty && orderBO.OrderDate != "")
                {
                    sqlHelper.AddParameter(command, "@OrderDate", orderBO.OrderDate, ParameterDirection.Input);
                }
            }
            
            if (orderBO.FreeQuantity != null && orderBO.FreeQuantity != string.Empty && orderBO.FreeQuantity != "")
            {
                sqlHelper.AddParameter(command, "@FreeQuantity", orderBO.FreeQuantity, ParameterDirection.Input);
            }
            if (orderBO.UnitDiscount != null && orderBO.UnitDiscount != string.Empty && orderBO.UnitDiscount != "")
            {
                sqlHelper.AddParameter(command, "@UnitDiscount", orderBO.UnitDiscount, ParameterDirection.Input);
            }
            if (orderBO.MobileDiscountFlag != null && orderBO.MobileDiscountFlag != string.Empty && orderBO.MobileDiscountFlag != "")
            {
                sqlHelper.AddParameter(command, "@MobileDiscountFlag", orderBO.MobileDiscountFlag, ParameterDirection.Input);
            }
            if (orderBO.TotalDiscount != null && orderBO.TotalDiscount != string.Empty && orderBO.TotalDiscount != "")
            {
                sqlHelper.AddParameter(command, "@TotalDiscountData", orderBO.TotalDiscount, ParameterDirection.Input);
            }
            if (orderBO.TaxAmount != null && orderBO.TaxAmount != string.Empty && orderBO.TaxAmount != "")
            {
                sqlHelper.AddParameter(command, "@TaxAmountData", orderBO.TaxAmount, ParameterDirection.Input);
            }

            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }


        internal DataSet GetSMSOrderDetails(NewCustomerOrderBO newCustomerOrderBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(newCustomerOrderBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "Temp_OrderTransaction";
            command.Parameters.Add(new SqlParameter("@shopId", newCustomerOrderBO.ShopId));
            command.Parameters.Add(new SqlParameter("@Mode", newCustomerOrderBO.Mode));
            command.Parameters.Add(new SqlParameter("@ProductData", newCustomerOrderBO.ProductData));
            command.Parameters.Add(new SqlParameter("@QuantityData", newCustomerOrderBO.QuantityData));
            command.Parameters.Add(new SqlParameter("@UnitData", newCustomerOrderBO.UnitData));
            return sqlHelper.ExecuteDataSet(command);

        }
    }
}
