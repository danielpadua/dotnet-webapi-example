# How to build and run

In order to run this project, you'll need to have an instance of **postgres** running, and a **postgres super user**.

Once you get that up and running, configure connectionString in `./src/AdventureService/appsettings.Development.json` - if your ASPNETCORE_ENVIRONMENT environment variable is set to "Development". If your ASPNETCORE_ENVIRONMENT is not set, or is set to "Production", then configure appsettings.json.

Replace the bold strings in the connectionString template as following:

`Server=`**localhost**`;User Id=`**user**`;Password=`**password**`;Database=`**database**`;`

Paste it in the `DefaultConnection`, and use `dotnet run` or use `F5` in project in Visual Studio.

After running the project, ef-core should run automatically latest migrations and create the database tables with initial seeded data
