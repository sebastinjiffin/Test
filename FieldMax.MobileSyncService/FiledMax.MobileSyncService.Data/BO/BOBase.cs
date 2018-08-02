/*---------------------------------------------------------------------
  Class Name: BOBase
  Author: Shinu T.C.
  Created Date: 21/03/2011
  Description: Base class for all Bussiness objects
  Last Modified By:  Naveen on 13/10/2011
-----------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FieldMax.MobileSyncService.Data.BO
{
    public abstract class BOBase
    {
        //private string conString = string.Empty;

        public string ConString { get; set; }

    }
}
