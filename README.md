# Flowingly API

Short description
A small Web API to parse mail content and extract billing information such as tax, totals and cost centre.

Highlights
- The app is built using .NET 8 Web API (`.NET 8`).
- Global exception handling is implemented using the `ExceptionMiddleware` (registered in `Program.cs`).
- Authorization is enforced via an authorization middleware — all API calls must include an `ApplicationKey` header.
- `Constants.cs` stores error messages and other constant values used across the project.
- Swagger is configured so you can explore and run the API endpoints via the Swagger UI.
- Unit tests are implemented using the xUnit framework.
- The logic layer exposes an interface (for example, `ITextParser`) to keep the code loosely coupled and to facilitate unit testing of implementations such as `TextParserImplementation`.

Configuration
- App configuration values are in `appsettings.json` (and `appsettings.Development.json` for development).
- Two important keys:
  - `ClientAppUrl` — URL of the client application.
  - `TaxRate` — default tax rate used by the parser.

Authentication / Calling the API
- Add the following HTTP header to requests:
  - `ApplicationKey: <your-application-key>`
- Swagger UI will allow you to set headers or call endpoints directly once the app is running.

How to run locally
- Using Visual Studio:
  - __Build Solution__ to compile.
  - Use __Debug > Start Debugging__ (or IIS Express) to run the API.
  - Open __Test Explorer__ to run unit tests.

Swagger
- When running the project, open the Swagger UI (typically `/swagger`) to view and execute the API endpoints.

Unit tests
- Tests are implemented with xUnit. See `Flowingly.API.UnitTest` for test classes such as `TextParserImplementationTests.cs` and `TextParserControllerTests.cs`.
- Tests validate the logic layer through the interface and controller behavior (including error and authorization handling).


Files of interest
- `Program.cs` — app startup, middleware and swagger registration
- `ExceptionMiddleware.cs` — global exception handling
- `Constants.cs` — shared constant values and messages
- `TextParserImplementation.cs` and its interface (e.g., `ITextParser`) — parser logic
- `appsettings.json` / `appsettings.Development.json` — configuration
- `Flowingly.API.UnitTest` — unit tests (xUnit)

Running the Project

- Build the project in Visual Studio 2022
- Run the project.
- To run the api in swagger, click on authorize in swagger and add teh API Key from appsettings.config.

