﻿
==============
API Versioning
==============
The API Versioning currently implemented uses a "api-version" HTTP header to determine which version of a resource URL to return. 
For more information, see http://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/RoutingConstraintsSample/ReadMe.txt


=============
Authorization
=============
Authorization for individual accounts currently uses what is called "OAuth2 bearer access tokens".   

The procedure is documented @ http://www.asp.net/web-api/overview/security/individual-accounts-in-web-api

The use of the [Authorize] controller or method attribute is required for the feature to work.

A temporary database, containing the current user list, is currently auto-created and stored in the App_Data directory (see App_Data\WebApiAuth.mdf).  
This will be upgraded to a full SQL Server database at some point in the near future.


======================
Publishing API Website
======================
Due to a Visual Studio limitation in Web API Help, to get ACTransit.CusRel.Public.Domain.Contracts classes to propogate to the final published website, 
the individual Publish Profiles publxml files were manually edited to include ACTransit.CusRel.Public.Domain.Contracts.xml, 
which was being stripped away during the Publish process.

If new profiles or to include additional namespaces, with their own XML documentation, please refer to https://aspnetwebstack.codeplex.com/workitem/1720

For debugging any Web Deploy issues from Visual Studio, please refer to: http://www.iis.net/learn/publish/troubleshooting-web-deploy/troubleshooting-web-deploy-problems-with-visual-studio


============================
WebAPI Connection Throttling
============================
Used to prevent multiple Feedback posts from same IP.   This will help prevent SPAM and DoS attacks.

For more information, refer to https://github.com/stefanprodan/WebApiThrottle


=====================
Cross Origin Requests
=====================
Cross Origin Resource Sharing is enabled and set to allow all visitors access to all features.   This is currently defined in the WebApiConfig.cs file.  
For more information, please refer to: http://www.asp.net/web-api/overview/security/enabling-cross-origin-requests-in-web-api


==============
Code Contracts
==============
Code Contracts for .NET, a Visual Studio extension, helps expose the code contracts feature built into .NET 4.5 to its fullest. 
It is currently used for real-time parameter checking but can be extended to use automated white-box testing with Pex.
The installer is located @ http://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970 and include links to more information related to Code Contracts.


=====================
DevExtreme Deployment
=====================
How to deploy an application to the web server (Extra IIS settings for DevExtreme): http://www.devexpress.com/Support/Center/Question/Details/KA18631


=======================
Microsoft Ajax Minifier
=======================
Used on the CusRelsite to minify both JS and CSS.  http://ajaxmin.codeplex.com/


===========================
ASP.NET Web API CacheOutput
===========================

Home @ https://github.com/filipw/AspNetWebApi-OutputCache/

A small library bringing caching options, similar to MVC's "OutputCacheAttribute", to Web API actions.

**CacheOutput** will take care of server side caching and set the appropriate client side (response) headers for you.

You can specify the following properties:
 - *ClientTimeSpan* (corresponds to CacheControl MaxAge HTTP header)
 - *MustRevalidate* (corresponds to MustRevalidate HTTP header - indicates whether the origin server requires revalidation of 
a cache entry on any subsequent use when the cache entry becomes stale)
 - *ExcludeQueryStringFromCacheKey* (do not vary cache by querystring values)
 - *ServerTimeSpan* (time how long the response should be cached on the server side)
 - *AnonymousOnly* (cache enabled only for requests when Thread.CurrentPrincipal is not set)
 
Additionally, the library is setting ETags for you, and keeping them unchanged for the duration of the caching period.
Caching by default can only be applied to GET actions.

Installation
--------------------
You can build from the source here, or you can install the Nuget version:

For Web API 2 (.NET 4.5)
	
	PM> Install-Package Strathweb.CacheOutput.WebApi2

For Web API 1 (.NET 4.0)
	
	PM> Install-Package Strathweb.CacheOutput


Usage
--------------------

	//Cache for 100s on the server, inform the client that response is valid for 100s
		[CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

	//Cache for 100s on the server, inform the client that response is valid for 100s. Cache for anonymous users only.
		[CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, AnonymousOnly = true)]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

	//Inform the client that response is valid for 50s. Force client to revalidate.
		[CacheOutput(ClientTimeSpan = 50, MustRevalidate = true)]
		public string Get(int id)
		{
			return "value";
		}

	//Cache for 50s on the server. Ignore querystring parameters when serving cached content.
		[CacheOutput(ServerTimeSpan = 50, ExcludeQueryStringFromCacheKey = true)]
		public string Get(int id)
		{
			return "value";
		}


Variations
--------------------
*CacheOutputUntil* is used to cache data until a specific moment in time. This applies to both client and server.
	
	//Cache until 01/25/2013 17:00
		[CacheOutputUntil(2013,01,25,17,00)]
		public string Get_until25012013_1700()
		{
			return "test";
		}


