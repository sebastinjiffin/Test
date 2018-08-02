using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class PhotoCaptureDAO
    {
       internal int UpdatePhotoCapture(PhotoCaptureBO photoCaptureBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(photoCaptureBO.ConString);

           if (photoCaptureBO.mode == 5)
           {
               SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
               command.CommandText = "PhotoGallary";
               sqlHelper.AddParameter(command, "@Mode", photoCaptureBO.mode, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@UserId", photoCaptureBO.UserId, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ShopId", photoCaptureBO.ShopId, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ImageName", photoCaptureBO.ImageName, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@latitude", photoCaptureBO.Latitude, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@longitude", photoCaptureBO.Longitude, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ImageDescription", photoCaptureBO.ImageDescription, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@MobileRefNumber", photoCaptureBO.mobReferenceNumber, ParameterDirection.Input);
               //sqlHelper.AddParameter(command, "@ImageData", photoCaptureBO.imageData, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ProcessIdVal", photoCaptureBO.ProcessId, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@CapturedDate", photoCaptureBO.DateTime, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@SyncDate", photoCaptureBO.SyncDate, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@GpsSource", photoCaptureBO.GpsSource, ParameterDirection.Input);
               if (photoCaptureBO.descriptionTypeId != 0)
               {
                   sqlHelper.AddParameter(command, "@DescTypeId", photoCaptureBO.descriptionTypeId, ParameterDirection.Input);
               }
               if (photoCaptureBO.ProcessDetailsId != null)
               {
                   sqlHelper.AddParameter(command, "@ProcessDetailId", photoCaptureBO.ProcessDetailsId, ParameterDirection.Input);
               }
               if (!string.IsNullOrEmpty(photoCaptureBO.TempShopId))
               {
                   sqlHelper.AddParameter(command, "@TempShopId", photoCaptureBO.TempShopId, ParameterDirection.Input);
               }
               if (photoCaptureBO.signalStrength != null && photoCaptureBO.signalStrength != string.Empty && photoCaptureBO.signalStrength != "")
               {
                   sqlHelper.AddParameter(command, "@signalStrength", photoCaptureBO.signalStrength, ParameterDirection.Input);
               }
               if (photoCaptureBO.networkProvider != null && photoCaptureBO.networkProvider != string.Empty && photoCaptureBO.networkProvider != "")
               {
                   sqlHelper.AddParameter(command, "@networkProvider", photoCaptureBO.networkProvider, ParameterDirection.Input);
               }
               return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
           }
           else
           {
               SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
               command.CommandText = "PhotoGallary";
               sqlHelper.AddParameter(command, "@Mode", photoCaptureBO.mode, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@UserId", photoCaptureBO.UserId, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ShopId", photoCaptureBO.ShopId, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ImagePath", photoCaptureBO.ImagePath, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ImageName", photoCaptureBO.ImageName, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@latitude", photoCaptureBO.Latitude, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@longitude", photoCaptureBO.Longitude, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@processName", photoCaptureBO.ProcessName, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ImageDescription", photoCaptureBO.ImageDescription, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@EmailRecipients", photoCaptureBO.emails, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@MobileRefNumber", photoCaptureBO.mobReferenceNumber, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@GpsSource", photoCaptureBO.GpsSource, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@CapturedDate", photoCaptureBO.DateTime, ParameterDirection.Input);
               if (photoCaptureBO.descriptionTypeId != 0)
               {
                   sqlHelper.AddParameter(command, "@DescTypeId", photoCaptureBO.descriptionTypeId, ParameterDirection.Input);
               }
               if (photoCaptureBO.ProcessDetailsId != 0)
               {
                   sqlHelper.AddParameter(command, "@ProcessDetailId", photoCaptureBO.ProcessDetailsId, ParameterDirection.Input);
               }
               if (!string.IsNullOrEmpty(photoCaptureBO.TempShopId))
               {
                   sqlHelper.AddParameter(command, "@TempShopId", photoCaptureBO.TempShopId, ParameterDirection.Input);
               }
               if (photoCaptureBO.signalStrength != null && photoCaptureBO.signalStrength != string.Empty && photoCaptureBO.signalStrength != "")
               {
                   sqlHelper.AddParameter(command, "@signalStrength", photoCaptureBO.signalStrength, ParameterDirection.Input);
               }
               if (photoCaptureBO.networkProvider != null && photoCaptureBO.networkProvider != string.Empty && photoCaptureBO.networkProvider != "")
               {
                   sqlHelper.AddParameter(command, "@networkProvider", photoCaptureBO.networkProvider, ParameterDirection.Input);
               }
               return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
           }
           
       }

       internal DataSet GetImagePath(PhotoCaptureBO photoCaptureBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(photoCaptureBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "getImagePath";
           return (sqlHelper.ExecuteDataSet(command));
       }
    }
}
