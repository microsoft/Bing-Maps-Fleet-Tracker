# Bing Maps Fleet Tracker - Administration Portal

The Administration portal project is an angular 4 project that exposes the functionalities of the backend to the deployment administrators. It can be used to view reports, track and provision assets, and compare dispatching routes.

## Getting the sources

The sources of the Administration Portal of Bing Maps Fleet Tracker are included in this repository. If you already downloaded the sources skip this step, otherwise clone the repository to a folder of your choosing and move to the `Frontend/` folder.

``` Bash
git clone https://github.com/Microsoft/Bing-Maps-Fleet-Tracker/
cd Bing-Maps-Fleet-Tracker/Frontend
```

## Prerequisites

* [Git](https://git-scm.com/)
* [Node.JS](https://nodejs.org/en/)
* [Yarn](https://yarnpkg.com/lang/en/docs/install/)
* [Angular CLI](https://github.com/angular/angular-cli#installation)

Once you have these tools set up, from the `Frontend/` folder run the following command to install the remaining dependencies:

``` Bash
yarn install
```

## Build and Run

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