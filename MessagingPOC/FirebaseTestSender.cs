using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FirebaseNet.Messaging;
using Newtonsoft.Json;

namespace MessagingPOC {

public class FirebaseTestSender
{

    public static ConcurrentDictionary<int,bool>  MessagesStatus { get; set; }
    protected static long _numOfSuccessfuleMsg = 0;
    protected static long _numOfFailedeMsg = 0;
    protected static int _msgCounter = 0;
    protected static volatile int _sendingInstances = 0;

     protected static Object _lock = new Object();
    

    public static void Init()
    {
        lock(_lock)
        {
            if(MessagesStatus == null)
                 MessagesStatus = new ConcurrentDictionary<int,bool>();
        }
        
    }
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
        while(_sendingInstances > 90)
        {
            Thread.Sleep(50);
        }
        var ind = dataJson.IndexOf("Index");
         var indexVal = dataJson.Substring(ind, 12);
        //Console.WriteLine("Processing Messsage " + indexVal + " CurrentTghreadId = " + Thread.CurrentThread.ManagedThreadId);
     

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


             Stopwatch stopWatchFcm = new Stopwatch();
            
        try{
            stopWatchFcm.Start();
            var size = dataJson.Length;  
           //Console.WriteLine($"Message Size: '{size}'");
            DownstreamMessageResponse result = (DownstreamMessageResponse) await client.SendMessageAsync(message);
         //   Console.WriteLine("After FCMClient: SendMessageAsync");
            
            if(1 == result.Success)
            {
                //Console.WriteLine($"********* Success: {result.Success} " + "\n Message " + indexVal + " SendingInstances = " + _sendingInstances);
                Interlocked.Increment(ref _numOfSuccessfuleMsg);
            }else{
                Interlocked.Increment(ref _numOfFailedeMsg);
                Console.WriteLine($"********* Failed: {result.Failure} " + "\n Message " + indexVal + " SendingInstances = " + _sendingInstances);
                
            }

            stopWatchFcm.Stop();
        }catch(Exception error)
        {

            Interlocked.Increment(ref _numOfFailedeMsg);
            Console.WriteLine($"********* Failed:" + "\n Message " + indexVal + " SendingInstances = " + _sendingInstances);
     
            Console.WriteLine($"Text: '{error.Message}'" );

           bStatus = false;
        }
        Interlocked.Decrement(ref _sendingInstances);

        return bStatus;
    }




     public async Task<bool> SenderDataPayloadToFirebaseAsync(Dictionary<String, String> data, String [] recievedRegistration_ids,  FCMClient client   )
    {

        bool bStatus = true;
       
        Interlocked.Increment(ref _sendingInstances);
        while(_sendingInstances > 80)
        {
            Thread.Sleep(2);
        }
        var ind = data["msg_index"];
         

        var registrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
        var serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";
        
        client.TestMode  = true;
        int msg_id = Convert.ToInt32(data["msg_index"]);
        FirebaseTestSender.MessagesStatus.AddOrUpdate(msg_id, false, (k,v) => false);
        var message = new Message
        {
            RegistrationIds  = recievedRegistration_ids,
            Data =   data            
        };
       
        try{
          
            var size = JsonConvert.SerializeObject(message).Length;
                //    Console.WriteLine($"********* Success: 100 " + "\n Message " + ind + " SendingInstances = " + _sendingInstances);
                //     Thread.Sleep(100);

                //Console.WriteLine($"Message Size: '{size}'");
            Task<IFCMResponse>  taskResponse = (Task<IFCMResponse>) client.SendMessageAsync(message);
            //Thread.Sleep(1);
            DownstreamMessageResponse result =  (DownstreamMessageResponse) await taskResponse;

          
            Interlocked.Add(ref _numOfSuccessfuleMsg, result.Success);
            Interlocked.Add(ref _numOfFailedeMsg, result.Failure);
            Interlocked.Decrement(ref _sendingInstances);
            if(result.Success >= 1)
            {
             //   Console.WriteLine($"********* Success: {_numOfSuccessfuleMsg} " + "\n Message " + ind + " SendingInstances = " + _sendingInstances);
              
            }else{
                
             //    Console.WriteLine($"********* Failed: {_numOfFailedeMsg} " + "\n Message " + ind + " SendingInstances = " + _sendingInstances);
                
            }

           


            // Interlocked.Add(ref _numOfSuccessfuleMsg, 100);
            // Interlocked.Add(ref _numOfFailedeMsg, 0);
        }catch(Exception error)
        {

           Interlocked.Add(ref _numOfFailedeMsg, recievedRegistration_ids.Length);
            Console.WriteLine($"********* Failed:" + "\n Message " + ind + " SendingInstances = " + _sendingInstances);
     
            Console.WriteLine($"Text: '{error.Message}'" );

           bStatus = false;
        }
       
       MessagesStatus.AddOrUpdate(msg_id, true, (k,v) => true);
    
        return bStatus;
    }


    public static long GetNumOfSuccessful(){return _numOfSuccessfuleMsg;}
    public static long GetNumOfFailedful(){return _numOfFailedeMsg;}
}
}
