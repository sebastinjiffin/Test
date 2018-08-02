using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
using System.Data;
namespace FieldMax.MobileSyncService.Data.BO
{
   public  class EmailAlertBO :BOBase
    {
        #region Properties

        public int UserId { get; set; }
        public int DistributorID { get; set; }
        public int Mode { get; set; }
        public string Status { get; set; }
     

        #endregion

        EmailAlertDAO EmailAlertDAO = new EmailAlertDAO();

        #region Methods
        public DataSet DistributorEmailTrigger()
        {
            return EmailAlertDAO.DistributorEmailTrigger(this);
        }
        public void DistributorEmailTriggerStatusLog()
        {
            EmailAlertDAO.DistributorEmailTriggerStatusLog(this);
        }
        #endregion

    }
}
