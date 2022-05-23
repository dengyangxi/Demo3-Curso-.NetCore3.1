#!/usr/bin/env groovy


//jmeter 测试
def call(String servicePath) {
   sh "jmeter -j jmeter.save.saveservice.output_format=xml -n -t ${servicePath}jmeter.jmx -l ${servicePath}jmeter.report.jtl"
   sh "cp ${servicePath}jmeter.report.jtl ${servicePath}jmeter.report.${BUILD_NUMBER}.jtl"
   perfReport errorFailedThreshold:5, sourceDataFiles:"${servicePath}jmeter.report.jtl"
   sh "cat ${servicePath}jmeter.report.${BUILD_NUMBER}.jtl"

   sh """#!/bin/sh
                           grep '<failure>true</failure>' ${servicePath}jmeter.report.${BUILD_NUMBER}.jtl
                           if [ \$? = 0 ]
                           then
                               exit 1
                           else
                               exit 0
                           fi
                           """
}
