#!/usr/bin/env groovy

// 执行部署方法入口
//Groovy
//run           'daprtest',          'backend',           './BackEnd/',       '5000',            '31110',      ['dev':'*/master', 'prod':'*/master']
//nameSpaceName ： 命名空间
//serviceName   :  服务名称
//servicePath   :  服务路径
//servicePort   :  服务端口
//nodePort      :  服务内部端口
//envInfo       :  自定义数组
def call(String nameSpaceName, String serviceName, String servicePath, String servicePort, String nodePort, Map envInfo) {
    def devBranch = envInfo['dev']
    def prodBranch = envInfo['prod']

    pipeline {
        agent {
            //
            label 'agentnode'
        }

        environment {
            DEV_MY_KUBECONFIG = credentials('kubeconfig')
            PROD_MY_KUBECONFIG = credentials('kubeconfig')
            HARBOR = credentials('harbor')
        }

        stages {
            stage('Dev - GitPull') {
                steps {
                    deleteDir()
                    gitCheckOut devBranch, env.GIT_URL
                }
                post {
                    success {
                        script {
                            echo 'pull done'
                        }
                    }
                }
            }
            stage('Dev - DockerBuild') {
                steps {
                    dockerImageBuild serviceName, "${servicePath}Dockerfile"
                }
            }
            stage('Dev - DockerTag') {
                steps {
                    dockerTag serviceName, 'dev'
                }
            }
            stage('Dev - DockerLogin') {
                steps {
                    dockerLogin 'dev'
                }
            }
            stage('Dev - DockerPush') {
                steps {
                    dockerPush serviceName, 'dev'
                }
            }
            stage('Dev - GenerateHarborSecretYAML') {
                steps {
                    harborSecret nameSpaceName, serviceName, 'dev'
                }
            }
            stage('Dev - GenerateK8SYAML') {
                steps {
                    k8sGenerateYaml nameSpaceName, serviceName, servicePath, 'dev', servicePort, nodePort
                }
            }
            stage('Dev - DeployToK8S') {
                steps {
                    k8sDeploy servicePath, 'dev'
                }
            }
            stage('Dev - CheckDeployStatus') {
                steps {
                    k8sCheckDeployStatus nameSpaceName, serviceName, 'dev'
                }
            }
            stage('Dev - Jmeter Test') {
                steps {
                    jmeterTest servicePath
                    
                }
            }

            stage('DeployToProd?') {
                steps {
                    input '部署生产？'
                }
            }

            stage('Prod - GitPull') {
                steps {
                    gitCheckOut prodBranch, env.GIT_URL
                }
            }
            stage('Prod - DockerBuild') {
                steps {
                    dockerImageBuild serviceName, "${servicePath}Dockerfile"
                }
            }
            stage('Prod - DockerTag') {
                steps {
                    dockerTag serviceName, 'prod'
                }
            }
            stage('Prod - DockerLogin') {
                steps {
                    dockerLogin 'prod'
                }
            }
            stage('Prod - DockerPush') {
                steps {
                    dockerPush serviceName, 'prod'
                }
            }
            stage('Prod - GenerateHarborSecretYAML') {
                steps {
                    harborSecret nameSpaceName, serviceName, 'prod'
                }
            }
            stage('Prod - GenerateK8SYAML') {
                steps {
                    k8sGenerateYaml nameSpaceName, serviceName, servicePath, 'prod', servicePort, nodePort
                }
            }
            stage('Prod - DeployToK8S') {
                steps {
                    k8sDeploy servicePath, 'prod'
                }
            }
            stage('Prod - CheckDeployStatus') {
                steps {
                    k8sCheckDeployStatus nameSpaceName, serviceName, 'prod'
                }
            }
        }
    }
}
