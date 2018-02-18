# Walkthrough of Administration Portal and Rest APIs

Bing Maps Fleet Tracker offers a wide variety of features that can be used in different scenarios. This section walks you through some of the most common scenarios to get you familiar with the capabilities available.

## Asset and Device Provisioning

An asset in Bing Maps Fleet Tracker represents any vehicle being tracked. There are two types of assets: cars and trucks. The differences between the two types is only relevant when using the dispatching features.

A device represents a phone/GPS device that will be used to collect the points for an asset. Devices can be detached from one asset and attached to another, thereby sending their points to the newly attached asset. In most use cases, asset and device will be one to one.

1. Go to the asset tab (Car icon in the nav bar) and create an asset by clicking on the "+" button. Give your asset a name and click submit.
2. Go to the devices tab (Phone icon in the nav bar) and create a device by clicking on the "+" button. Open the mobile application from your phone and scan the QR code to finish provisioning the device. Alternatively, if you will be using your own background tracking client, dismiss the popup and give your device a unique ID and a unique name.
3. In the devices tab, locate your newly created device and click on the edit (pencil icon) button. Scroll down till you find the "Linked Asset" property and select the asset you created. Click submit once you are done.

You have now successfully provisioned a new asset and a device, and linked them together.

## Giving another User access to your Deployment

On a fresh deployment, only the owner can access the portal. If another user tries to sign in to the portal, they will be marked as "pending" and blocked from acessing resources, until an administrator grants them access.

1. To view all existing and pending users go to the users tab (People icon in the nav bar). You will find all the users with their assigned roles.
2. Click the edit (pencil icon) button to edit a user. You can change a user's role to any of the following:
    * Blocked: User will not be able to access the portal
    * Pending: User will not be able to access the portal until an administrator grants him access
    * Viewer: User will be able to view assets, devices, and reports but will not be able to edit anything
    * Administrator: User will have full access to the portal's features

Click submit once you choose a role to assign to a user.

You can also add a user who has not logged in before by clicking the '+' icon and entering his email.

### Creating a geofence

If you wish to be alerted when a specific group of assets enters or exits an area, you can use the geofence feature. You define a geofence as a polygon surrounding an area. Triggered geofences have a cooldown (in minutes) which controls how much time must at least pass before it gets triggered again. Inbound geofences trigger when an asset that was outside the geofenced area enters it. Outbound trigger when an asset that was inside the geofenced area leaves the it.

1. Go to the geofences tab (the dotted square icon) and click the '+' icon.
2. Use the map to specify a polygon surrounding the area you want to geofence.
3. Fill in the geofence information including the name, list of emails to notify, cooldown in minutes and the type: inbound or outbound.
4. Select the assets you want to subscribe to this geofence.
5. Click submit to finish creating the geofence.

### View Asset Location History and Trips

When you move to the assets tab, you will see the current location of all assets in real time. You can click on the name of any asset to zoom in and follow that asset around.

You can click on the points button (Bullseye icon) and view the entire location history of that asset. Additionally, you can filter to show a specific time period only.

You can click on the trip button (Rising trend icon) and view the trips this asset has made. Trips are summarized versions of the location history of an asset. They include information about periods of motion of the asset, the start and ending locations, and any minor stops along the way. You can click on a single trip and view detailed information about it. When in single trip mode, you can click on the map and it will show you information like instantaneous speed, acceleration and other detailed information.

## Using the Rest APIs

If you wish to integrate with Bing Maps Fleet Tracker programmatially, then you can use the Restful APIs provided by the backend service directly. Under the hood, these Restul APIs are what the mobile client and the Administration Portal use. You can learn more about these APIs by visting the Swagger instance included by default with every deployment at "/swagger".

### Authentication and Authorization

To be able to use the APIs in different scenarios, you must understand the different Authentication modes available for use.

#### Azure Active Directory (AAD)

This is the primary mode of Authentication and is used by the Administration Portal and Swagger. It involves following the OAuth2 flow using Azure Active Directory as the authentication provider.

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