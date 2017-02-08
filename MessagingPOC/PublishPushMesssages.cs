using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

// Imports the Google Cloud client library
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using StackExchange.Redis;
using MessagingPOC;

public class PublishPushMesssages
{

    #region Members
    protected static int _publishedMsgCount = 0;

    #region Redis

    static ConnectionMultiplexer _redis = null;
    static IDatabase _redisDB = null;
    #endregion

    #region CloudPubSub
    bool _bSubscriptionAllreadyExists = false;
    static PublisherClient _publisher = null;
    static SubscriberClient _subscriber = null;
    static SubscriptionName _subscriptionName = null;
    static string DefaultProjectId = "pushnotificationpoc-19baf";
    static string DefaultRedisServerAddress = "127.0.0.1";
    static string DefaultTopicName = "myNewTopic";

    static TopicName CurrentTopicName;
    String DefaultSubscriptionId = "myTestReader";
    #endregion

    #region FCM
    static String RegistrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
    static String serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";

    #endregion
    #endregion

    #region PublicMethods
    /// <summary>
    /// Initializes a new instance of the <see cref="PublishFCMMessages"/> class.
    /// </summary>
    public PublishPushMesssages()
    {
        _publisher = PublisherClient.Create();

        _redis = ConnectionMultiplexer.Connect(DefaultRedisServerAddress);
        _redisDB = _redis.GetDatabase();
        CurrentTopicName = new TopicName(DefaultProjectId, DefaultTopicName);
        _subscriber = SubscriberClient.Create();
        //String  subscriptionId = "projects/pushnotificationpoc-19baf/subscriptions/myTestReader";

        SubscriptionName subscriptionName = new SubscriptionName(DefaultProjectId, DefaultSubscriptionId);
        String t = subscriptionName.ToString();
        try
        {
            _subscriber.CreateSubscription(subscriptionName, CurrentTopicName, pushConfig: null, ackDeadlineSeconds: 60);
        }
        catch (RpcException e)
        {
            if (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
                _bSubscriptionAllreadyExists = true;

            }
        }
    }


    /// <summary>
    /// Instantiates the publish MSGS tasks.
    /// </summary>
    /// <param name="numberOfParallelTasks">The number of parallel tasks.</param>
    /// <param name="numOfMsgToPublish">The number of MSG to publish.</param>
    /// <param name="Msg">The MSG.</param>
    public void InstantiatePublishMsgsTasks(int numberOfParallelTasks, int numOfMsgToPublish, int numOfUsersIdPerMessage, String Msg)
    {
        Task[] tasks = new Task[numberOfParallelTasks];
        for (int indx = 0; indx < numberOfParallelTasks ; indx++)
        {
            tasks[indx] = Task.Factory.StartNew(() => PublishToPubSubTskDelegate(numOfMsgToPublish, numOfUsersIdPerMessage));
        }
        Task.WaitAll(tasks);
    }


    #endregion

    #region ProtectedMethods
    /// <summary>
    /// Publishes to pub sub TSK delegate.
    /// </summary>
    private static void PublishToPubSubTskDelegate(int numOfMsgToPublish, int numOfUsersIdPerMessage)
    {

        int totalSize = 0;
        int publishedMsgCount = 0;       
        PublisherClient publisher = PublisherClient.Create();
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        var listMessages = new List<PubsubMessage>();
        int count = 0;
        long customerId = 0;
        while(publishedMsgCount < numOfMsgToPublish)
        {
            
            int tenantId = 1;
            customerId++;
            int index = Interlocked.Increment(ref _publishedMsgCount);
            var currPushMsg = new PushMessageData(tenantId, numOfUsersIdPerMessage, "Title Text The quick brown fox jumps over the lazy dog ", "Content text The quick brown fox jumps over the lazy dog", PushMessageDataPayloadTypeEnum.BigText, index);
            String json = currPushMsg.GetFCMMessageJson();
            // Publish a message to the topic.
            PubsubMessage message = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.

                Data = ByteString.CopyFromUtf8(json),
                // The attributes provide metadata in a string-to-string dictionary.
                Attributes =
                    {
                        { "description", "Simple text message" }
                    }
            };
            totalSize += message.CalculateSize();
            listMessages.Add(message);

           
            //messages.SetValue(message, count);
            count++;
            publishedMsgCount++;
        }        
         stopWatch.Start();
        var response = publisher.Publish(CurrentTopicName,listMessages);
         stopWatch.Stop();

         TimeSpan ts = stopWatch.Elapsed;
            
 // Format and display the TimeSpan value.
        // String result = String.Format("Publish " + numOfMsgToPublish + " Length in Bytes = " + totalSize + " Messages Time  {0:00}:{1:00}:{2:00}.{3:00}", 
        //             ts.Hours, ts.Minutes, ts.Seconds, 
        //             ts.Milliseconds/10);

         // Console.WriteLine(result);

           Console.WriteLine("Publish Messages TimeMiliseconds = " + stopWatch.ElapsedMilliseconds);
          // Console.WriteLine("Publish Messages Time Tics = " + stopWatch.ElapsedTicks);

    } 
    #endregion
}