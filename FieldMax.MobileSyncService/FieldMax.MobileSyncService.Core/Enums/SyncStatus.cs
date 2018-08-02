using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldMax.MobileSyncService.Core.Enums
{
    public enum SyncStatus
    {
        Submitted = 1,
        Ready = 2,
        Running = 3,
        Completed = 4,
        Failed = 5,
        Cancelled = 6
    }
}
