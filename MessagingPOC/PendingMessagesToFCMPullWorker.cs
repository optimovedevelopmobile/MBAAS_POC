
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
// Imports the Google Cloud client library
using System.Threading.Tasks;
using StackExchange.Redis;


namespace MessagingPOC
{


    public class PendingMessagesToFCMPullWorker
    {
        #region Members

        #region Redis
        protected static ConnectionMultiplexer _redis = null;
        protected static IDatabase _redisDB = null;
        protected static string PendingQueue = "Pending";
        protected static string DefaultRedisServerAddress = "127.0.0.1";
        #endregion

        #region FCM

        protected static String RegistrationId = "dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y";
        protected static String serverKey = "AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q";

        #endregion // Instantiates a client

        #region Process

        Task[] _processingTasks = null;
        protected CancellationToken[] _ctProcessTaskToken;
        protected CancellationTokenSource[] _ctSourceProcessTask;

        protected static ConcurrentQueue<String> _queue = null;
        protected CancellationToken[] _ctPullTaskToken;
        protected CancellationTokenSource[] _ctSourcePullTask;
        protected static Task _pullingTask = null;
        protected static Object _redisOperationLock = null;

        protected static int _numOfThreads = 1;
        protected static int _maxNumOfMsgPerThread = 1;

        protected static Dictionary<int, TenantFCMMetaData> _tenantsFCMData = new Dictionary<int, TenantFCMMetaData>();
        #endregion

        #endregion

        #region PublicMembers
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingMessagesToFCMPullWorker" /> class.
        /// </summary>
        public PendingMessagesToFCMPullWorker()
        {
            _queue = new ConcurrentQueue<String> ();
           
            _redisOperationLock = new Object();

            _tenantsFCMData.Add(1, new TenantFCMMetaData(1, serverKey));

        }


        /// <summary>
        /// Initializes the pull worker.
        /// </summary>
        /// <returns></returns>
        public bool InitializePullWorker(int numOfThreads, int maxNumOfMsgPerThread)
        {

            bool bStatus = false;
            _numOfThreads = numOfThreads;
            _maxNumOfMsgPerThread = maxNumOfMsgPerThread;
            _processingTasks = new Task[_numOfThreads];
            _redis = ConnectionMultiplexer.Connect(DefaultRedisServerAddress);
            _redisDB = _redis.GetDatabase();


            return bStatus;
        }

        /// <summary>
        /// Activates the pulling tasks.
        /// </summary>
        /// <returns></returns>
        public bool ActivatePullingTasks()
        {
             Console.WriteLine("Entering ActivatePullingTasks");
            bool bStatus = false;

            _ctSourceProcessTask = new CancellationTokenSource[_numOfThreads];
            _ctProcessTaskToken = new CancellationToken[_numOfThreads];

            _ctSourcePullTask = new CancellationTokenSource[_numOfThreads];
            _ctPullTaskToken = new CancellationToken[_numOfThreads];

            try
            {

                 
                Task[] tasks = new Task[1];
                 _ctSourcePullTask[0] = new CancellationTokenSource();
                 _ctPullTaskToken[0] =  _ctSourcePullTask[0].Token;
                var currToken =  _ctSourcePullTask[0].Token;
                tasks[0] = Task.Factory.StartNew(() => PullMessagesFromRedisTaskAsync(currToken));

                Task.WaitAll(tasks);                

               
                            
                bStatus = true;
                Console.WriteLine("Exiting ActivatePullingTasks");
            }
            catch (Exception error)
            {
                Console.WriteLine("Failed Pulling From Redis:\n" + error.Message);
                throw;
            }
            return bStatus;
        }


        /// <summary>
        /// Stops the pulling tasks.
        /// </summary>
        /// <returns></returns>
        public bool StopPullingTasks()
        {
            bool bStatus = true;
            foreach(var cancel in _ctSourceProcessTask)
            {
                cancel.Cancel();
                bStatus &= cancel.IsCancellationRequested;
            }
                      
            return bStatus;
        }

        #endregion

        #region Protected

        /// <summary>
        /// Pulls the messages task.
        /// </summary>
        /// <param name="ct">The ct.</param>
        /// 
        protected static async void PullMessagesFromRedisTaskAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var length = _redisDB.ListLength(PendingQueue);
            if (length == 0)
            {
                Thread.Sleep(10);
            }
            else
            {
                var b = await PullMessagedFromPubSubAsync(ct);
            }

        }

        /// <summary>
        /// Pulls the messaged from pub sub.
        /// </summary>
        /// <param name="ct">The ct.</param>
        protected static async Task<bool> PullMessagedFromPubSubAsync(CancellationToken ct)
        {
            bool bStatus = true;
            var fcm = new FCMTester();
            var firebasenet = new FirebaseTestSender();
            Stopwatch stopWatch = new Stopwatch();
          
                var length = _redisDB.ListLength(PendingQueue);
                long numOfMsg = length - 1;

                while (length > 0)
                {

                    RedisValue v = _redisDB.ListRightPop(PendingQueue);
                  
                   firebasenet.SenderDataToFirebaseAsync(v.ToString());
                  
                    length = _redisDB.ListLength(PendingQueue);
                   
                }
            
                Console.WriteLine("Exit PullMessagedFromPubSubAsync");
                return  bStatus;
        }
        


        #endregion

    }
}