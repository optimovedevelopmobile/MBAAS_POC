using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FirebaseNet.Messaging;
using Newtonsoft.Json;

namespace MessagingPOC {

public class FirebaseTestSender
{

    protected static int _msgCounter = 0;
    protected static int _sendingInstances = 0;
    public async Task<bool> SenderToFirebaseAsync2(String dataJson)
    {
        bool bStatus = true;
         var ind = dataJson.IndexOf("Index");
         var indexVal = dataJson.Substring(ind, 10);
        Console.WriteLine("Processing Messsage " + indexVal);

    var registrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
        var serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";
        FCMClient client = new FCMClient(serverKey);
        var message = new Message
        {
            To = registrationId,
            Notification = new AndroidNotification()
            {
                Body = "great match!",
                Title = "Portugal vs. Denmark",
                Icon = "myIcon"
            }   
        };

        try{
      
            DownstreamMessageResponse result = (DownstreamMessageResponse) await client.SendMessageAsync(message);
             Console.WriteLine("After FCMClient: SendMessageAsync");
              Console.WriteLine($"Success: {result.Success} " + " Message" + indexVal);
        }catch(Exception error)
        {

            Console.WriteLine($"Text: '{error.Message}'");

            
        }
       return bStatus;
    }

      public async void SenderToFirebaseAsync(String msg)
    {
    var registrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
        var serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";
        FCMClient client = new FCMClient(serverKey);
        var message = new Message
        {
            To = registrationId,
            Notification = new AndroidNotification()
            {
                Body = "great match!",
                Title = "Portugal vs. Denmark",
                Icon = "myIcon"
            }   
        };

        try{
      
            var result = await client.SendMessageAsync(message);
             Console.WriteLine("After FCMClient: SendMessageAsync");
        }catch(Exception error)
        {

            Console.WriteLine($"Text: '{error.Message}'");

            
        }
       
    }


     public async Task<bool> SenderDataToFirebaseAsync(String dataJson)
    {

        bool bStatus = true;

        Interlocked.Increment(ref _sendingInstances);
        while(_sendingInstances > 100)
        {
            Thread.Sleep(50);
        }
        var ind = dataJson.IndexOf("Index");
         var indexVal = dataJson.Substring(ind, 10);
        Console.WriteLine("Processing Messsage " + indexVal);
     

    var registrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
        var serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";
        FCMClient client = new FCMClient(serverKey);
        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataJson);

        var message = new Message
        {
            To = registrationId,
            Data =   values
            //Dictionary<string, string>
            // {
            //      { "content", "BigText Text Content ............. " },
            //        { "title", msg },
            //         { "veryLongText", "1 The quick brown fox jumps over the lazy dog \n 2 The quick brown fox jumps over the lazy dog \n 3 The quick brown fox jumps over the lazy dog \n 4 The quick brown fox jumps over the lazy dog " },
            //         { "type", "BigText" }
            // }   
        };

        try{
      
              
            DownstreamMessageResponse result = (DownstreamMessageResponse) await client.SendMessageAsync(message);
            Console.WriteLine("After FCMClient: SendMessageAsync");
            Console.WriteLine($"********* Success: {result.Success} " + "\n Message " + indexVal + " SendingInstances = " + _sendingInstances);
            
        }catch(Exception error)
        {

            Console.WriteLine($"Text: '{error.Message}'");

            Interlocked.Decrement(ref _sendingInstances);
        }
        Interlocked.Decrement(ref _sendingInstances);

        return bStatus;
    }
}
}
