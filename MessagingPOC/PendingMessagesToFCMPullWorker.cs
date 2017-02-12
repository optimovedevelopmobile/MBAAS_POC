
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
// Imports the Google Cloud client library
using System.Threading.Tasks;
using FirebaseNet.Messaging;
using Newtonsoft.Json;
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

            _ctSourceProcessTask = new CancellationTokenSource[_numOfThreads];
            _ctProcessTaskToken = new CancellationToken[_numOfThreads];

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

           

            _ctSourcePullTask = new CancellationTokenSource[2];
            _ctPullTaskToken = new CancellationToken[2];

            try
            {                               
                
                  for(int ind=0; ind < _numOfThreads; ind++)
                  {
                    _ctSourceProcessTask[ind] = new CancellationTokenSource();
                    _ctProcessTaskToken[ind] =  _ctSourceProcessTask[ind].Token;
                    var currToken =  _ctProcessTaskToken[ind];
                    _processingTasks[ind] = Task.Factory.StartNew( () =>   PullMessagesFromRedisTaskAsync(currToken) );
                  }
                
                Task.WaitAll(_processingTasks);                

               
                            
                bStatus = true;
                Console.WriteLine("Exiting ActivatePullingTasks");
            }
            catch (Exception error)
            {
                Console.WriteLine("Failed ActivatePullingTasks Initiate Tasks:\n" + error.Message);
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
        protected static async Task<bool> PullMessagesFromRedisTaskAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            try{

              //var b = await  PullMessagedFromPubSubAsync(ct);
               var b = await  PullMultiMessagedAsync(ct);
              Console.WriteLine("Exiting PullMessagesFromRedisTaskAsync ");

            }catch(Exception e) {
                    Console.WriteLine("PullMessagesFromRedisTaskAsync Faile" + e.Message);
            }

            return true;
        }
    
                      

        /// <summary>
        /// Pulls the messaged from pub sub.
        /// </summary>
        /// <param name="ct">The ct.</param>
        protected static async Task<bool> PullMessagedFromPubSubAsync(CancellationToken ct)
        {
            bool bStatus = true;
         
            FirebaseTestSender.Init();
            var firebasenet = new FirebaseTestSender();
            Stopwatch stopWatch = new Stopwatch();
          
                long length = 0;
               
               lock(_redisOperationLock){
                     length = _redisDB.ListLength(PendingQueue);
                    }
                long numOfMsg = length - 1;
                RedisValue message = new RedisValue();
                FCMClient client = new FCMClient(serverKey);
                while (length > 0)
                {                                        
                    
                    lock(_redisOperationLock){
                        length = _redisDB.ListLength(PendingQueue);
                        
                        if(length >= 1)
                        {
                            message =  _redisDB.ListRightPop(PendingQueue);                            
                        }
                    }
                    if(length >= 1){                        
                        Dictionary<String, String>  convertedMessage = null;
                        String[] recievedRegistration_ids;
                        bool converted = ConvertMessageWithTokens(message, out convertedMessage, out recievedRegistration_ids);
                      
                        firebasenet.SenderDataPayloadToFirebaseAsync(convertedMessage, recievedRegistration_ids, client);

                    }
                                        
                   lock(_redisOperationLock){
                     length = _redisDB.ListLength(PendingQueue);
                    }                                      
                   
                }

                while(true)
                {
                    bool isFinished = CheckMessagesStatus();  
                    if(isFinished)
                        break;
                        Thread.Sleep(50);
                }
                
                                                           
                return  bStatus;
        }



        /// <summary>
        /// Pulls the messaged from pub sub.
        /// </summary>
        /// <param name="ct">The ct.</param>
        protected static async Task<bool> PullMultiMessagedAsync(CancellationToken ct)
        {
            bool bStatus = true;
         
            FirebaseTestSender.Init();
           FCMClient client = new FCMClient(serverKey);
            Stopwatch stopWatch = new Stopwatch();
          
                long length = 0;
               
               lock(_redisOperationLock){
                     length = _redisDB.ListLength(PendingQueue);
                    }
                long numOfMsg = length - 1;
                RedisValue message = new RedisValue();
                //RedisValue[] msgCollection = new RedisValue[1];
                long start = 0, end = 0;
                long step = length/5;
                        
                   
                var msgCollection1 = _redisDB.ListRange(PendingQueue, 0, 999);                                
                var msgCollection2 = _redisDB.ListRange(PendingQueue, 1000, 1999);                                
                var msgCollection3 = _redisDB.ListRange(PendingQueue, 2000, 2999);                                
                var msgCollection4 = _redisDB.ListRange(PendingQueue, 3000, 3999);                                
                var msgCollection5 = _redisDB.ListRange(PendingQueue, 4000, 5000);                                
                _redisDB.ListTrimAsync(PendingQueue, 0, 0, CommandFlags.HighPriority);    
                _redisDB.ListRightPopAsync(PendingQueue);    
                    
                Parallel.Invoke(() => DoSomeWork(msgCollection1, client), () => DoSomeWork(msgCollection2, client)
                 ,() => DoSomeWork(msgCollection2, client)
                 ,() => DoSomeWork(msgCollection3, client),() => DoSomeWork(msgCollection4, client),() => DoSomeWork(msgCollection5, client));
                  
                                        
                   lock(_redisOperationLock){
                      
                       length = _redisDB.ListLength(PendingQueue); 
                    }                                                                                          
                
                

                while(true)
                {
                    bool isFinished = CheckMessagesStatus();  
                    if(isFinished)
                        break;
                        Thread.Sleep(50);
                }
                
                                                           
                return  bStatus;
        }

        private static void DoSomeWork(RedisValue[] msgCollection, FCMClient client)
        {
            var firebasenet = new FirebaseTestSender();
            foreach(var m in msgCollection){                        
                        Dictionary<String, String>  convertedMessage = null;
                        String[] recievedRegistration_ids;
                        bool converted = ConvertMessageWithTokens(m, out convertedMessage, out recievedRegistration_ids);
                        firebasenet.SenderDataPayloadToFirebaseAsync(convertedMessage, recievedRegistration_ids, client);

                    };
        }

        private static bool CheckMessagesStatus()
        {
            bool isFinished = false;

            if(FirebaseTestSender.MessagesStatus.Count > 0)
            {
                isFinished = ! FirebaseTestSender.MessagesStatus.Values.Contains(false);
               
            }
           return isFinished;
        }

        private static bool ConvertMessageWithTokens(RedisValue message, out Dictionary<String, String> convertedMessage,  out String[] recievedRegistration_ids)
        {
            bool bStatus = true;

            convertedMessage = null;

            var dataPayloadDictionary = JsonConvert.DeserializeObject<Dictionary<String, String> >(message);
            var customerIds = dataPayloadDictionary["customer_ids"];
            char[] delimiterChars = { ','};
            String[] tokens = customerIds.Split(delimiterChars);
            recievedRegistration_ids = new String[tokens.Length];
                        
            int index = 0;
            foreach (var customerIdStr  in tokens)
            {
                var customerId = Convert.ToInt64(customerIdStr);
                String customerToken = String.Empty;
                var found = GetCutomerTokenById(customerId, out customerToken);
                recievedRegistration_ids[index] = customerToken;
                index++;
            } 
          
            dataPayloadDictionary.Remove("customer_ids");          
            convertedMessage = dataPayloadDictionary;
            
        return bStatus;
        }

        private static bool GetCutomerTokenById(long customerId, out string customerToken)
        {
            customerToken = RegistrationId;
            return true;
        }



        #endregion

    }
}