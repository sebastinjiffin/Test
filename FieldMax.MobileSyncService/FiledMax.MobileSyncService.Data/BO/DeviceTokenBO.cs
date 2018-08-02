using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
  public class DeviceTokenBO:BOBase
    {
      DeviceTokenDAO deviceTokenDAO = new DeviceTokenDAO();

        #region Properties

        public int UserId { get; set; }
        public string Mode { get; set; }
        public string DeviceId { get; set; }        

        #endregion

        #region Methods

        public int UpdateDeviceToken()
        {
            return deviceTokenDAO.UpdateDeviceToken(this);
        }

        public DataSet GetDeviceId()
        {
            return deviceTokenDAO.GetDeviceId(this);
        }

       #endregion
    }

   

}
