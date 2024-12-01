# AutoParts_ShopAndForum

The main function of the platform is to provide an application that can be used for buying parts for cars. Every part is in a specific category so that users can browse easily. There is a forum section in which every authenticated user can ask and asnwer questions. Every forum post is in a specific category as well. Every user can create an account in the system or login via gmail.

## Used technologies

The web application is built with ASP.NET using the MVC pattern. The selected database for storing the data is Microsoft SQL Server. ASP.NET Core Identity framework is used for the whole authentication process - handling individual users' accounts as well as the external ones from gmail. Code First approach is used for generating the database. Entity Framework Core is the used ORM. For the fronted are used Razor-based views.

#Installation and Setup

To start the project locally:
1. Clone the repository and open the solution ``AutoParts_ShopAndForum.sln``
2. Configure database connection: 
The application uses Microsoft SQL Server for storing the data. The connection string of the application should be updated. This can be done from the ``appsettings.json`` file in the web project.
3. Create database:
	```
		1. Open Package Manager Console from Visual Studio
		2. Select **AutoParts_ShopAndForum.Infrastructure** as the Target/Default Project
		3. Run the following command: **update-database**
	```
