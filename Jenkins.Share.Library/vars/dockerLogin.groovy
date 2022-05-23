#!/usr/bin/env groovy

//登录 docker harbor
def call(String envName) {
    sh 'docker login --username=$HARBOR_USR --password=$HARBOR_PSW  192.168.10.129'
}
