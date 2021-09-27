# Bing Maps Fleet Tracker - Backend Services

The backend is divided into two main components:

* The Restful service with which the administration portal and mobile client communicate.
* An Azure function project that performs the trip detection.

## Getting the sources

The sources of the Backend Services of Bing Maps Fleet Tracker are included in this repository. If you already downloaded the sources skip this step, otherwise clone the repository to a folder of your choosing and move to the `Backend/` folder.

``` Bash
git clone https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/
cd Bing-Maps-Fleet-Tracker/Backend
```

## Prerequisites

* Windows environment
* [Git](https://git-scm.com/)
* [Visual Studio 2022](https://www.visualstudio.com/downloads/) including the capabilities:
  * ASP.NET and web development (this includes the ASP.NET Core Runtime **6.0**)
  * Azure development
 
## Build and Run

### Rest Service

The Restful service is a .NET 6.0 web application and is compatibility with Entity Framework **6.4**.

1. Open solution `Backend/Trackable.sln` using Visual Studio 2022
2. Select `Build` > `Build Solution` from the menus. Your solution should build successfully.
3. Navigate to the `appsettings.json` file under the `Trackable.Web` project. You will need to update some settings here to get your service up and running.
    * "ConnectionStrings: DefaultConnection": Should point to a Sql Server instance. By default this is set to localdb.
    * "Authorization: ClientId, Client Secret": Should include the Application Id and Application Secret of a [AAD V2 application](https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade). Note:- While adding app for bing map registration, make sure to add redirect url in format "http://localhost:<port_number>/signin-oidc". Where <port_number> is the port your application uses.
    * "Authorization: OwnerEmail": Email of the owner of the deployment. On a fresh deployment, this is the only email that will be allowed to access the service.
    * "Authorization: SecurityKey": A string of at least 128 bits that will be used in the signing and validation of the JWT tokens.
    * "SubscriptionKeys: BingMapsKey": A valid Bing Maps Subscription Key for geocoding and frontend map controls. You can obtain a bing maps key by following this [article](https://msdn.microsoft.com/en-us/library/ff428642.aspx).
    * "Serving: ServeFrontend": Specifies whether the service should serve the administration portal from `wwwroot/dist`. If you set this to true, you must build the administration portal with the environment `self` and copy the artifacts from `Frontend/dist/` to `backend/Trackable.Web/wwwroot/dist/`. Command to build using `self environment` is `ng build --env=self`
    * "Serving: ServeSwagger": Specifies whether the service should serve a swagger endpoint.
    * "Serving: IsDebug": IsDebug turns off RequireHttps and turns on some diagnostic logging.
    * (Optional) "SendGrid: ApiKey, FromEmailAddress": Specifies the SendGrid account used for sending Geofence email alerts. You can sign up for SendGrid keys from the [SendGrid website](https://sendgrid.com/).
    * (Optional) "Instrumentation": Urls used to send telemetry Microsoft, see [Telemetry collected by Bing Maps Fleet Tracker](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/README.md#telemetry-collected-by-bing-maps-fleet-tracker).
    * (Optional) "Versioning": Used by the service to notify when a newer version is available in the deployment portal.
4. Right click `Trackable.Web` project and choose option `Set as Startup Project`.
5. Run project using Visual Studio.

### Azure Functions

1. Open solution `Backend/Trackable.sln` using Visual Studio 2022
2. Select `Build` > `Build Solution` from the menus. Your solution should build successfully.
3. Navigate to the `appsettings.json` file under the `Trackable.Func` project. You will need to update some settings here to get your service up and running.
    * "Values: DatabaseConnection": Should point to a Sql Server instance. By default this is set to localdb.
    * "Values: BingMapsKey": A valid Bing Maps Subscription Key for geocoding and frontend map controls. You can obtain a bing maps key by following this [article](https://msdn.microsoft.com/en-us/library/ff428642.aspx).
    * "Values: AzureWebJobsStorage": Specifies the connection string to an Azure Storage Account which is used internally in the Azure Function.
4. Right click `Trackable.Func` project and choose option `Set as Startup Project`.
5. Run project using Visual Studio.

### Using DotNet CLI

If you wish to build the solution using dotnet CLI, you will need to run the following command from the root directory.

``` Bash
dotnet build Backend/
```

For building inidividual projects, you will need to specify the path to the directory containing the csproj.

``` Bash
dotnet build Backend/src/Trackable.Web/
```

For running the Rest Service you would use

``` Bash
dotnet run --project Backend/src/Trackable.Web/Trackable.Web.csproj
```
