//部署到 k8s
def call(String namespaceName, String serviceName, String envName) {
    dir('harborsecret') {
        checkout([$class: 'GitSCM', branches: [[name: '*/master']], extensions: [], userRemoteConfigs: [[credentialsId: 'gitee', url: 'https://gitee.com/dyx88168/jenkins-demo-secretsjenkins-demo-secrets.git']]])
        sh """sed -i 's/{{ServiceName}}/${serviceName}/g'  secrets.yaml"""
        sh """sed -i 's/{{NameSpaceName}}/${namespaceName}/g'  secrets.yaml"""

        if (envName == 'dev') {
            sh("kubectl --kubeconfig  ${DEV_MY_KUBECONFIG} apply -f secrets.yaml")
        }
        if (envName == 'prod') {
            sh("kubectl --kubeconfig  ${PROD_MY_KUBECONFIG} apply -f secrets.yaml")
        }
    }
}
