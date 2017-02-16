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



var admin = require("firebase-admin");

admin.initializeApp({
  credential: admin.credential.cert("pushnotificationpoc-19baf-firebase-adminsdk-5vd1z-9e5e90b831.json"),
  databaseURL: "https://pushnotificationpoc-19baf.firebaseio.com"
});

console.log("admin.app name = ", admin.app().name);  // "[DEFAULT]"
var adminAuth = admin.auth();
//console.log("Auth: ", adminAuth);
var adminDatabase = admin.database();
//console.log("adminDatabase: ", adminDatabase);

// This registration token comes from the client FCM SDKs.
var registrationToken = "d1Kf-Gl8qcY:APA91bGviQ9es84Nsd3-H52-MbVGB5KvktVdhn0NK9LcQkDAlgbnvneS6Fa3ivn2OaqbENutXnjDuH4slqN95zt855IHihiq8ASSm6m4ji9nVFUBXp_ZYpTEf7xJIYKiccwZvtS-AYax";

// See the "Defining the message payload" section below for details
// on how to define a message payload.
var payload = {
    notification: {
    title: "$GOOG up 1.43% on the day",
    body: "$GOOG gained 11.80 points to close at 835.67, up 1.43% on the day."
  }
};

var options = {
dryRun: true  

};


// Send a message to the device corresponding to the provided
// registration token.
admin.messaging().sendToDevice(registrationToken, payload, options)
  .then(function(response) {
    // See the MessagingDevicesResponse reference documentation for
    // the contents of response.
    console.log("Successfully sent message:", response);
  })
  .catch(function(error) {
    console.log("Error sending message:", error);
  });
  
module.exports = app;
