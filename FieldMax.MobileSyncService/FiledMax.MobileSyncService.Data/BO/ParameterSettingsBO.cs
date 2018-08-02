using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ParameterSettingsBO : BOBase
    {
        #region Properties

        public int isDateWiseBeat { get; set; }
        public int UserId { get; set; }
        public DateTime date { get; set; }
        #endregion
        #region Methods
        ParameterSettingsDAO parameterSettingsDAO = new ParameterSettingsDAO();
        public int getIsDateWiseBeat()
        {
            return parameterSettingsDAO.getIsDateWiseBeat(this);
        }
        public int getSqliteCount()
        {
            return parameterSettingsDAO.getSqliteCount(this);
        }
         public int getIsDailyBeat()
        {
            return parameterSettingsDAO.getIsDailyBeat(this);
        }

        public int getRequireErb()
        {
            return parameterSettingsDAO.getRequireErb(this);
        }
        public int RequireSMSForOrder()
        {
            return parameterSettingsDAO.RequireSMSForOrder(this);
        }
        public int RequireSMSForcollection()
        {
            return parameterSettingsDAO.RequireSMSForcollection(this);
        }

        public string isValidDeleiveryDateForMilma()
        {
            return parameterSettingsDAO.isValidDeleiveryDateForMilma(this);
        }
        public DataSet GetUserParameters()
        {
            return parameterSettingsDAO.GetUserParameters(this);
        }
        #endregion
    }
}
