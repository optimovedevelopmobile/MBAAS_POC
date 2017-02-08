using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FCM.Net;
using Newtonsoft.Json;
using MessagingPOC;



namespace MessagingPOC
{
    public class FCMData
    {

        protected Message fcmMessage { get; set; }
        //protected Message _fcmMessage = null;
        protected int _tenantId = -1;
        protected int TenantId { get; set; }

        public FCMData(int tenantId, String serverKey, String tokenId, String title, String content, PushMessageDataPayloadTypeEnum type, int msgIndex)
        {
            fcmMessage = new Message();
            fcmMessage.RegistrationIds = new List<String>() { tokenId };
            TenantId = tenantId;

            var data = new FCMDataPayloadSection(title, content, type, msgIndex);

            string json = JsonConvert.SerializeObject(data);
            fcmMessage.Data = data.DataSection;
        }

        public String GetFCMMessageJson()
        {
            string json = JsonConvert.SerializeObject(this.fcmMessage);
            return json;
        }
    }
}
