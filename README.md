 # STHT

A Web application with REST API that implements
- an endpoint that gets the User's Locale 
- an external endpoint API to get Shipping Costs based on the User Locale
- display the option to update the shipping price and calculate the total bidding price
- store the shipping details along with the bidding cost, shipping cost and user locale in the database

## Technology used : 
This application is developed with ASP.NET Core 6.0 Razor Pages, utilizing Minimal API for creating an API that handles user locale.
The frontend is designed with CSHTML (a blend of C# and HTML), Bootstrap, CSS, and jQuery. 
The backend, uses C# Razor pages which follow a Page Template Pattern, which is similar to the MVC (Model-View-Controller) architecture. PostgreSQL serves as the database, integrated  through Entity Framework 6.0, an  Object-Relational Mapping (ORM) tool.

## Requirements to Run the Project
1. Download .NET 6.0 SDK ( https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. Rider IDE (https://www.jetbrains.com/rider/download/#section=mac)
3. Postgres Database 14.0
4. PgAdmin ( GUI for Postgres) or any preferable GUI of your choice

### Dependencies 
- Microsoft.EntityFrameworkCore 6.0.25
- Microsoft.EntityFrameworkCore.Design 6.0.25
- Newtonsoft.JSON 13.0.3
- NqSQL.EntityFrameworkCore.PostGresSQL 6.0.22 (Driver for database connection )

## Steps 
- Open the RIDER IDE
- Click on the get from VCS 
- Clone the Github repository : https://github.com/preethi-prak/STHT.git
- Enter the Directory and Clone the repository 

  ### Restore Dependencies 

  - After the project is loaded, Rightclick on the Project -> Manage Nuget Package 
  - Ensure all the necessary packages are restored. (The .csproj file takes care of automatically managing and installing packages )

  ### Database Settings
  - After Connecting to a Postgres server, Create a new database
  - Navigate to "appsettings.json" file to modify the connection string in the following format
    
    "ConnString": "Server=server_name;Port=port_number;Database=database_name;User Id=user_name;Password=your_password"
  - Test the connection string

  ### Database Migration 
   - In the terminal inside the Rider IDE ,Run the following command to install a tool to manage Entity Framework Commands 
   ```
   dotnet tool install -g dotnet-ef

   ```
  - Navigate to the project folder where the .csproj file resides and run the following command to create the tables, columns and constraints 
  ```
  dotnet ef database update

  ```
  Ensure that the BUILD shows success
  

 ### Running the application 
 
 Now click the RUN Button to run the application. 
 
