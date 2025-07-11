﻿# Creating and running migrations:
```shell
dotnet ef migrations add init_db --project Shared --startup-project Api
```

# Setting up dev environment and using `.env` with .NET Aspire  

This solution keeps every “external parameter” (secrets, connection strings, …) in **one file called `.env`**.  
The same file is used when you run the app locally and when you publish with `aspire publish`.

---

## Local development

1. Create a file called `.env` in the app-host project folder (next to Program.cs or the `.csproj`).  
   Example:

   ```
   # .env
   Parameters__username = mydbuser
   Parameters__password = S3cr3tP@ss
   ```

   • `Parameters__username` maps to the parameter you registered with  
     `builder.AddParameter("username", secret: true);`  
   • `Parameters__password` maps to `builder.AddParameter("password", secret: true);`

2. At the very top of *Program.cs* call your helper that loads the file into
   environment variables **before** `DistributedApplication.CreateBuilder`:

   ```csharp
   DotEnv.Load();                           // 1️⃣  reads .env and sets env-vars
   var builder = DistributedApplication.CreateBuilder(args);  // 2️⃣
   ```

3. Run the solution:

   ```
   dotnet run
   ```

   The parameters pick up the values automatically—no JSON, no prompts.

---

## 2. Publishing (`aspire publish` / `dotnet publish`)

1. Tell MSBuild to copy the `.env` file to the publish / Docker-Compose folder
   by adding this to the **app-host** `.csproj`:

   ```xml
   <ItemGroup>
     <None Include=".env" CopyToOutputDirectory="PreserveNewest" />
     <None Include=".env" CopyToPublishDirectory="Always"    />
   </ItemGroup>
   ```

2. Publish:

   ```
   dotnet publish -c Release
   ```

   Publish output:

   ```
   bin/Release/net8.0/publish/compose/
       docker-compose.yml
       .env          ← copied automatically
   ```

   Docker Compose loads `.env` by convention, so every container started by
   Aspire receives the same `Parameters__username`, `Parameters__password`, …

3. (Optional) Skip the “enter a value” prompt that appears while the manifest is
   being generated by exporting the variables during the publish:

   *bash*  
   ```bash
   export $(cat .env | xargs)
   dotnet publish -c Release
   ```

   *PowerShell*  
   ```powershell
   Get-Content .env | ForEach-Object {
       $n,$v = $_ -split '=',2
       Set-Item -Path "env:$n" -Value $v
   }
   dotnet publish -c Release
   ```

That’s all—one `.env` file, zero surprises, same values everywhere.