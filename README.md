# Azure Serverless App Embedded Defender
The following describes how to deploy an app-embebbed defender with a C# azure function for runtime defense with Prisma Cloud. This document describes the process for deploying an individual C# Azure function with an app-ebmbedded defender to an existing Azure Function App in Azure Cloud. 

Note: The following settings are required
1. Update Azure Function App Runtime to 64 Bit.
2. Add TWS_Policy environment variable in Application Settings in the Azure function App.
3. Add TWS_DEBUG_ENABLED = TRUE to set verbose logging in the log stream for debugging (Optional)
4. Create a nuget.config file
5. Add dependencies to the project file
6. Modify the C# function to include a reference to the app-embedded defender
7. Configure a runtime policy in the Prisma Cloud console before deploying the defender
8. Deploy the C# Azure Function to an existing Azure Function App with a CI/CD pipeline

## Update Azure Function App runtime to 64-bit
The default runtime for an Azure Function App is 32-bit. The app embedded defender requires 64-bit. The setting can be changed in the Configuration --> General Settings section of the Function App.
<p align="center">
<img src="images/runtime-configuration-64-bit.png" width="85%">
</p>

## Add TWS_POLICY and TWS_DEBUG_ENABLED environment variables
Set the TWS_POLICY environment variable to connect the app-embedded defender with the account and policy associated with the defender.
<p align="center">
<img src="images/set-tws-policy-environment-variable.png" width="85%">
</p>

## Create a Nuget.config file
Create a new file called nuget.config, add the following text to the file. Save and close.
```
<configuration>
  <packageSources>
    <add key="local-packages" value="./twistlock"/>
  </packageSources>
</configuration>
```
## Add dependencies to the Project file
Add the following XML block to your existing prjoect file(*.csproj).
```
  <ItemGroup>
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http" Version="3.0.13" />
    <PackageReference Include="Microsoft.Azure.WebJobs.Extensions" Version="5.0.0-beta.1" />
    <PackageReference Include="Microsoft.NET.Sdk.Functions" Version="4.1.1" />
    <PackageReference Include="Twistlock" Version="22.06.229" />
    <TwistlockFiles Include="twistlock\*" Exclude="twistlock\twistlock.22.06.229.nupkg"/>
    <None Include="@(TwistlockFiles)" CopyToOutputDirectory="Always" LinkBase="twistlock\" />
  </ItemGroup>
```

## Modify C# Azure Function to include app-embedded defender
Inside the azure function source code, insert a reference to the twistlock binary. Only the text "Twistlock.Serverless.Init(log);" is required.
```
namespace Company.Function
{
    public static class HttpTriggerDRW
    {
        [FunctionName("HttpTriggerDRW")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Twistlock.Serverless.Init(log);

```
## Configure Runtime Policy in Prisma Cloud Console
In the prisma cloud conslue, navigate to Defend > Runtime > Serverless Policy > Add Rule

[Configure runtime policy for serverless defense](https://docs.paloaltonetworks.com/prisma/prisma-cloud/prisma-cloud-admin-compute/runtime_defense/runtime_defense_serverless)

Note: In the runtime policy section, under Networking / DNS / Allowed section, include "twistlock.com" and any other FQDNs you don't want to be alerted on (this is only necessary if you enable the IP connectivity feature).

## Deploying from Azure DevOps pipeline
The CI/CD pipeline completes the following tasks:
1. Downloads the twistlock binary from the Prisma Cloud console.
2. Build and Package the Azure function into a Zip file.
3. Deploy azure function zip with app-embedded defender to pre-configured Azure function App.

<p align="center">
<img src="images/azure-function-serverless-ado.png" width="85%">
</p>

### Download the Twistlock defender from the Prisma Cloud Console
```
curl -sSL -k --header "authorization: Bearer __TWS-POLICY-TOKEN__" -X POST __CONSOLE-URL__/api/v1/defenders/serverless/bundle -o twistlock_serverless_defender.zip -d "{\"runtime\":\"dotnetcore3.1\",\"provider\":\"azure\"}";
unzip twistlock_serverless_defender.zip;
```
After completing the steps outlined below you will be able to verify the defender is connected. To view the security audits, go to Monitor > Events > Serverless Audits. You should see audits with the following messages:
DNS resolution of domain name yahoo.com triggered by /usr/bin/wget explicitly denied by a runtime rule.
<p align="center">
<img src="images/azure-function-connectted-app-embedded-defender.png" width="85%">
</p>

Security events related to runtime defense of Azure C# functions with an app-embedded defender will appear in Monitor --> Events --> Serverless Audits.
<p align="center">
<img src="images/azure-function-event-app-embedded-defender.png" width="85%">
</p>
