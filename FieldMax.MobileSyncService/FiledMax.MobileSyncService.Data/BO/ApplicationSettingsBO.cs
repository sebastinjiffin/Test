using System;
using System.Collections.Generic;
using System.Linq;
using FieldMax.MobileSyncService.Data.DAO;
using System.Text;
using System.Data;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ApplicationSettingsBO : BOBase
    {
        #region Variables
        ApplicationSettingsDAO objApplicationSettingsDAO = new ApplicationSettingsDAO();
        #endregion

        #region Properties
        public string Mode { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }

        #endregion

        #region Methods
        public DataSet GetApplicationSettings(ApplicationSettingsBO objApplicationSettingsBO)
        {
            return objApplicationSettingsDAO.GetApplicationSettings(objApplicationSettingsBO);
        }
        public DataSet GetEditableFlag(ApplicationSettingsBO objApplicationSettingsBO)
        {
            return objApplicationSettingsDAO.GetEditableFlag(objApplicationSettingsBO);
        }
        public DataSet GetShopType(ApplicationSettingsBO objApplicationSettingsBO)
        {
            return objApplicationSettingsDAO.GetShopType(objApplicationSettingsBO);
        }
        public string GetUserRole()
        {
            return objApplicationSettingsDAO.GetUserRole(this);
        }
        public DataSet GetModifiedDate()
        {
            return objApplicationSettingsDAO.GetModifiedDate(this);
        }
        #endregion
    }
}
