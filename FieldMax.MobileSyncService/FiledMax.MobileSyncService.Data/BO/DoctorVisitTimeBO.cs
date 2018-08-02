using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class DoctorVisitTimeBO : BOBase
    {
        #region Properties

        public string HospitalVisitId { get; set; }
        public string VisitDay { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string DoctorId { get; set; }
        #endregion

        DoctorVisitTimeDAO doctorVisitTimeDAO = new DoctorVisitTimeDAO();

        #region Methods

        public int UpdateDoctorVisitTime()
        {
            return doctorVisitTimeDAO.UpdateDoctorVisitTime(this);
        }

        public int UpdatePreferredVisitTime()
        {
            return doctorVisitTimeDAO.UpdatePreferredVisitTime(this);
        }


        #endregion
    }
}
