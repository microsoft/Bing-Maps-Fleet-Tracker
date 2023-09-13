| :exclamation:  This solution is deprecated and no longer maintained.  |
|-----------------------------------------------------------------------|

# Bing Maps Fleet Tracker

Bing Maps Fleet Tracker is a fleet management solution based on the Bing Maps APIs. It offers functionalities that enable tracking and managing a fleet of vehicles in real time. For more details, read this [blog post](https://aka.ms/bingmapsfleettrackerblog).

## Getting Started

There are two parts to the set-up process:
1. Set up the back-end services and administration portal;
2. Set up the mobile client.

#### Step 1: 
First you need to build and run the back-end services and administration portal from source as detailed [here](#build-and-run).

After deploying the back-end services and administration portal successfully, you can find the walk-through of using the Bing Maps Fleet Tracker solution [here](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/WALKTHROUGH.md).

#### Step 2:
##### Android client:
The easiest way to set up Android client is to download the prebuilt apk from [here](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/releases). You can also build and run the Android client from source as detailed [here](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/MobileClient/README.md#android).

##### iOS client:
You will need to build and run the iOS client from source as detailed [here](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/MobileClient/README.md#ios).

## Build and Run

The Bing Maps Fleet Tracker solution consist of 3 major components:

* [The Backend Services](#backend-services)
* [The Administration Portal](#administration-portal)
* [The Mobile Client](#mobile-client)

Each of these components can be built and run separately, and each can be replaced with implementations of your own.

#### Backend Services

The backend services are responsible for the collection and processing of location information, the provisioning of tracking devices and assets, dispatching, and report generation. They are made up of an ASP.NET Core 2 service, and two Azure Functions. For more information on building an running the backend services see [Backend\README.md](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/Backend/README.md).

#### Administration Portal

The administration portal is an angular application that exposes the functionalities of the backend to the deployment administrators. It can be used to view reports, track and provision assets, and compare dispatching routes. For more information on building an running the administration portal see [Frontend\README.md](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/Frontend/README.md).

#### Mobile Client

The mobile client is an ionic mobile application responsible for collection of asset location information and the forwarding of this information to the backend. It is meant to provide an out of the box background tracking solution but it is not the only way to integrate with Bing Maps Fleet Tracker; any GPS device with an internet connection can be used to provide the background tracking (see [Using the Rest APIs](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/WALKTHROUGH.md#using-the-rest-apis)). For more information on building and running the mobile client see [MobileClient\README.md](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/MobileClient/README.md).

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

You can always enable/disable the data collection from the settings tab in your administration portal.

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

## History

See [release history](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/releases).
