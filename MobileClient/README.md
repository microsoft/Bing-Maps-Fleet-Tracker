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

You should be greeted by a registration page in your browser. If this command fails to run, ensure all the prerequisites have been installed correctly. If this step succeeds, you are ready to run on an emulator or device.

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

To solve these issues, go to the `plugin/` directory, search for "com.android.support:support-v4:+" and replace it with "com.android.support:support-v4:27.1.0".

Then remove and add the Android platform directory using the commands:

``` Bash
ionic cordova platform remove android
ionic cordova platform add android@6.4.0
```

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