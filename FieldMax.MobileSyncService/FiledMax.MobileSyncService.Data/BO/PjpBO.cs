using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class PjpBO:BOBase
    {
        PjpDAO pjpDAO = new PjpDAO();

        #region Properties
        public int UserId { get; set; }
        public int Status { get; set; }
        public string Mode { get; set; }
        public string Date { get; set; }
        public string Routes { get; set; }
        public string Remarks { get; set; }        
        public string MobileTransactionDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string MobileSyncDate { get; set; }
        public string StatusList { get; set; }
        public string UserIdList { get; set; }
        public string CommonRemarks { get; set; }
        
        #endregion

        #region Methods
        public int updatePjp()
        {
            return pjpDAO.updatePjp(this);
        }
        public DataSet RequireMailForPjp()
        {
            return pjpDAO.RequireMailForPjp(this);
        }
        #endregion

    }
}
