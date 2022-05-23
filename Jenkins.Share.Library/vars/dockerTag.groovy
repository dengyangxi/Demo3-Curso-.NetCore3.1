#!/usr/bin/env groovy

//给镜像打标签
def call(String serviceName, String envName) {
    sh "docker tag ${serviceName}:latest 192.168.10.129/dapr/${serviceName}:${envName}-${BUILD_NUMBER}"
    
}
