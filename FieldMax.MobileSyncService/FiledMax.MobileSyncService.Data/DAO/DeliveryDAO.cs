using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class DeliveryDAO
    {

        internal int UpdateDeliveryDate(FieldMax.MobileSyncService.Data.BO.DeliveryBO deliveryBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(deliveryBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspMobileDelivery";
            sqlHelper.AddParameter(command, "@UserId", deliveryBO.UserId, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", deliveryBO.shopId, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", deliveryBO.mode, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@BillNo", deliveryBO.billNo, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@orderId", deliveryBO.orderId, System.Data.ParameterDirection.Input);


            sqlHelper.AddParameter(command, "@productIdSet", deliveryBO.productIdSet, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@deliveredQtySet", deliveryBO.deliveryQuantitySet, System.Data.ParameterDirection.Input);
           
            sqlHelper.AddParameter(command, "@isClosed", deliveryBO.isClosed, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", deliveryBO.mobileTransactionDate, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobilereferenceNo", deliveryBO.mobileReferenceNo,ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@productUnitIdSet", deliveryBO.productUnitSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@productSchemeSet", deliveryBO.productSchemeSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@invoiceQtySet", deliveryBO.invoiceQtySet, ParameterDirection.Input);

            sqlHelper.AddParameter(command, "@Latitude ", deliveryBO.latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Longitude", deliveryBO.longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ProcessName", deliveryBO.processName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", deliveryBO.gpsSource, ParameterDirection.Input);

            if (deliveryBO.signalStrength != null && deliveryBO.signalStrength != string.Empty && deliveryBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", deliveryBO.signalStrength, ParameterDirection.Input);
            }
            if (deliveryBO.networkProvider != null && deliveryBO.networkProvider != string.Empty && deliveryBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", deliveryBO.networkProvider, ParameterDirection.Input);
            }

            if (deliveryBO.returnReasonId != null && deliveryBO.returnReasonId != string.Empty && deliveryBO.returnReasonId != "")
            {
                sqlHelper.AddParameter(command, "@returnReasonIdSet", deliveryBO.returnReasonId, ParameterDirection.Input);
            }




            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal DataSet getDeliveryDetails(FieldMax.MobileSyncService.Data.BO.DeliveryBO deliveryBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(deliveryBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspMobileDelivery";
            sqlHelper.AddParameter(command, "@UserId", deliveryBO.UserId, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", deliveryBO.mode, System.Data.ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", deliveryBO.shopId, System.Data.ParameterDirection.Input);
           // return sqlHelper.ExecuteDataSet(command);
            return sqlHelper.ExecuteDataSet(command);
        }
        internal DataSet getBills(FieldMax.MobileSyncService.Data.BO.DeliveryBO deliveryBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(deliveryBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetShipmentBillNo";
           sqlHelper.AddParameter(command, "@ShopId", deliveryBO.shopId, System.Data.ParameterDirection.Input);
            // return sqlHelper.ExecuteDataSet(command);
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
