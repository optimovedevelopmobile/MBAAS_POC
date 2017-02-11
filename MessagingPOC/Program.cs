using System;
using StackExchange.Redis;
// Imports the Google Cloud client library
using Google.Pubsub.V1;
using System.Diagnostics;
using System.Threading;
using MessagingPOC;

namespace ConsoleApplication
{
    public class Program
    {

        public static void Main(string[] args)
        {

            PublisherClient _publisher = null;        

            Stopwatch stopWatchAll = new Stopwatch();
            Stopwatch stopWatch = new Stopwatch();
            int numOfPublishThreads = 1;
            int numOfMessagesPerThread = 10;
            int numOfUsersIdPerMessage = 100;
            String serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";

            var pubFCM = new PublishPushMesssages();
            stopWatch.Start();
            stopWatchAll.Start();
            pubFCM.InstantiatePublishMsgsTasks(numOfPublishThreads, numOfMessagesPerThread, numOfUsersIdPerMessage, "the Msg");
            stopWatch.Stop();
            Console.WriteLine("Publish to Pubsub Miliseconds = " + stopWatch.ElapsedMilliseconds);
            Console.WriteLine(" AFter Publish to PubSub");
            Stopwatch stopWatchRedis = new Stopwatch();
            stopWatchRedis.Start();
            var pullWorker = new MessagesCloudPubSubPullWorker();
            pullWorker.InitializePullWorker();
            pullWorker.StartPullingFromCloudPubSub(numOfPublishThreads * numOfMessagesPerThread);
            stopWatchRedis.Stop();
            Console.WriteLine("Read and Insert into Redis Miliseconds = " + stopWatchRedis.ElapsedMilliseconds);
            Console.WriteLine(" AFter Read PubSub Into Redis");

             Stopwatch stopWatchFcm = new Stopwatch();
            stopWatchFcm.Start();
            var pullWorkerToFCM = new PendingMessagesToFCMPullWorker();
            pullWorkerToFCM.InitializePullWorker(1, 10);
            pullWorkerToFCM.ActivatePullingTasks();
            stopWatchFcm.Stop();
            Console.WriteLine("PendingMessagesToFCMPullWorker Miliseconds = " + stopWatchFcm.ElapsedMilliseconds);

            var s = FirebaseTestSender.GetNumOfSuccessful();
            var f = FirebaseTestSender.GetNumOfFailedful();
            Console.WriteLine("Exit FCM 1 Send Status Succeeded = " + s + " Failed = " + f);
            stopWatchAll.Stop();

            Thread.Sleep(2000);
             s = FirebaseTestSender.GetNumOfSuccessful();
             f = FirebaseTestSender.GetNumOfFailedful();
            Console.WriteLine(" FCM 2 Send Status Succeeded = " + s + " Failed = " + f);
            TimeSpan ts = stopWatchAll.Elapsed;

            // Format and display the TimeSpan value.
            String result = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine(result);

            Console.WriteLine(" *************** Miliseconds =  **************  " + stopWatchAll.ElapsedMilliseconds);
            Console.WriteLine("***************  Tics =  *************** " + stopWatchAll.ElapsedTicks);



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
        public static void Main_HelloWorldRedis(string[] args)
        {
             ConnectionMultiplexer redis = null;
            Console.WriteLine("Hello World!");
            try{
               redis = ConnectionMultiplexer.Connect("127.0.0.1");
               IDatabase db = redis.GetDatabase();
               string value = "abcdefg";
                db.StringSet("mykey", value);
                db.ListLeftPush("yossi2", "two");
               int  i =1;
            }catch(Exception error){

                Console.WriteLine(error.Message);
            }
          
        }

     public static void Main_HelloWorldPubSub()
    {
        // Instantiates a client
        PublisherClient publisher = PublisherClient.Create();

        // Your Google Cloud Platform project ID
        string projectId =  "pushnotificationpoc-19baf";

        // The name for the new topic
        string topicName = "my-new-topic";

        // The fully qualified name for the new topic
        string formattedTopicName = PublisherClient.FormatTopicName(projectId, topicName);

        // Creates the new topic
        Topic topic = publisher.CreateTopic(formattedTopicName);

        Console.WriteLine($"Topic {topic.Name} created.");
    }
    }
}
