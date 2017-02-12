using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingPOC
{

    public enum PushMessageDataPayloadTypeEnum
    {
            
            None = 0,
            StyleInbox  = 1,
            BigText     = 2,
            BigPicture  = 3,
            CustomView  = 4,



    }

    public class FCMDataPayloadSection
    {

        public Dictionary<String, String> DataSection { get; set; }
        protected PushMessageDataPayloadTypeEnum _type = PushMessageDataPayloadTypeEnum.None;
        public FCMDataPayloadSection(String titleArg, String contentArg, PushMessageDataPayloadTypeEnum typeArg, int msgIndex)
        {
            msg_index = msgIndex.ToString();
            _type = typeArg;
            title = titleArg ;
            content = contentArg ;
            type = typeArg.ToString();
            dry_run = "true";
            DataSection = new Dictionary<String, String>();

            if (String.IsNullOrEmpty(registration_ids) == false)
                DataSection.Add("registration_ids", registration_ids);

            if (String.IsNullOrEmpty(to) == false)
                DataSection.Add("to", to);

            if (String.IsNullOrEmpty(title) == false)
                DataSection.Add("title", title);

            if (String.IsNullOrEmpty(content) == false)
                DataSection.Add("content", content);

            if (String.IsNullOrEmpty(type) == false)
                DataSection.Add("type", type);

            if (String.IsNullOrEmpty(dry_run) == false)
                DataSection.Add("dry_run", dry_run);

            if (String.IsNullOrEmpty(imageurl) == false)
                DataSection.Add("imageurl", imageurl);

            if (String.IsNullOrEmpty(bigImageurl) == false)
                DataSection.Add("bigImageurl", bigImageurl);

            if (String.IsNullOrEmpty(msg_index) == false)
                DataSection.Add("msg_index", msg_index);
             
        }

        public void SetRegistrationIds(String[] ids)
        {
            registration_ids += ids[0];
            if (ids.Length > 1)
            {
                for (int indx = 1; indx < ids.Length; indx++)
                {
                    registration_ids += ", " + ids[indx];
                }
            }

        }

        public void SetCustomerIdsAsRegistrationIds(long[] customerIds)
        {
            customer_ids += customerIds[0];
            if (customerIds.Length >= 1)
            {
                for (int indx = 1; indx < customerIds.Length; indx++)
                {
                    customer_ids += ", " + customerIds[indx];
                }

                
             if (String.IsNullOrEmpty(customer_ids) == false)
                DataSection.Add("customer_ids", customer_ids);   
            }

        }

         public void SetLongText(string txt)
        {
                           
             if (_type == PushMessageDataPayloadTypeEnum.BigText  && String.IsNullOrEmpty(txt) == false)
             {

                 veryLongText = txt;
                 DataSection.Add("veryLongText", veryLongText);  
             }
                         
        }

          public void SetBigImage(string imageUrl)
        {
                           
             if (_type == PushMessageDataPayloadTypeEnum.BigPicture  && String.IsNullOrEmpty(imageUrl) == false)
             {
                bigImageurl = imageurl;
                DataSection.Add("bigImageurl", bigImageurl);           
             }
               
        }

        
       

        #region Properties
        public string to { get; set; }
        public string registration_ids { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string type { get; set; }
        public string imageurl { get; set; }
        public string bigImageurl { get; set; }
        public string dry_run { get; set; }
        public string customer_ids { get; set; }

        public string veryLongText { get; set; }

         public String msg_index { get; set; }
        #endregion
    }
}
