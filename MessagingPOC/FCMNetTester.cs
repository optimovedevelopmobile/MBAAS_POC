using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FCM.Net;
using Newtonsoft.Json;

namespace MessagingPOC{

public class FCMTester{


    public async void SenderToFCMAsync(String msg)
    {

        var registrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
        var serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";
        using (var sender = new Sender(serverKey))
        {
            var message = new Message
            {
                RegistrationIds = new List<string> { registrationId },
                Notification = new Notification
                {
                    Title = "Test from FCM.Net",
                    Body = $"Hello World@!{DateTime.Now.ToString()}"
                }
                
            };
            Task<ResponseContent> response = sender.SendAsync(message);
            ResponseContent result =  await response;
            Console.WriteLine($"Success: {result.MessageResponse.Success}");

            var json = "{\"notification\":{\"title\":\"mensagem em json\",\"body\":\"funciona!\"},\"to\":\"" + registrationId + "\"}";
            result = await sender.SendAsync(json);
            Console.WriteLine($"Success: {result.MessageResponse.Success}");
            Console.WriteLine("Exiting SenderToFCMAsync");
        }
    }


     public async void SenderDataToFCMAsync(String dataJson)
    {
       // Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataJson);

        var registrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
        var serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";

        try{
using (var sender = new Sender(serverKey))
        {
            var message = new Message
            {
                RegistrationIds = new List<string> { registrationId },
                Data = dataJson,
            //     new Dictionary<string, string>
            // {
            //      { "content", "BigText FCM.NET Text Content" },
            //        { "title", "BigText Text Title" },
            //         { "veryLongText", "1 The quick brown" },
            //         { "type", "BigText" }
            // }   
                
            };
            Task<ResponseContent> response = sender.SendAsync(message);
            ResponseContent result =  await response;
            Console.WriteLine($"Success: {result.MessageResponse.Success}");

            var json = "{\"notification\":{\"title\":\"mensagem em json\",\"body\":\"funciona!\"},\"to\":\"" + registrationId + "\"}";          
            Console.WriteLine($"Success: {result.MessageResponse.Success}");
            Console.WriteLine("Exiting SenderToFCMAsync");
        }
        
        }catch(Exception e)
        {

            Console.WriteLine(e.Message);
        }
        
    }

     public async Task<bool> SenderDataToFCMAsync2(String data)
        {
        
        bool bStatus = true;
            var registrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
            var serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";
var sender = new Sender(serverKey);
            try {

            var message = new Message
        {
            RegistrationIds = new List<string> { registrationId },
                        Data = new Dictionary<string, string>
                {
                    { "content", "BigText FCM.NET Text Content ............. " },
                    { "title", "BigText Text Title" },
                        { "veryLongText", " The quick brown fox jumps over the lazy dog" },
                        { "type", "BigText" }
                }

        };

            // using (var sender = new Sender(serverKey))
            // {
            //     Console.WriteLine(" SenderToFCMAsync  1");
            //     var message = new Message
            //     {
            //         RegistrationIds = new List<string> { registrationId },
            //                     Data = new Dictionary<string, string>
            //             {
            //                 { "content", "BigText FCM.NET Text Content ............. " },
            //                 { "title", "BigText Text Title" },
            //                     { "veryLongText", " The quick brown fox jumps over the lazy dog" },
            //                     { "type", "BigText" }
            //             }

            //     };
                Console.WriteLine(" SenderToFCMAsync  2");
                Task<ResponseContent> response = sender.SendAsync(message);
                
                Console.WriteLine(" SenderToFCMAsync  3");
                ResponseContent result = await response;      
                Console.WriteLine(" SenderToFCMAsync  4");
                bStatus  = result.MessageResponse.Success == 1;
                Console.WriteLine($"Success: {result.MessageResponse.Success}");
                Console.WriteLine("Exiting SenderToFCMAsync");
                sender.Dispose();

            //}
            
            }catch(Exception e)
            {
                  sender.Dispose();
                Console.WriteLine("error Sender " + e.Message);
            }
           
            


            return bStatus;
        }
}



}