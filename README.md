# Azure Serverless App Embedded Defender
The following describes how to deploy an app-embebbed defender with a C# azure function for runtime defense with Prisma Cloud. This document describeds the process for deploying an individual C# Azure function with an app-ebmbedded defender to an existing Azure Function App in Azure Cloud. 

Note: The following settings are required
1. Update Azure Function App Runtime to 64 Bit.
2. Add TWS_Policy environment variable in Application Settings in the Azure function App.
3. Add TWS_DEBUG_ENABLED = True to set verbose logging in the log stream for debugging (Optional)

## Deploying from Azure DevOps pipeline
The CI/CD pipeline completes the following tasks:
1. Downloads the twistlock binary from the Prisma Cloud console.
2. Build and Package the Azure function into a Zip file.
3. Deploy azure function zip with app-embedded defender to pre-configured Azure function App.

### Download the Twistlock defender from the Prisma Cloud Console
```
curl -sSL -k --header "authorization: Bearer __TWS-POLICY-TOKEN__" -X POST __CONSOLE-URL__/api/v1/defenders/serverless/bundle -o twistlock_serverless_defender.zip -d "{\"runtime\":\"dotnetcore3.1\",\"provider\":\"azure\"}";
unzip twistlock_serverless_defender.zip;
```
