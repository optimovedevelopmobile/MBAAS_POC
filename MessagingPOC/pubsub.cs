using System;
// Imports the Google Cloud client library
// using Google.Apis.Pubsub.v1;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using StackExchange.Redis;

using Newtonsoft.Json;
using FCM.Net;

using System.Threading;
using System.Threading.Tasks;

using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace MessagingPOC
{

    public class QuickstartSample
    {


        public static void Main2()
        {

           PublisherClient _publisher = null;

          //  QueryPerfCounter myTimer = new QueryPerfCounter();
           // myTimer.Start();

            Stopwatch stopWatch = new Stopwatch();
            int numOfPublishThreads = 1;
            int numOfMessagesPerThread = 1;
            int numOfUsersIdPerMessage = 10;
            String serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";
            
            var pubFCM = new PublishPushMesssages();    
            stopWatch.Start();     
            pubFCM.InstantiatePublishMsgsTasks(numOfPublishThreads, numOfMessagesPerThread, numOfUsersIdPerMessage, "the Msg");
            stopWatch.Stop();
             Console.WriteLine("Publish to Pubsub Miliseconds = " + stopWatch.ElapsedMilliseconds);

            stopWatch.Start();  
            var pullWorker = new MessagesCloudPubSubPullWorker();
            pullWorker.InitializePullWorker();
            pullWorker.StartPullingFromCloudPubSub(numOfPublishThreads * numOfMessagesPerThread);
            stopWatch.Stop();
            Console.WriteLine("Read and Insert into Redis Miliseconds = " + stopWatch.ElapsedMilliseconds);

            stopWatch.Start();   
            var pullWorkerToFCM = new PendingMessagesToFCMPullWorker();           
            pullWorkerToFCM.InitializePullWorker(1, 10);
            pullWorkerToFCM.ActivatePullingTasks();            
            stopWatch.Stop();
            Console.WriteLine("PendingMessagesToFCMPullWorker Miliseconds = " + stopWatch.ElapsedMilliseconds);


            TimeSpan ts = stopWatch.Elapsed;
            
 // Format and display the TimeSpan value.
              String result = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", 
                    ts.Hours, ts.Minutes, ts.Seconds, 
                    ts.Milliseconds/10);

          Console.WriteLine(result);

           Console.WriteLine("Miliseconds = " + stopWatch.ElapsedMilliseconds);
           Console.WriteLine("Tics = " + stopWatch.ElapsedTicks);
           
           

           // myTimer.Stop();
            // Calculate time per iteration in nanoseconds
            //double result = myTimer.Duration(1);

           // Console.WriteLine("The Duration of LOading into PubSub 10 and Loading them into redis Queue \n from which we read and Execute is taking "+ result + " nanoseconds" );


            //         bool bSubscriptionAllreadyExists = false;
            //         // Instantiates a client
            //         PublisherClient publisher = PublisherClient.Create();

            //         ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1");
            //         IDatabase db = redis.GetDatabase();

            //         // Your Google Cloud Platform project ID
            //         string projectId = "pushnotificationpoc-19baf";

            //         // The name for the new topic
            //         TopicName topicName = new TopicName(projectId, "myNewTopic");
            //         // First create a topic.           
            //         //publisher.CreateTopic(topicName);

            //         // Subscribe to the topic.
            //         SubscriberClient subscriber = SubscriberClient.Create();
            //         //String  subscriptionId = "projects/pushnotificationpoc-19baf/subscriptions/myTestReader";
            //         String subscriptionId = "myTestReader";
            //         SubscriptionName subscriptionName = new SubscriptionName(projectId, subscriptionId);
            //         String t = subscriptionName.ToString();
            //         try
            //         {
            //             subscriber.CreateSubscription(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: 60);
            //         }
            //         catch (RpcException e)
            //         {
            //             if (e.Status.StatusCode == StatusCode.AlreadyExists)
            //             {
            //                 // Already exists.  That's fine.
            //                 bSubscriptionAllreadyExists = true;

            //             }
            //         }



            //     // Publish a message to the topic.
            //     PubsubMessage message = new PubsubMessage
            //     {
            //         // The data is any arbitrary ByteString. Here, we're using text.
            //         Data = ByteString.CopyFromUtf8("Hello, Pubsub"),
            //         // The attributes provide metadata in a string-to-string dictionary.
            //         Attributes =
            //         {
            //             { "description", "Simple text message" }
            //         }
            //     };
            //     publisher.Publish(topicName, new[] { message });


            //     PullResponse response = subscriber.Pull(subscriptionName, returnImmediately: true, maxMessages: 10);
            //     var currEnum = response.ReceivedMessages.GetEnumerator();
            //     var count = response.ReceivedMessages.Count;
            //     String[] acksArray = new String[count];
            //     int indx = 0;
            //     while (currEnum.MoveNext())
            //     {
            //         var msg = currEnum.Current as Google.Cloud.PubSub.V1.ReceivedMessage;
            //         acksArray[indx] = msg.AckId;
            //         indx++;
            //         // Perform logic on the item
            //         //
            //     }

            //     foreach (Google.Cloud.PubSub.V1.ReceivedMessage received in response.ReceivedMessages)
            //     {
            //         PubsubMessage msg = received.Message;
            //         Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
            //         Console.WriteLine($"Text: '{msg.Data.ToStringUtf8()}'");
            //         string jsonMsg = msg.Data.ToStringUtf8();           
            //         db.ListLeftPush("Pending", jsonMsg);           

            //         var currValue = db.ListRightPop("Pending");

            //     }
            //     subscriber.Acknowledge(subscriptionName, acksArray);


            //    var f = new FirebaseTestSender();
            //   // f.SenderToFirebaseAsync("");  //SenderToFirebaseAsync("");
            //    f.SenderDataToFirebaseAsync("");
            //     Thread.Sleep(10000);
            //     Console.WriteLine("After FirebaseTestSender");

            Thread.Sleep(100000);
            Console.WriteLine("Exiting Main");
        }

    }
}
