# Bing Maps Fleet Tracker - Mobile Client

This section details how to build and run the mobile application from source, for pre-built artifacts see [deployment guide](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/DEPLOYMENT.md).

## Prerequisites

* [Git](https://git-scm.com/)
* [Node.JS](https://nodejs.org/en/)
* [Ionic (with Cordova)](https://ionicframework.com/docs/intro/installation/)

Once you have these tools set up, move to the `MobileClient/` folder and run the following command to install the remaining dependencies:

``` Bash
npm install
```

### Running as web application

Any ionic application can be run on a browser for testing basic functionality. To ensure correct installation, from the `MobileClient/` folder run:

``` Bash
ionic serve
```

You should be greeted by a registration page in your browser. If this command fails to run, ensure all the prerequisites have been installed correctly. 

If you click _scan_ in the registration page, you will get the following error:


> Error: Uncaught (in promise): ReferenceError: 'cordova' is not defined ReferenceError: 'cordova' is not defined at Anonymous function (http://localhost:8100/build/main.js:371:13) at t.prototype.invoke (http://localhost:8100/build/polyfills.js:3:9008) at onInvoke (http://localhost:8100/build/vendor.js:30921:21) at t.prototype.invoke (http://localhost:8100/build/polyfills.js:3:9008) at e.prototype.run (http://localhost:8100/build/polyfills.js:3:6455) at Anonymous function (http://localhost:8100/build/polyfills.js:3:4574) at t.prototype.invokeTask (http://localhost:8100/build/polyfills.js:3:9614) at onInvokeTask (http://localhost:8100/build/vendor.js:30912:21) at t.prototype.invokeTask (http://localhost:8100/build/polyfills.js:3:9614) at e.prototype.runTask (http://localhost:8100/build/polyfills.js:3:7057) at s (http://localhost:8100/build/polyfills.js:3:4205) at Anonymous function (http://localhost:8100/build/polyfills.js:3:4612) at t.prototype.invokeTask (http://localhost:8100/build/polyfills.js:3:9614) at onInvokeTask (http://localhost:8100/build/vendor.js:30912:21) at t.prototype.invokeTask (http://localhost:8100/build/polyfills.js:3:9614) at e.prototype.runTask (http://localhost:8100/build/polyfills.js:3:7057) at i (http://localhost:8100/build/polyfills.js:3:3664) at invoke (http://localhost:8100/build/polyfills.js:3:10870) 


If you click _dismiss_ in the registration page, you will get the following error:


> Error: Error in ./RegistrationPage class RegistrationPage - inline template:40:2 caused by: Object doesn't support property or method 'dismiss' at DebugAppView.prototype._rethrowWithContext (http://localhost:8100/build/vendor.js:89149:17) at Anonymous function (http://localhost:8100/build/vendor.js:89162:17) at Anonymous function (http://localhost:8100/build/vendor.js:35817:9) at t.prototype.invokeTask (http://localhost:8100/build/polyfills.js:3:9614) at onInvokeTask (http://localhost:8100/build/vendor.js:30912:21) at t.prototype.invokeTask (http://localhost:8100/build/polyfills.js:3:9614) at e.prototype.runTask (http://localhost:8100/build/polyfills.js:3:7057) at invoke (http://localhost:8100/build/polyfills.js:3:10827)


**_Those errors are expected._**

If this step succeeds, you are ready to run on an emulator or device.

### Android

#### Android Prerequisites

* [JDK 8 or later](http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html) and *JAVA_HOME* set to the right path
* [Android SDK](https://developer.android.com/studio/index.html) and *ANDROID_HOME* set to the right path

Ensure that you have the Android SDK downloaded

To download the native plugins and setup the cordova android project run:

``` Bash
ionic cordova platform add android@6.4.0
```

#### Android Build and run

To build application:

``` Bash
ionic cordova build android
```

To run the application on a device or emulator:

``` Bash
ionic cordova run android
```

#### Android troubleshooting

In some environments, building fresh clones may fail with the errors:

``` Bash
ERROR: In <declare-styleable> FontFamilyFont, unable to find attribute android:fontVariationSettings
ERROR: In <declare-styleable> FontFamilyFont, unable to find attribute android:ttcIndex
```

To solve these issues, go to the `plugin/` directory, search for "com.android.support:support-v4:+" and replace it with "com.android.support:support-v4:27.1.0". You will need to change that in two files: `build.gradle` and `barcodescanner.gradle`.

Then remove and add the Android platform directory using the commands:

``` Bash
ionic cordova platform remove android
ionic cordova platform add android@6.4.0
```

For Windows, if you get errors like "Error occurred during initialization of VM. Could not reserve enough space for 2097152KB object heap", make sure your jvm is in the correct architecture. You can check your _system type_ in System Information APP. For example, you may see _x64-based PC_ for your system type. Then you will need to download _Windows x64 Java SE Development Kit_ from [JDK](https://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html).

### iOS

#### iOS Prerequisites


* XCode 7 or higher
* iOS 9
* A free Apple ID or paid Apple Developer account

To download the native plugins and setup the cordova iOS project run:

``` Bash
ionic cordova platform add ios
```

#### iOS Build and run

To build application:

``` Bash
ionic cordova build ios
```

To run the application on a device or emulator, you will need to set up a provisioning profile, and follow the steps outlined in [the ionic documentation](https://ionicframework.com/docs/intro/deploying/).
