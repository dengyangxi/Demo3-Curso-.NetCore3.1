#!/usr/bin/env groovy


//ǩ������
//              ��֧              git��ַ 
def call(String branchName, String gitUrl) {
    echo "branchName:${branchName} gitUrl:${gitUrl}"
    checkout([$class: 'GitSCM', branches: [[name: branchName]], extensions: [], userRemoteConfigs: [[credentialsId: 'gitee', url: gitUrl]]])
}