*CacheOutputUntilToday* is used to cache data until a specific hour later on the same day. This applies to both client and server.

	//Cache until 23:55:00 today
		[CacheOutputUntilToday(23,55)]
		public string Get_until2355_today()
		{
			return "value";
		}

*CacheOutputUntilThisMonth* is used to cache data until a specific point later this month. This applies to both client and server.

	//Cache until the 31st day of the current month
		[CacheOutputUntilThisMonth(31)]
		public string Get_until31_thismonth()
		{
			return "value";
		}

*CacheOutputUntilThisYear* is used to cache data until a specific point later this year. This applies to both client and server.

	//Cache until the 31st of July this year
		[CacheOutputUntilThisYear(7,31)]
		public string Get_until731_thisyear()
		{
			return "value";
		}

Each of these can obviously be combined with the 5 general properties mentioned in the beginning.

Caching convention
--------------------
In order to determine the expected content type of the response, **CacheOutput** will run Web APIs internal *content negotiation process*, based on the incoming request & the return type of the action on which caching is applied. 

Each individual content type response is cached separately (so out of the box, you can expect the action to be cached as JSON and XML, if you introduce more formatters, those will be cached as well).

**Important**: We use *action name* as part of the key. Therefore it is *necessary* that action names are unique inside the controller - that's the only way we can provide consistency. 

So you either should use unique method names inside a single controller, or (if you really want to keep them the same names when overloading) you need to use *ActionName* attribute to provide uniqeness for caching. Example:

		[CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
		public IEnumerable<Team> Get()
		{
			return Teams;
		}

		[ActionName("GetById")]
		[CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
		public IEnumerable<Team> Get(int id)
		{
			return Teams;
		}


Server side caching
--------------------
By default **CacheOutput** will use *System.Runtime.Caching.MemoryCache* to cache on the server side. However, you are free to swap this with anything else
(static Dictionary, Memcached, Redis, whatever..) as long as you implement the following *IApiOutputCache* interface (part of the distributed assembly).

	public interface IApiOutputCache
	{
		T Get<T>(string key) where T : class;
		object Get(string key);
		void Remove(string key);
		void RemoveStartsWith(string key);
		bool Contains(string key);
		void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null);
	}

Suppose you have a custom implementation:

	public class MyCache : IApiOutputCache {
	  //omitted for brevity
	}

You can register your implementation using a handy *GlobalConfiguration* extension method:

	//instance
	configuration.CacheOutputConfiguration().RegisterCacheOutputProvider(() => new MyCache());

	//singleton
	var cache = new MyCache();
	configuration.CacheOutputConfiguration().RegisterCacheOutputProvider(() => cache);	

If you prefer **CacheOutput** to use resolve the cache implementation directly from your dependency injection provider, that's also possible. Simply register your *IApiOutputCache* implementation in your Web API DI and that's it. Whenever **CacheOutput** does not find an implementation in the *GlobalConiguration*, it will fall back to the DI resolver. Example (using Autofac for Web API):

	cache = new MyCache();
	var builder = new ContainerBuilder();
	builder.RegisterInstance(cache);
	config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());

If no implementation is available in neither *GlobalConfiguration* or *DependencyResolver*, we will default to *System.Runtime.Caching.MemoryCache*.

Each method can be cached multiple times separately - based on the representation (JSON, XML and so on). Therefore, *CacheOutput* will pass *dependsOnKey* value (which happens to be a prefix of all variations of a given cached method) when adding items to cache - this gives us flexibility to easily remove all variations of the cached method when we want to clear the cache. When cache gets invalidated, we will call *RemoveStartsWith* and just pass that key.

The default cache store, *System.Runtime.Caching.MemoryCache* supports dependencies between cache items, so it's enough to just remove the main one, and all sub-dependencies get flushed. However, if you change the defalt implementation, and your underlying store doesn't - it's not a problem. When we invalidate cache (and need to cascade through all dependencies), we call *RemoveStartsWith* - so your custom store will just have to iterate through the entire store in the implementation of this method and remove all items with the prefix passed.

Cache invalidation
--------------------

There are three ways to invalidate cache:

- [AutoInvalidateCacheOutput] - on the controller level (through an attribute)
- [InvalidateCacheOutput("ActionName")] - on the action level (through an attribute)
- Manually - inside the action body

