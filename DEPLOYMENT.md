# One Click Deployment

The [Bing Maps Fleet Tracker Deployment Portal](https://aka.ms/bingmapsfleettracker) provides a one click deployment experience. You will need to have an active Azure subscription before you can deploy using the portal.

The deployment portal will prompt you for login, read your Azure Subscription information and then prompt you to enter some required information (e.g. your Bing Maps Subscription Key). It will then deploy the necessary resources to your Azure Subscription.

None of the data entered on the deployment portal is stored by Bing Maps Fleet Tracker. It is only used to configure your deployment.

## Creating a New Deployment

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

### Mobile Client

The background tracking mobile application is available for Android as a downloadable APK at [this link](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/releases). You can also build the mobile application by following these [steps](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/blob/master/MobileClient/README.md).

## Updating an Existing Deployment

This project is actively developed and maintained. You will recieve update notifications on your administration portal informing you of the latest available version. You can also see the version information in your administration portal in the settings tab.

The update process does not reset your data and should be seamless. If your deployment is already up-to-date and working, redeploying will do nothing. If your deployment had previously failed to deploy any resource for any reason, running the update process will deploy any missing resources.

You can also use the deployment process to change any of the secrets you entered while deploying the portal originally (eg. Bing Maps Subscription Key). You will not be required to re-enter these secrets on update.

You can only use the update functionality with Bing Maps Fleet Tracker deployments that have been created through the deployment portal, if you build and deploy from Visual Studio, you will be unable to use this functionality.

1. Go to the [Bing Maps Fleet Tracker Deployment Portal](https://aka.ms/bingmapsfleettracker).
2. Sign in with the account you have your Azure subscription and deployment on.
    If you have multiple Azure subscriptions you will need to specify the one your deployment exists on.
3. Enter the name of your deployment in the "Deployment Name" field. You should recieve a prompt pointing you to an update link. Click on the update link.
4. Any deployment secrets that you entered previously will be pre-populated with their old values. You may update values here if you wish.
5. Click the "Deploy" button and wait until the deployment finishes. You may view the resoures being allocated by clicking on the "View in Azure" link.
    Once the deployment finishes, you can go to your newly published website at "https://{YOUR\_DEPLOYMENT\_NAME}.azurewebsites.net/".

## Troubleshooting a Failed Deployment

Occasionally the deployment script may fail for some reason or another. If that happens, you can view the detailed error in the Azure portal by following the "Open in Azure link".

In most cases, rerunning the deployment using the [update functionality](#updating an existing deployment) will most likely fix the issue.

If the issue persists, please open a [Gitub issue](https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/issues) and include the error message from the azure portal.

## Upgrading your azure resources to handle more assets

The deployment portal deploys the compute resources (Web App) and data resources (SQL Server) at the smallest possible tier to save you hosting cost.

The default deployed configuration (B1 Web App and S1 Sql Server) should be able to support approximately 150 tracked assets. The exact number of assets a deployment support depends on your use case.

If you want to track more than 150 assets, then you should upgrade your web app by Scaling Up (increasing the machine CPU and RAM resources) and Scaling Out (increasing the number of machines). Additionally, you will need to upgrade the Sql Server database by increasing the DTUs. This [article](https://docs.microsoft.com/en-us/azure/app-service/web-sites-scale) shows you how to scale up azure resources.