 
 pipeline {
     environment {
      BEARER_TOKEN = credentials("bearer_token")
      TWISTLOCK_URL = credentials("prisma_serverless_url")
    }
   
    agent any 
    stages {
        stage('Download Twistlock Dependency') { 
            steps {
                 sh '''#!/bin/bash
                  echo "Download Twistlock Defender Binary";
                  curl -sSL -k --header "authorization: Bearer $BEARER_TOKEN" -X POST $TWISTLOCK_URL/api/v1/defenders/serverless/bundle -o twistlock_serverless_defender.zip -d "{\"runtime\":\"dotnetcore3.1\",\"provider\":\"azure\"}";
                  unzip twistlock_serverless_defender.zip;
                '''
            }
        }
        stage('Publish') { 
            steps {
                 sh '''#!/bin/bash
                  echo "Deploy Azure Function"
                  az login --service-principal -u $AZURE_CLIENT_ID -p $AZURE_CLIENT_SECRET -t $AZURE_TENANT_ID
                  az account set -s $AZURE_SUBSCRIPTION_ID
                  zip -r archive.zip ./*'
                  az functionapp deployment source config-zip -g $RESOURCE_GROUP -n $FUNC_NAME --src archive.zip"
                  az logout
                '''
            }
        }
    }
 }
