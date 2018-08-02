using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;


namespace FieldMax.MobileSyncService.Data.BO
{
    public class ImeiBO:BOBase
    {
        ImeiDAO imeiDAO = new ImeiDAO();

        #region Properties

        public int UserId { get; set; }
        public string Imei { get; set; }
        public string AndroidVersion { get; set; }
        public int SignalStrength { get; set; }
        public string networkProvider { get; set; }
        #endregion

        #region Methods

        public int UpdateImei()
        {
            return imeiDAO.UpdateImei(this);
        }

        public int LogImei()
        {
            return imeiDAO.LogImei(this);
        }

        #endregion
    }
}