Example:

	[AutoInvalidateCacheOutput]
	public class Teams2Controller : ApiController
	{
		[CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
		public IEnumerable<Team> Get()
		{
			return Teams;
		}

		public void Post(Team value)
		{
			//do stuff
		}
	}

Decorating the controller with [AutoInvalidateCacheOutput] will automatically flush all cached *GET* data from this controller after a successfull *POST*/*PUT*/*DELETE* request.

You can also use the [AutoInvalidateCacheOutput(TryMatchType = true)] variation. This will only invalidate such *GET* requests that return the same *Type* or *IEnumerable of Type* as the action peformed takes as input parameter. 

For example:

	[AutoInvalidateCacheOutput(TryMatchType = true)]
	public class TeamsController : ApiController
	{
		[CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
		public IEnumerable<Team> Get()
		{
			return Teams;
		}

		[CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
		public IEnumerable<Player> GetTeamPlayers(int id)
		{
			//return something
		}

		public void Post(Team value)
		{
			//this will only invalidate Get, not GetTeamPlayers since TryMatchType is enabled
		}
	}

Invalidation on action level is similar - done through attributes. For example:

	public class TeamsController : ApiController
	{
		[CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
		public IEnumerable<Team> Get()
		{
			return Teams;
		}

		[CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
		public IEnumerable<Player> GetTeamPlayers(int id)
		{
			//return something
		}

		[InvalidateCacheOutput("Get")]
		public void Post(Team value)
		{
			//this invalidates Get action cache
		}
	}

Obviously, multiple attributes are supported. You can also invalidate methods from separate controller:

		[InvalidateCacheOutput("Get", typeof(OtherController))] //this will invalidate Get in a different controller
		[InvalidateCacheOutput("Get")] //this will invalidate Get in this controller
		public void Post(Team value)
		{
			//do stuff
		}

Finally, you can also invalidate manually. For example:

		public void Put(int id, Team value)
		{
			//do stuff, update resource etc.

			//now get cache instance
			var cache = Configuration.CacheOutputConfiguration().GetCacheOutputProvider(Request);

			//and invalidate cache for method "Get" of "TeamsController"
			cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey((TeamsController t) => t.Get()));
		}

As you see, you can we use expression try to allow you to point to the method in a strongly typed way (we can't unfortunately do that in the attributes, since C# doesn't support lambdas/expression trees in attributes). 

If your method takes in arguments, you can pass whatever - we only use the expression tree to get the name of the controller and the name of the action - and we invalidate all variations.

You can also point to the method in a traditional way:
			
			cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey("TeamsController","Get"));

Customizing the cache keys
--------------------------

You can provide your own cache key generator. To do this, you need to implement the `ICacheKeyGenerator` interface. The default implementation should suffice in most situations.

When implementing, it is easiest to inherit your custom generator from the `DefaultCacheKeyGenerator` class.

To set your custom implementation as the default, you can do one of these things:

	// Method A: register directly
	Configuration.CacheOutputConfiguration().RegisterDefaultCacheKeyGeneratorProvider(() => new CustomCacheKeyGenerator());
	
	// Method B: register for DI (AutoFac example, the key is to register it as the default ICacheKeyGenerator)
	builder.RegisterInstance(new CustomCacheKeyGenerator()).As<ICacheKeyGenerator>(); // this will be default
	builder.RegisterType<SuperNiceCacheKeyGenerator>(); // this will be available, and constructed using dependency injection

You can set a specific cache key generator for an action, using the `CacheKeyGenerator` property:

	[CacheOutput(CacheKeyGenerator=typeof(SuperNiceCacheKeyGenerator))]

PS! If you need dependency injection in your custom cache key generator, register it with your DI *as itself*.

This works for unregistered generators if they have a parameterless constructor, or with dependency injection if they are registered with your DI.

Finding a matching cache key generator is done in this order:

1. Internal registration using `RegisterCacheKeyGeneratorProvider` or `RegisterDefaultCacheKeyGeneratorProvider`.
2. Dependency injection.
3. Parameterless constructor of unregistered classes.
4. `DefaultCacheKeyGenerator`


JSONP
--------------------
We automatically exclude *callback* parameter from cache key to allow for smooth JSONP support. 

So:

	/api/something?abc=1&callback=jQuery1213

is cached as:

	/api/something?abc=1

Position of the *callback* parameter does not matter.

Etags
--------------------
For client side caching, in addition to *MaxAge*, we will issue Etags. You can use the Etag value to make a request with *If-None-Match* header. If the resource is still valid, server will then response with a 304 status code.

For example:

	GET /api/myresource
	Accept: application/json

	Status Code: 200
	Cache-Control: max-age=100
	Content-Length: 24
	Content-Type: application/json; charset=utf-8
	Date: Fri, 25 Jan 2013 03:37:11 GMT
	ETag: "5c479911-97b9-4b78-ae3e-d09db420d5ba"
	Server: Microsoft-HTTPAPI/2.0

On the next request:

	GET /api/myresource
	Accept: application/json
	If-None-Match: "5c479911-97b9-4b78-ae3e-d09db420d5ba"
	
	Status Code: 304
	Cache-Control: max-age=100
	Content-Length: 0
	Date: Fri, 25 Jan 2013 03:37:13 GMT
	Server: Microsoft-HTTPAPI/2.0

License
--------------------

Licensed under Apache v2. License included.

