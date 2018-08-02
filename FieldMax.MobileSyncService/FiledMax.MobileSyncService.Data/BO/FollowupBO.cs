using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public class FollowupBO:BOBase
    {
       FollowupDAO followupDAO = new FollowupDAO();

       #region Properties

       public int UserId { get; set; }
       public int ShopId { get; set; }
       public int FollowUpId { get; set; }
       public string Remarks { get; set; }
       public DateTime? FollowUpDate { get; set; }
       public string Latitude { get; set; }
       public string Longitude { get; set; }
       public string ProcessName { get; set; }
       public string networkProvider { get; set; }
       public string signalStrength { get; set; }
       #endregion

       #region Methods
       public int UpdateFollowUp()
       {
           return followupDAO.UpdateFollowUp(this);
       }

       public int GetFollowUp()
       {
           return followupDAO.GetFollowUp(this);
       }
       #endregion

    }
}
