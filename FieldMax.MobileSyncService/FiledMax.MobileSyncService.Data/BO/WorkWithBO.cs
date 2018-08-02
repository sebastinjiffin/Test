using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class WorkWithBO : BOBase
    {
        #region Properties

        public int userId { get; set; }
        public string Name { get; set; }
        public string userIdSet { get; set; }
        public string processName { get; set; }
        public string lattitude { get; set; }
        public string longitude { get; set; }
        public DateTime mobileCaptureDate { get; set; }
        public string shopId { get; set; }
        public string mobRefNo { get; set; }
        public int gpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        public string Others { get; set; }
        public string DepartmentIdSet { get; set; }
        #endregion

        WorkWithDAO workWithDAO = new WorkWithDAO();

        #region Methods

        public DataSet GetWorkWithMasters()
        {
            return workWithDAO.GetWorkWithMasters(this);
        }

        public int updateWorkWith()
        {
            return workWithDAO.updateWorkWith(this);
        }

       

        #endregion

    }
}

