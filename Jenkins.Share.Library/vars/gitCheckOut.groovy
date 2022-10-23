#!/usr/bin/env groovy


//签出代码
//              分支              git地址 
def call(String branchName, String gitUrl) {
    echo "branchName:${branchName} gitUrl:${gitUrl}"
    checkout([$class: 'GitSCM', branches: [[name: branchName]], extensions: [], userRemoteConfigs: [[credentialsId: 'gitee', url: gitUrl]]])
}
