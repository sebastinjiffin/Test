using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class ChequeRemittanceDAO
    {
        internal int UpdateChequeRemittance(ChequeRemittanceBO chequeRemittanceBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(chequeRemittanceBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);



            command.CommandText = "uspRemittance";
            sqlHelper.AddParameter(command, "@Mode", chequeRemittanceBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", chequeRemittanceBO.mobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Remarks", chequeRemittanceBO.Remarks, ParameterDirection.Input);
            if (chequeRemittanceBO.BankId != 0)
            {
                sqlHelper.AddParameter(command, "@BankId", chequeRemittanceBO.BankId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@BankIdSet", chequeRemittanceBO.BankId, ParameterDirection.Input);
            }

            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));

        }
    }
}

