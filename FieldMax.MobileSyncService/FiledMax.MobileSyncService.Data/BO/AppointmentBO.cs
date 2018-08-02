using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
namespace FieldMax.MobileSyncService.Data.BO
{
    public class AppointmentBO : BOBase
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string AppointmentDate { get; set; }
        public string AppointmentDescription { get; set; }
        public string mobileSyncDate { get; set; }
        public string MobReferenceNo { get; set; }
        public int UserId { get; set; }
        public string Source { get; set; }


        AppointmentDAO AppointmentDAO = new AppointmentDAO();
        public int UpdateAppointment()
        {
            return AppointmentDAO.UpdateAppointment(this);
        }
    }
}
