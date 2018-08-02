using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System.Data;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class MobileControlsDAO
    {
        internal int isBillwiseColection(MobileControlsBO mobileControlsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(mobileControlsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT status FROM MobilePrivilegeControls where ControlId = (select ControlId from MobileControl where ControlName like '%btnChequeRemittance%')";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int isExpenseTransactionNeeded(MobileControlsBO mobileControlsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(mobileControlsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT status FROM MobilePrivilegeControls where ControlId = (select ControlId from MobileControl where ControlName like '%btnExpenseAdvance%')";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }
    }
}
