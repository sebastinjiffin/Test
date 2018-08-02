using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public class SignatureBO:BOBase
   {
       SignatureDAO signatureDAO = new SignatureDAO();

       #region Properties
       public int UserId { get; set; }
       public int OrderId { get; set; }
       public int ShopId { get; set; }
       public string Data { get; set; }
       public string Shopkeepername { get; set; }
       public string Latitude { get; set; }
       public string Longitude { get; set; }
       public string ProcessName { get; set; }
       public int Mode { get; set; }
       public string MobileReferenceNo { get; set; }
       public int GpsSource { get; set; }
       public string MobileSyncDate { get; set; }
       public string ServerSyncDate { get; set; }
       public string ProcessId { get; set; }

       #endregion

       #region Methods
       public int UpdateSignature()
       {
           return signatureDAO.UpdateSignature(this);
       }
       #endregion

       #region Methods
       public string GetProcessName()
       {
           return signatureDAO.GetProcessName(this);
       }
       #endregion
   }
}
