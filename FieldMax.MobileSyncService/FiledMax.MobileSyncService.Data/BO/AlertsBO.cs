using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class AlertsBO : BOBase 
    {

        #region Properties

        public DateTime AlertDate { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        #endregion

        AlertsDAO alertsDAO = new AlertsDAO();

        #region Methods

        public int UpdateAlert()
        {
            return alertsDAO.GetAlertsCount(this);
        }

        public int GetAlertsCount()
        {
            return alertsDAO.GetAlertsCount(this);
        }

        public int UpdateMessageStatus()
        {
            return alertsDAO.UpdateMessageStatus(this);
        }

        #endregion

    }
}
