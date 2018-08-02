using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class IncentiveDashboardBO : BOBase
    {
        IncentiveDashboardDAO incentiveDashboardDAO = new IncentiveDashboardDAO();
        
        #region Properties
        
        public string UserId { get; set; }
        public string Mode { get; set; }
        
        #endregion

        #region Methods

        public DataSet GetPeriod()
        {
            return incentiveDashboardDAO.GetPeriod(this);
        }

        public DataSet GetPerformanceTarget()
        {
            return incentiveDashboardDAO.GetPerformanceTarget(this);
        }

        public DataSet GetPerformanceAchieved()
        {
            return incentiveDashboardDAO.GetPerformanceAchieved(this);
        }

        public DataSet GetSKUDriveTarget()
        {
            return incentiveDashboardDAO.GetSKUDriveTarget(this);
        }

        public DataSet GetSKUDriveAchieved()
        {
            return incentiveDashboardDAO.GetSKUDriveAchieved(this);
        }

        #endregion
    }
}
