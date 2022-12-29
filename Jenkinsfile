 
 pipeline {
     environment {
      TWISTLOCK_URL = credentials("prisma_url")
      BEARER_TOKEN = credentials("bearer_token")
    }
   
    agent any 
    stages {
        stage('Download Twistlock Dependency') { 
            steps {
                 sh '''#!/bin/bash
                  echo "Download Twistlock Defender Binary"
                  ls
                  curl -sSL -k --header "authorization: Bearer $BEARER_TOKEN" -X POST $TWISTLOCK_URL/api/v1/defenders/serverless/bundle -o twistlock_serverless_defender.zip -d "{\"runtime\":\"dotnetcore3.1\",\"provider\":\"azure\"}";
                  unzip twistlock_serverless_defender.zip;
                  pwd
                  ls
                  chmod a+x twistcli;
                '''
            }
        }
    }
 }
