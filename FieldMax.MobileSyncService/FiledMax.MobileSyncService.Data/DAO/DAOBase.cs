/*---------------------------------------------------------------------
  Class Name: DAOBase
  Author: Shinu T.C.
  Created Date: 21/03/2011
  Description: Base class for all DAO's
  Last Modified By:  Shinu T.C. on 21/03/2011
-----------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public abstract class DAOBase
    {
        private static string conString = string.Empty;
        public static string conLicenceString = string.Empty;
        private DAOBase()
        {
            //conString = "Data source=192.168.1.2\\SQL2K5;Initial Catalog=FX_Pharma1.0.1; user ID=empuser;Password=emp@123;pooling='true';Max Pool Size=1000000";
            //conLicenceString = "Data source=192.168.1.2\\SQL2K5;Initial Catalog=FX_PharmaLicense_1.0.1; user ID=empuser;Password=emp@123;pooling='true';Max Pool Size=1000000";
        }

        /// <summary>
        /// 
        /// </summary>
        protected DateTime Today
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
