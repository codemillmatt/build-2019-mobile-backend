# Choosing a Backend Architecture For Your Mobile App

Choosing the backend architecture for your mobile app can be daunting. Do you go with a traditional WebAPI? Serverless? Maybe a mobile backend as a service (MBaaS) that packages several convenient components for mobile developers together? In this session you will learn about the pros and cons of each, the context of when each makes sense to use, and you’ll see a real-world app built using each of these backends. When you leave this session, you’ll be equipped with the knowledge of how to pick and implement the right backend for your mobile application.

## Get Some Azure

You're gonna need some Azure to get this repo up and running - so why not sign up for a [free account](https://azure.microsoft.com/free/?WT.mc_id=build2019-github-masoucou)? 12 months of free services, plus $200 in free credit. More than enough to get you rolling.

## Deploying The Demos

First, clone this repo. Download it as a zip. Just get it locally. Then...

Next there is a deploy folder. It contains an [ARM template](https://docs.microsoft.com/azure/azure-resource-manager/resource-group-authoring-templates?WT.mc_id=build2019-github-masoucou) and a Bash script with [Azure CLI commands](https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest) that will deploy everything and setup the environment variables in Azure for you.

## Running the Demos

The demo is of a shopping cart application. Within a Xamarin application, it allows the user to browse products, see the product's inventory, and purchase them.

It can be broken down into 3 main parts.

1. The mobile application
1. A suite of microservices served as APIs
1. A suite of microservices served as serverless Azure Functions

The microservices are then broken down into 3 parts as well.

1. Products metadata - Node.js (or C# Azure Function) with MongoDB
1. Inventory - ASP.NET (or C# Azure Function) with SQL Server
1. Shopping cart - ASP.NET (or C# Azure Function) with MongoDB

During the talk, you learned about the trade-offs between building a mobile backend in a "pure web api" format using [containers](https://docs.microsoft.com/azure/app-service/containers/quickstart-dotnetcore?WT.mc_id=build2019-github-masoucou) and [Azure App Services](https://docs.microsoft.com/azure/app-service/app-service-web-get-started-dotnet?WT.mc_id=build2019-github-masoucou). Using an event driven [Azure Functions](https://docs.microsoft.com/azure/azure-functions/functions-overview?WT.mc_id=build2019-github-masoucou) approach. Or going with a Mobile Backend as a Service (MBaaS) such as [Visual Studio App Center](https://docs.microsoft.com/appcenter/?WT.mc_id=build2019-github-masoucou).

Everything will already be running in Azure if you ran the setup script. If you did not, you can run everything locally. Check out this video on how to get it setup.

> The video will be coming soon!!!

### Miscellaneous Services

There were several miscellaneous services demonstrated that applied to any mobile backend. These are _not_ included in the setup scripts. But they aren't that difficult to setup and will be included in the video below.

The first service is [Azure Front Door](https://docs.microsoft.com/azure/frontdoor/front-door-overview?WT.mc_id=build2019-github-masoucou). Front Door allows you to manage and define global routing of your web application traffic. This means you can route requests to their nearest backend.

The second was [Azure Blob Storage](https://docs.microsoft.com/azure/storage/blobs/storage-blobs-overview?WT.mc_id=build2019-github-masoucou) and [Azure CDN](https://docs.microsoft.com/azure/cdn/cdn-overview?WT.mc_id=build2019-github-masoucou). Blog storage is a cost efficient was to store static, binary data. While a CDN will cache it and bring it to the edge.

## Developing Backends For Mobile Apps Talk Summary

Mobile development is hard. Mix in backend development - and then you really have a mess on your hands.

### Mobile backends have several, _unique_, needs that you need to account for

* API versioning (_because not everybody is going to update the mobile app at the same time you roll out a new backend_).
* Offline data and conflict resolution (_because nobody stays within wifi or cell reach, and of course they're going to modify data when offline. And of course that data will somehow be out of sync with then server when they go back online_).
* Push notifications (_because hey, you gotta get your users' attention somehow)_!
* Tough debugging _(seriously, between all the different devices, iOS simulators, Android emulators, fast connectivity, slow connectivity, intermittent connectivity, no connectivity - not to mention just creating the app - debugging is no fun)_.

### Plus mobile backend have table stakes

By table stakes, I mean that these features need to be present in order for a backend to support mobile apps.

* Scalable _(let's face it, the whole reason we write apps is for people to use them, so we want to be able to scale them up)_.
* Authentication _(we need to know who are users are in order to provide any sort of user-based security or personalization)_.
* Data and file storage _(goes hand-in-hand with the data from the unique needs section - gotta store that data!)_.
* Push notifications _(again from the unique needs section, in order for a backend to be mobile, you somehow need to be able to send push notifications from it)_.

### Other Considerations

Then of course there are other things that you need to keep in mind when you're developing apps.

* Cost _(you don't want to blow your life savings just to run a backend for your app)!_
* Productivity _(let your development team create the backend in the language they are most proficient in)_.
* There will be more clients _(like it or not, mobile doesn't exist in isolation, and eventually more clients will use the backend - web, IoT, who knows what else)_!