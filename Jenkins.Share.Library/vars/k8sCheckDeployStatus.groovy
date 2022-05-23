#!/usr/bin/env groovy

//检查状态
def call(String nameSpaceName, String serviceName, String envName) {
    if (envName == 'dev') {
        sh("""
                    ATTEMPTS=0
                    ROLLOUT_STATUS_CMD='kubectl --kubeconfig  ${DEV_MY_KUBECONFIG} rollout status deployment/${serviceName}  -n ${nameSpaceName}-ns'
                    until \$ROLLOUT_STATUS_CMD || [ \$ATTEMPTS -eq 60 ]; do
                        \$ROLLOUT_STATUS_CMD
                        ATTEMPTS=\$((attempts + 1))
                        sleep 10
                    done
                 """)
    }
    if (envName == 'prod') {
       sh("""
                    ATTEMPTS=0
                    ROLLOUT_STATUS_CMD='kubectl --kubeconfig  ${PROD_MY_KUBECONFIG} rollout status deployment/${serviceName}  -n ${nameSpaceName}-ns'
                    until \$ROLLOUT_STATUS_CMD || [ \$ATTEMPTS -eq 60 ]; do
                        \$ROLLOUT_STATUS_CMD
                        ATTEMPTS=\$((attempts + 1))
                       sleep 10
                    done
                 """)
    }

}

