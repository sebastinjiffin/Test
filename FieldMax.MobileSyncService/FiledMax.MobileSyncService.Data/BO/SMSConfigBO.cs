using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
using Infocean.DataAccessHelper;
using System.Data;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class SMSConfigBO : BOBase
    {
        SMSconfigDAO smsConfigBO = new SMSconfigDAO();
        public string ProcessName { get; set; }
        public string ActivityId { get; set; }
        public DataSet getSmsConfiguration()
        {
            return smsConfigBO.getSmsConfiguration(this);
        }
    }
}
