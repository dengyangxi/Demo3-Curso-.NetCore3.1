#!/usr/bin/env groovy

//部署到k8s
def call(String servicePath, String envName) {
    if (envName == 'dev') {
        sh("kubectl --kubeconfig  ${DEV_MY_KUBECONFIG} apply -f ${servicePath}deployment.yaml")
    }
    if (envName == 'prod') {
        sh("kubectl --kubeconfig  ${PROD_MY_KUBECONFIG} apply -f ${servicePath}deployment.yaml")
    }
}

