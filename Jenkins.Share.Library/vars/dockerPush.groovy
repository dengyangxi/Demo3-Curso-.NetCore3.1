#!/usr/bin/env groovy

//推送镜像到 harbor
def call(String serviceName,String envName) {
    sh "docker push 192.168.10.129/dapr/${serviceName}:${envName}-${BUILD_NUMBER}"
}
