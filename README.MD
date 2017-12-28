# AC Transit Customer Relations application (CusRel)

Public transit ticketing system for customer issues and feedback with: inter-departmental routing with notifications, department/person assigments, simple workflow, ticket searching, pre-canned reports, daily reminders and more.
   
## Features of CusRel website:

 * Ticket creation form with: bus transit-related fields (lookups for stop ids, routes, division and direction); reason-for-ticket codes (to simplify ticket routing); customer comments (from call center or injected from public-facing website forms); ticket status/meta-data; contact history discussion; research history discussion; change history (field change audit); linked tickets, and file attachments.

 * Ticket routing queues: user's assigned tickets, user's department tickets and queues specifically for your Customer Relations staff (aka CusRel team): read-to-close tickets and rejected tickets.

 * Ticket workflow: Upon new ticket from any source 
 
 * Dynamic ticket search to find related tickets, with: general date-range/email/name/phone/badge; pick specific ticket fields and associated values (select lists when applicable), save/load presets for fast recall of past searches.
 
 * All on screen lists can be downloaded or printed.

 * Admin screens to manage: user's features (Active Directory is used seperately for authentication), user access (user needs record in CusRel to enable access), department contacts (who receives notifications for department-level ticket routing) and general site configuration (home screen HTML, ticket assignment and high priorty ticket notifications' content, SMTP settings, Lost Property defintions [item category/item type], etc).

 * Pre-canned reports for CusRel team: Lost & Found item tickets, Open/Aging tickets, Ticket status by department, and Closed-ticket "productivity" by department.
 
 * Links to company resources, CusRel documentation and SSRS reports (ad-hoc).

 * ASP.NET MVC 5.2, Entity Framework 6.1.3, Bootstrap 3.3, PowerShell 5.0 and jQuery/jQuery UI and Knockout.

 * log4net and email logging for errors and website tracing.
 
## Features of Public API website:

 * Receive tickets using REST API, with different endpoints and JSON objects for different ticket categories: /incident, /ask (general Q&A), /suggestion, /loadfound

 * API contracts are self-documenting, created at compile-time, viewable on site.
 
 * Receiving website is configurable

 * ASP.NET Web API 5.2 and Entity Framework 6.1.3.

 * log4net and email logging for errors and website tracing.

## Features of CusRel Windows service:

* Submits, accepts and rejects public file attachments using virustotal.com.

* Installed as Windows service using command line options.

* Attachments stored in CusRel database as MIME encoded blobs.

* log4net and email logging for errors and website tracing.

## Minimum Requirements

 * Visual Studio 2017

 * SQL Server 2016 (Express is OK)

## Installation

 * Open the project using Microsoft Visual Studio.

 * Make sure the ACTransit.CusRel.Website project is your startup project.

 * Build and run solution.

By default, the application will run in LocalDB mode -- for demo purposes, this is sufficient.  For production, we suggest at least SQL Server 2016 Express.

When you compile, [PublishScripts/publish.bat](https://github.com/actransitorg/ACTransit.CusRel/PublishScripts/publish.bat) is executed and creates customized databases under the App_Data directory for the web application (see [PublishScripts](https://github.com/actransitorg/ACTransit.CusRel/PublishScripts) for more details).

## Files requiring settings customization

* ACTransit.Framework\ACTransit.Framework\Notification\EmailPayload.cs
* CusRel\ACTransit.CusRel.Website\Web.config
	- Find ADConnectionString and replace "Please provide your Active Directory URL" with your company's LDAP/AD URL (e.g. ldap://ds.example.com/dc=example,dc=com)
* CusRel\ACTransit.CusRel.Website\log4net.config
* CusRel\ACTransit.CusRel.Public.API\Web.config
* CusRel\ACTransit.CusRel.Public.API\log4net.config
* CusRel\ACTransit.CusRel.ServiceHost\App.config
* CusRel\ACTransit.CusRel.ServiceHost\log4net.config

### Configure database users

* Open (localdb)\MSSQLLocalDB.CusRel.dbo.AuthorizedUsers table, add yourself to it (use default user as template).  Note: your NT Login name needs to match the AuthorizedUsers.UserId column.

### Configure log4net

* By default, log4net uses LOG_DIR environment variable for location of log files

## Contact Us

We welcome and encourage your feedback regarding features, found issues, pull requests or any new business processes you have developed.  
If you want to reach out to our team, please visit http://actransit.org or email devsupport@actransit.org.

## License

Code released under the [MIT](https://github.com/actransitorg/ACTransit.CusRel/LICENSE.md) license.  Docs released under [Creative Commons](https://github.com/actransitorg/CusRel/docs/LICENSE_CC.md).