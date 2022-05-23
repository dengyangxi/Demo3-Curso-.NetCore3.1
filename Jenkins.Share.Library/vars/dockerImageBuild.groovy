#!/usr/bin/env groovy

//创建镜像
def call(String serviceName, String dockerfilePath) {
    echo "serviceName:${serviceName} dockerfilePath:${dockerfilePath}"
    sh "docker build -t ${serviceName} -f  ${dockerfilePath} ."
}
