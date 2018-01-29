# Bing Maps Fleet Tracker

Bing Maps Fleet Tracker is a fleet management solution based on the Bing Maps APIs. It offers functionalities that enable tracking and managing a fleet of vehicles in real time. For more details, read this [blog post](https://aka.ms/bingmapsfleettrackerblog).

## How to deploy

To use this project, the easiest way is to use the [Bing Maps Fleet Tracker Deployment Portal](https://aka.ms/bingmapsfleettracker). For a step by step walkthrough, see [Out of the box deployment](#out-of-the-box-deployment).The portal will automatically read your Azure Subscriptions and prompt you to enter the required details e.g. your Bing Maps Subscription Key. A deployment would then be made to your Azure Subscription.

None of the data entered on the deployment portal is stored by Bing Maps Fleet Tracker. They are only used to configure your deployment.

## Telemetry collected by Bing Maps Fleet Tracker

After deployment, on your first use of the application, you will be prompted to allow us to collect
anonymous aggregate telemetry and error/warning log data. We use the telemetry data to get a feel for the
usage of this project. Error log data are used to focus our efforts on fixing the issues you face.

Here is a list of all the telemetry items we collect:

* Deployment Id: this is a random GUID that is unique for each deployment.
* Assets count: the total number of vehicles registered.
* Locations count: the total number of locations stored.
* Auto locations count: the number of automatically detected locations.
* Tracking devices count: the total number of tracking devices.
* Geo-fences count: the number of geo-fences setup.
* Tracking points count: the number of tracking points stored on the system.

Error/warning log data collected consist of:

* Deployment Id: this is a random GUID that is unique for each deployment.
* Software version: the version of the software causing the error/warning.
* Error/warning message: the error/warning message.

You can always enable/disable the data collection from the settings tab in your application.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit [https://cla.microsoft.com](https://cla.microsoft.com).

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Out of the box Deployment

The [Bing Maps Fleet Tracker Deployment Portal](https://aka.ms/bingmapsfleettracker) provides a one click deployment experience. You will need to have an active Azure subscription before you can deploy using the portal.

### Creating a New Deployment

1. Go to the [Bing Maps Fleet Tracker Deployment Portal](https://aka.ms/bingmapsfleettracker).
2. Sign in with the account you have your Azure subscription on.
    If you have multiple Azure subscriptions you will be able to specify which one to use while deploying.
3. Enter a name for your deployment in the "Deployment Name" field. Availability of the name will be checked.
4. You are required to enter a valid "Bing Maps Subscription Key" for usage in the map controls and geocoding. You can obtain a bing maps key by following this [article](https://msdn.microsoft.com/en-us/library/ff428642.aspx).
5. You will need to create a Microsoft Application which will be used for authentication in your portal.
    1. Go to [Microsoft Application Portal](https://apps.dev.microsoft.com/?deeplink=/appList)
    2. Sign in with any Microsoft account
    3. Click the "Add an app" button
    4. Enter an application name of your choosing and click create.
    5. Note down the "Application Id" field, as you will need to enter this in the Bing Maps Fleet Tracker Portal.
    6. Click the "Generate New Password" button. Note down the secret specified here, as you will need to enter this in the Bing Maps Fleet Tracker Portal.
    7. Click the "Add Platform" button and choose "Web".
    8. Enter the Redirect Url as "https://{YOUR\_DEPLOYMENT\_NAME}.azurewebsites.net/signin-oidc".
    9. Save the changes and return to the portal. Enter the "Application Id" and "Appliation Secret" that you noted down.
6. You have the option to enter a SendGrid Api Key and Email if you wish to use the geofence email notifications feature. You can sign up for SendGrid keys from the [SendGrid website](https://sendgrid.com/).
7. Click the "Deploy" button and wait until the deployment finishes. You may view the resoures being allocated by clicking on the "View in Azure" link. 
    Once the deployment finishes, you can go to your newly published website at "https://{YOUR\_DEPLOYMENT\_NAME}.azurewebsites.net/".

### Updating an existing Deployment

This project will be actively developed and maintained. You will recieve update notifications on your administration portal informing you of the latest available version. You can also see the version information in your administration portal in the settings tab.

The update process does not reset your data and should be seamless. If your deployment is already up-to-date and working, redeploying will do nothing. If your deployment had previously failed to deploy any resource for any reason, running the update process will deploy any missing resources.

You can also use the deployment process to change any of the secrets you entered while deploying the portal originally, such as the "Bing Maps Subscription Key". You will not be required to re-enter these secrets on update.

You can only use the update functionality with Bing Maps Fleet Tracker deployments that have been created through the portal, if you build and deploy from Visual Studio, you will be unable to use this functionality.

1. Go to the [Bing Maps Fleet Tracker Deployment Portal](https://aka.ms/bingmapsfleettracker).
2. Sign in with the account you have your Azure subscription and deployment on.
    If you have multiple Azure subscriptions you will need to specify the one your deployment exists on.
3. Enter the name of your deployment in the "Deployment Name" field. You should recieve a prompt pointing you to an update link. Click on the update link.
4. Any deployment secrets that you entered previously will be pre-populated with their old values. You may update values here if you wish.
5. Click the "Deploy" button and wait until the deployment finishes. You may view the resoures being allocated by clicking on the "View in Azure" link.
    Once the deployment finishes, you can go to your newly published website at "https://{YOUR\_DEPLOYMENT\_NAME}.azurewebsites.net/".

### Mobile Application

The background tracking mobile application is available for Android as a downloadable APK at [this link](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/releases). You can also build the mobile application by following these [steps](#building-and-running-the-mobile-app).

## Using the Administration Portal

Bing Maps Fleet Tracker offers a wide variety of features that can be used in many ways to achieve many goals. In the this section we will walk through some of the most common scenarios to get you familiar with the capabilities available.

### Asset and Device Provisioning

An asset in Bing Maps Fleet Tracker represents any vehicle being tracked. There are two types of assets: cars and trucks. The differences between the two types is only relevant when using the dispatching features.

A device represents a phone/GPS device that will be used to collect the points for an asset. Devices can be detached from one asset and attached to another, thereby sending their points to the newly attached asset. In most use cases, asset and device should be one to one.

1. Go to the asset tab (Car icon in the nav bar) and create an asset by clicking on the "+" button. Give your asset a name and click submit.
2. Go to the devices tab (Phone icon in the nav bar) and create a device by clicking on the "+" button. Open the mobile application from your phone and scan the QR code to finish provisioning the device. Alternatively, if you will be using your own background tracking client, dismiss the popup and give your device a unique ID and a unique name.
3. In the devices tab, locate your newly created device and click on the edit (pencil icon) button. Scroll down till you find the "Linked Asset" property and select the asset you created. Click submit once you are done.

You have now successfully provisioned a new asset and device.

### Add another User to your Deployment

On a fresh deployment, only the owner can access the portal. If any one else signs in to the portal they will be marked as "pending" until an administrator accepts them.

1. To view all existing and pending users go to the users tab (People icon in the nav bar). You will find all the users with their assigned roles.
2. Click the edit(pencil icon) button to edit a user. You can change a user's role to any of the following:
    * Blocked: User will not be able to access the portal
    * Pending: User will not be able to access the portal until an administrator grants him access
    * Viewer: User will be able to view assets, devices, and reports but will not be able to edit anything
    * Administrator: User will have full access to the portal's features

Click submit once you choose a role to assign to a user.

You can also add a user who has not logged in before by clicking the '+' icon and entering his email.

### Create a geofence

If you wish to be alerted when a specific group of assets enters or exits an area, you can use the geofence feature. A geofence is a polygon encapsulating an area. Triggered geofences have a cooldown (in minutes) which controls how much time must at least pass before it gets triggered again. Inbound geofences trigger when an asset that was outside the geofenced area enters it. Outbound trigger when an asset that was inside the geofenced area leaves the it.

If an asset enters an Inbound geofence and triggers it, it will not trigger it again until it leaves, and re-enters and the cooldown period has passed. Likewise, the triggering behavior of Outbound geofences is identical.

1. Go to the geofences tab (the dotted square icon) and click the '+' icon.
2. Use the map to specify a polygon surrounding the area you want to geofence.
3. Fill in the geofence information including the name, list of emails to notify, cooldown in minutes and the type: inbound or outbound.
4. Select the assets you want to subscribe to this geofence.
5. Click submit to finish creating the geofence.

### View Asset Location History and Trips

When you move to the assets tab, you will see the current location of all assets in real time. You can click on the name of any asset to zoom in and follow that asset around.

You can click on the points (Bullseye icon) button and view the entire location history of that asset. Additionally, you can filter to a specific time period, and restrict the history to it.

You can click on the trip (Rising trend icon) button and view the trips this asset has made. Trips are summarized versions of the location history of an asset. They include information about periods of motion of the asset, the start and ending locations, and any minor stops along the way. You can click on a single trip and view detailed information about it. When in single trip mode, you can click on the map and it will show you information like instantaneous speed, acceleration and other in depth information.

## Using the Rest APIs

An alternative to using the Administration Portal is to use the Restful interface of the backend service directly. Under the hood, these Restul APIs are what the mobile client and the Administration Portal use to fulfill their functionality. You can learn more about these APIs by visting the Swagger instance included by default with every deployment at "https://{YOUR\_DEPLOYMENT\_NAME}.azurewebsites.net/swagger".

### Authentication and Authorization

To be able to use the APIs in different scenarios, you must understand the different Authentication modes available for use.

#### Azure Active Directory (AAD)

This is the primary mode of Authentication and is used by the Administration Portal and Swagger. It involves following the OAuth2 flow using Azure Active Directory as the authentication provider. It is however not convenient to use for some headless scenarios, so we provide alternative authentication methods.

#### Personal Access Tokens

Once signed in using AAD (from either swagger or the portal), a user can generate a PAT and use that to authenticate with the APIs. This PAT has all the permissions the generating user has and can be used to trigger any API the user has access to. The generated PATs are long lived (unless black-listed) and will need to be renewed every 5 years.

You can generate your PAT using the API:

``` Http
POST /api/users/me/token
```

Your PAT is also available in the Administration portal under the Users tab > Edit User.

#### Device Token

Tracking devices that post points to the backend are issued special tokens that give them access to only the post points API. Additionally, they are restricted to posting only to themselves. These are returned when a device is first created, and can be black-listed just like the PATs. They also last up to 5 years.

You can regenerate or view a Device Token using the API:

``` Http
POST /api/devices/{id}/token
```

A Device Token is also available in the Administration portal under the Tracking Devices tab > Edit Tracking Device.

## Build and Run

Bing Maps Fleet Tracker can be broken down into 3 major components: The backend, administration portal and mobile application. Each of these components can be built and run separately, and each can be replaced with implementations of your own.

The backend is responsible for the collection and processing of location information, the provisioning of tracking devices and assets, dispatching, and report generation. It consists of an ASP.NET Core 2 service, and two Azure Functions.

The portal is an angular application that exposes the functionalities of the backend to the deployment administrators. It can be used to view reports, track and provision assets, and compare dispatching routes.

The mobile application is an ionic application responsible for collection of asset location information and the forwarding of this information to the backend. It is meant to provide an out of the box background tracking but it is not the only way to integrate with Bing Maps Fleet Tracker; any GPS device with an internet connection can be used to provide the background tracking (see [Using the Rest APIs](#using-the-rest-apis)).

### Getting the sources

The sources of all 3 components are included in this repository. Simply clone the repository to a folder of your choosing.

``` Bash
git clone https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/
```

### Building and running the backend

The backend is divided into two main components: The Restful service which the administration portal communicates with, and an azure function that performs the trip detection. The Restful service is a .Net Core 2 web application that targets .Net Framework 4.6 for compatibility with Entity Framework 6. Entity Framework 6 is needed for its Geospatial support which Entity Framework Core (7) does not yet support. Unfortunately, this means only Windows machines are able to run this code.

#### Backend Prerequisites

* [Git](https://git-scm.com/)
* [Visual Studio 2017](https://www.visualstudio.com/downloads/) including the capabilities:
  * ASP.NET and Web Development
  * Azure Development
  * .Net Core 2.0

#### Rest Service Build and Run

1. Open solution "Backend/Trackable.sln" using Visual Studio 2017
2. Select Build > Build Solution from the menus. Your solution should build successfully.
3. Navigate to the appsettings.json file under the Trackable.Web project. You will need to update some settings here to get your service up and running.
    * "ConnectionStrings: DefaultConnection": Should point to a Sql Server instance. By default this is set to localdb.
    * "Authorization: ClientId, Client Secret": Should include the Application Id and Application Secret of a [Microsoft application](https://apps.dev.microsoft.com/?deeplink=/appList).
    * "Authorization: OwnerEmail": Email of the owner of the deployment. On a fresh deployment, this is the only email that will be allowed to access the service.
    * "Authorization: SecurityKey": A string of atleast 128 bits that will be used in the signing and validation of the JWT tokens.
    * "SubscriptionKeys: BingMapsKey": A valid Bing Maps Subscription Key for geocoding and frontend map controls. You can obtain a bing maps key by following this [article](https://msdn.microsoft.com/en-us/library/ff428642.aspx).
    * "Serving: ServeFrontend": Specifies whether service should serve the administration portal from wwwroot/dist. If you set this to true, you must build the administration portal with the enivornment "self" and copy the artifacts from frontend/dist/ to backend/Trackable.Web/wwwroot/dist/.
    * "Serving: ServeSwagger": Specifies whether the service should serve swagger.
    * "Serving: IsDebug": IsDebug turns off RequireHttps and turns on some diagnostic logging.
    * (Optional) "SendGrid: ApiKey, FromEmailAddress": Specifies the SendGrid account used for sending Geofence email alerts.You can sign up for SendGrid keys from the [SendGrid website](https://sendgrid.com/).
    * (Optional) "Instrumentation": Urls used to send telemetry Microsoft, see [Telemetry collected by Bing Maps Fleet Tracker](#telemetry-collected-by-bing-maps-fleet-tracker).
    * (Unused) "Versioning": Used by the deployment portal to keep track of version information. Not used when self-publishing.
4. Right click "Trackable.Web" project and choose option "Set as Startup Project".
5. Run project using Visual Studio.

#### Azure Function Build and Run

1. Open solution "Backend/Trackable.sln" using Visual Studio 2017
2. Select Build > Build Solution from the menus. Your solution should build successfully.
3. Navigate to the appsettings.json file under the Trackable.Func project. You will need to update some settings here to get your service up and running.
    * "Values: DatabaseConnection": Should point to a Sql Server instance. By default this is set to localdb.
    * "Values: BingMapsKey": A valid Bing Maps Subscription Key for geocoding and frontend map controls. You can obtain a bing maps key by following this [article](https://msdn.microsoft.com/en-us/library/ff428642.aspx).
    * "Values: AzureWebJobsStorage": Specifies the connection string to an Azure Storage Account which is used internally in the Azure Function.
4. Right click "Trackable.Func" project and choose option "Set as Startup Project".
5. Run project using Visual Studio.

### Building and running the portal

The Administration portal project is an angular 4 project that exposes the functionalities of the backend to the deployment administrators. It can be used to view reports, track and provision assets, and compare dispatching routes.

#### Portal Prerequisites

* [Git](https://git-scm.com/)
* [Node.JS](https://nodejs.org/en/)
* [Yarn](https://yarnpkg.com/lang/en/docs/install/)
* [Angular CLI](https://github.com/angular/angular-cli#installation)

Once you have these tools set up, move to the `Frontend/` folder and run the following command to install the remaining dependencies:

``` Bash
yarn install
```

#### Administration Portal Build and Run

To build the portal use the command:

``` Bash
ng build
```

To run the portal:

1. Navigate to "Frontend\src\environments\environment.ts"
2. Fill the "backendUrl" with the url of any running backend. By default this points to localhost.
3. Fill the "frontendUrl" with the url of the frontend server. By default this points to localhost.
4. Use the following command to run the frontend.

    ``` Bash
    ng serve
    ```

### Building and running the mobile app

This section details how to build and run the mobile application from source, for pre-built artifacts see [Out of the box deployment](#out-of-the-box-deployment).

#### Mobile App Prerequisites

* [Git](https://git-scm.com/)
* [Node.JS](https://nodejs.org/en/)
* [Ionic (with Cordova)](https://ionicframework.com/docs/intro/installation/)

Once you have these tools set up, move to the `MobileClient/` folder and run the following command to install the remaining dependencies:

``` Bash
npm install
```

#### Running as web application

Any ionic application can be run on a browser for testing basic functionality. To ensure correct installation, from the `MobileClient/` folder run:

``` Bash
ionic serve
```

You should be greeted by a registration page in your browser. If this command fails to run, ensure all the prerequisites have been installed correctly. If this step succeeds, you are ready to run on an emulator or device.

#### Android

##### Android Prerequisites

* [JDK 8 or later](http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html) and *JAVA_HOME* set to the right path
* [Android SDK](https://developer.android.com/studio/index.html) and *ANDROID_HOME* set to the right path

Ensure that you have the Android SDK downloaded

To download the native plugins and setup the cordova android project run:

``` Bash
ionic cordova platform add android
```

##### Android Build and run

To build application:

``` Bash
ionic cordova build android
```

To run the application on a device or emulator:

``` Bash
ionic cordova run android
```

#### iOS

This feature is not yet supported. Will be coming soon.