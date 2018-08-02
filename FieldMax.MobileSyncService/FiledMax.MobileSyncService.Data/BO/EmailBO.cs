using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class EmailBO : BOBase
    {
        EmailDAO emailDao = new EmailDAO();
        #region Properties

        public int id { get; set; }
        public int organisationId { get; set; }
        public string smtp { get; set; }
        public int port { get; set; }
        public Boolean defaultCredentials { get; set; }
        public Boolean enableSSL { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string emailFrom { get; set; }
        public string subject { get; set; }
        public string emailData { get; set; }
        #endregion
        #region Methods
        public EmailBO getEmailSettings()
        {
            EmailBO emailBo = new EmailBO();
            DataSet sqlDataSet = emailDao.getEmailSettings(this);
            emailBo = readDataFromReader(sqlDataSet);
            return emailBo;
        }
        internal EmailBO readDataFromReader(DataSet ds)
        {
            EmailBO emailBo = new EmailBO();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    emailBo.id =Convert.ToInt32(dr["id"].ToString().Trim());
                    emailBo.organisationId = Convert.ToInt32(dr["OrganizationId"].ToString().Trim());
                    emailBo.smtp = dr["SMTP"].ToString().Trim();
                    emailBo.port = Convert.ToInt32(dr["Port"].ToString().Trim());
                    emailBo.defaultCredentials = Convert.ToBoolean(dr["DefaultCredentials"].ToString().Trim());
                    emailBo.enableSSL = Convert.ToBoolean(dr["EnableSSL"].ToString().Trim());
                    emailBo.username = dr["UserName"].ToString().Trim();
                    emailBo.password = dr["Password"].ToString().Trim();
                    emailBo.emailFrom = dr["EmailFrom"].ToString().Trim();
                    emailBo.subject = dr["EngergiseSubject"].ToString().Trim();
                    emailBo.emailData = dr["EmailData"].ToString().Trim();
                }
            }
            return emailBo;
        }
        public string getOrganisationEmail()
        {
            EmailBO emailBo = new EmailBO();
            string result  = emailDao.getOrganistionEmail(this);
           return result;
        }
        #endregion
    }
}
