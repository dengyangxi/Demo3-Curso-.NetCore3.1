#!/usr/bin/env groovy
package com.chester

class Utilities implements Serializable {
  def steps
  Utilities(steps) {this.steps = steps}
  def echo() {
    steps.sh "echo a"
  }
}