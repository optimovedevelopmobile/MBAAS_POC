using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingPOC
{
   

    public class PushMessageData
    {

      
        protected int TenantId { get; set; }
        protected String DataPayload { get; set; }

        public PushMessageData(int tenantId, int numOfUsersIdPerMessage, String title, String content, PushMessageDataPayloadTypeEnum type, int msgIndex)
        {
            TenantId = tenantId;

           
            var data = new FCMDataPayloadSection(title, content, type, msgIndex);
            data.SetLongText("This is my long text");

            long[] ids = new long[numOfUsersIdPerMessage];
            long currId = 1000000;
            for(int i=0; i< numOfUsersIdPerMessage; i++)
            { 
                ids.SetValue(currId, i);
                currId++;
            }
            data.SetCustomerIdsAsRegistrationIds(ids);

            DataPayload = JsonConvert.SerializeObject(data.DataSection);
        }

        public String GetFCMMessageJson()
        {
            return DataPayload;
        }
    }
}
