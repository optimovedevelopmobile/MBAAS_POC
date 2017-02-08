using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingPOC
{
    public class TenantFCMMetaData
    {

        public TenantFCMMetaData(int tenantId, String serverKey)
        {

            TenantId = tenantId;
            ServerKey = serverKey;
        }
        public int TenantId { get; set; }
        public String ServerKey { get; set; }

    }
}
