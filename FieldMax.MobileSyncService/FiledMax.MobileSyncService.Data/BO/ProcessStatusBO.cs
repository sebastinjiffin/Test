using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ProcessStatusBO : BOBase
    {
        ProcessStatusDAO processStatusDAO = new ProcessStatusDAO();
        #region Methods

        public int getOrderConfirmStatusId()
        {
            return processStatusDAO.getOrderConfirmStatusId(this);
        }

        public int getOrderCorrectionStatusId()
        {
            return processStatusDAO.getOrderCorrectionStatusId(this);
        }
        #endregion
    }
}
