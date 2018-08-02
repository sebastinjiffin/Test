using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class VersionLogBO : BOBase 
    {
        #region Properties

        public int UserId { get; set; }
        public string VersionNo { get; set; }
        public string BuildNo { get; set; }
        public string Mode { get; set; }

        #endregion

        VersionLogDAO versionLogDAO = new VersionLogDAO();

        #region Methods

        public int GetVersionLog()
        {
            return versionLogDAO.GetVersionLog(this);
        }

        public DataSet GetVersionNo()
        {
            return versionLogDAO.GetVersionNo(this);
        }

        public DataSet GetBuildNo()
        {
            return versionLogDAO.GetBuildNo(this);
        }

        public int UpdateVersionLog()
        {
            return versionLogDAO.UpdateVersionLog(this);
        }

        #endregion

    }
}
