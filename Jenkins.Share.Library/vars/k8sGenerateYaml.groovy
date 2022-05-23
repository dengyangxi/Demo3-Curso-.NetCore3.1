#!/usr/bin/env groovy

//
def call(String namespaceName, String serviceName, String servicePath, String envName, String servicePort, String nodePort) {
    sh """sed "s/{{tagversion}}/${envName}-${BUILD_NUMBER}/g"  ${servicePath}deployment.yaml.tpl > ${servicePath}deployment.yaml """
    sh """sed -i 's/{{ServiceName}}/${serviceName}/g'  ${servicePath}deployment.yaml"""
    sh """sed -i 's/{{ServicePort}}/${servicePort}/g'  ${servicePath}deployment.yaml"""
    sh """sed -i 's/{{NodePort}}/${nodePort}/g'  ${servicePath}deployment.yaml"""
    sh """sed -i 's/{{NameSpaceName}}/${namespaceName}/g'  ${servicePath}deployment.yaml"""
}
