var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');

var index = require('./routes/index');
var users = require('./routes/users');

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(require('stylus').middleware(path.join(__dirname, 'public')));
app.use(express.static(path.join(__dirname, 'public')));

app.use('/', index);
app.use('/users', users);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

var sleep = require('sleep');
var FCM = require('fcm-node');
 var RegistrationId = 'dARiEevCnFo:APA91bFTev5UB_plXxXKmYTrkx79isGzjIeCSy0UST-KNaVQsnGICoF7qgbEYyFu-3n1y807iPNmFI5IbzIlNLpJQ6q-OMqAZmWZeEURmoO3TIlA2TmR9ZSL4Bq4INzHqPmtRsAIxg0Y';
 var serverKey = 'AAAAkwlfmpI:APA91bElre6S3XNPQUzrLjhF5zPgUJFFWHrzblzNxcIpxAgzVEoay_RdS9wTbW-99Gq8KMvd9ecimKgBjJLh_Zjbrv4wQ-Hjl_gFEOYeGNzPUjxWljH7lIwVwyXvn3QCMFEvFF-Jh9_Q';

var fcm = new FCM(serverKey);

  var message = { //this may vary according to the message type (single recipient, multicast, topic, et cetera)
      to: RegistrationId, 
    // collapse_key: 'your_collapse_key',

      notification: {
          title: 'Title of your push notification', 
          body: 'Body of your push notification' 
      },

      data: {  //you can send only notification or only data(or include both)
          my_key: 'my value',
          my_another_key: 'my another value'
      }
  };

 fcm.send(message, function(err, response){
      if (err) {
          console.log("Something has gone wrong!");
      } else {
          console.log("Successfully sent with response: ", response);
      }
  });
 var replied = 0;
 var succeeded = 0;
 var failed = 0;
 var maxCalls = 10000;


// var vigourPerformance = require("vigour-performance").
// run(
//   () => {
   
//     fcm.send(message, function(err, response){
//             if (err) {
//                // console.log("Something has gone wrong!");
//             } else {
//               //  console.log("Successfully sent with response: ", response);
//             }
//             replied++;
//            // console.log("replied=", replied);
//             if(replied >= maxCalls)
//             {
//                console.log("Finished with", replied);
//             }
//         });

//   },
//   (average, iterations) => {
//     // callback 
//     // average : subject function average run time in milliseconds, 
//      console.log("average: ", average);
//      console.log("iterations: ", iterations);
//     // iterations : number of times the subject function was executed 
//     // ) 
//   },
//   5000 // number of times to execute the subject function 
// )
var maxUse =  100;
var currUse = 0;

var perf = require("vigour-performance").time;
var startTime = perf();
 for(count = 0; count < maxCalls; count++){
  
    console.log("count: ", count);
      fcm.send(message, function(err, response){
            if (err) {
              failed++;
                console.log("Something has gone wrong!");
            } else {
              succeeded++;
                console.log("Successfully sent with response: ", response);
            }
           
            replied++;
            console.log("replied=", replied);
            if(replied >= maxCalls)
            {
              var elapsed = perf(startTime);
               console.log("Finished with elapsed = ", elapsed);
               console.log("Finished with replied = ", replied);
               console.log("Finished with succeeded = ", succeeded);
               console.log("Finished with Failed = ", failed);
            }
        });
  }

module.exports = app;
