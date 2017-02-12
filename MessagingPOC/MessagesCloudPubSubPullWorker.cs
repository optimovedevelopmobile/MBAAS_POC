
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Google.Cloud.PubSub.V1;
using StackExchange.Redis;
using Grpc.Core;
using System.Threading;
using System.Diagnostics;

namespace MessagingPOC
{



    public class MessagesCloudPubSubPullWorker
    {


        #region Members

        #region Redis

        static ConnectionMultiplexer _redis = null;
        static IDatabase _redisDB = null;
        protected static string PendingQueue = "Pending";
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

        #region Processings
        protected CancellationToken _ctProcessTask;
        protected CancellationTokenSource _ctSourceProcessTask;
        protected static Task _pullingTask = null; 
        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesCloudPubSubPullWorker"/> class.
        /// </summary>
        public MessagesCloudPubSubPullWorker()
        {
           
        }

        /// <summary>
        /// Initializes the pull worker.
        /// </summary>
        /// <returns></returns>
        public bool InitializePullWorker()
        {

            bool bStatus = false;
         
            _redis = ConnectionMultiplexer.Connect(DefaultRedisServerAddress);
            _redisDB = _redis.GetDatabase();

            CurrentTopicName = new TopicName(DefaultProjectId, DefaultTopicName);
            _subscriber = SubscriberClient.Create();
            //String  subscriptionId = "projects/pushnotificationpoc-19baf/subscriptions/myTestReader";

            _publisher = PublisherClient.Create();
            _subscriptionName = new SubscriptionName(DefaultProjectId, DefaultSubscriptionId);
            String t = _subscriptionName.ToString();
            try
            {
                _subscriber.CreateSubscription(_subscriptionName, CurrentTopicName, pushConfig: null, ackDeadlineSeconds: 60);
            }
            catch (RpcException e)
            {
                if (e.Status.StatusCode == StatusCode.AlreadyExists)
                {
                    // Already exists.  That's fine.
                    _bSubscriptionAllreadyExists = true;

                }
            }

            return bStatus;
        }


        /// <summary>
        /// Starts the pulling from cloud pub sub.
        /// </summary>
        /// <returns></returns>
        public bool StartPullingFromCloudPubSub(int maxMsgPull)
        {

            bool bStatus = false;
            int total = 0;
            while(true)
            {

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    PullResponse response = _subscriber.Pull(_subscriptionName, returnImmediately: true, maxMessages: maxMsgPull);
                    
                    if(response != null)
                    {
                        var currEnum = response.ReceivedMessages.GetEnumerator();
                        var count = response.ReceivedMessages.Count;    
                        total += count;      
                        if(count > 0){
                         PushMessagesInToRedisQueue(response);

                         AcknowledgePubSuMsgs(currEnum, count);
                        } else 
                                break;
                       
            
                        TimeSpan ts = stopWatch.Elapsed;
                        
            // Format and display the TimeSpan value.
                     
                    
                        Console.WriteLine("Pull and Ack count=" + count + " Messages TimeMiliseconds = " + stopWatch.ElapsedMilliseconds);
                    

                    }else
                        break;
                  
            }
           Console.WriteLine("Pull and Ack Total = " + total);
                    
            return bStatus;
        }

        /// <summary>
        /// Pushes the messages in to redis queue.
        /// </summary>
        /// <param name="response">The response.</param>
        protected static void PushMessagesInToRedisQueue(PullResponse response)
        {
            foreach (Google.Cloud.PubSub.V1.ReceivedMessage received in response.ReceivedMessages)
            {
                PubsubMessage msg = received.Message;
           //     Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
           //    Console.WriteLine($"Text: '{msg.Data.ToStringUtf8()}'");
                string jsonMsg = msg.Data.ToStringUtf8();
                                      
                _redisDB.ListLeftPush(PendingQueue, jsonMsg);

                var size = _redisDB.ListLength("Pending");
                Console.WriteLine("redis length size = " + size );
            }
        }

        /// <summary>
        /// Acknowledges the pub su MSGS.
        /// </summary>
        /// <param name="currEnum">The curr enum.</param>
        /// <param name="count">The count.</param>
        protected static void AcknowledgePubSuMsgs(IEnumerator<ReceivedMessage> currEnum, int count)
        {
            String[] acksArray = new String[count];
            int indx = 0;
            while (currEnum.MoveNext())
            {
                var msg = currEnum.Current as Google.Cloud.PubSub.V1.ReceivedMessage;
                acksArray[indx] = msg.AckId;
                indx++;
            }
            _subscriber.Acknowledge(_subscriptionName, acksArray);
        }

    }
}
