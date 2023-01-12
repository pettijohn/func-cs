#!/bin/bash


for i in {0..50}
do
    # Clean up - kill the function host & native process
    kill -9 `ps -aux | grep func | grep 7071 | grep -v grep | awk '{print $2'}`
    sleep 1
    #kill -9 `ps -aux | grep nativeaot | grep -v grep | awk '{print $2'}`
    #sleep 1 

    func start --port 7071 &
    #FUNC_PID=$!
    sleep 30
    curl -s -o /dev/null -w "%{time_total}\n" http://localhost:7071/api/InProcExample >> cold.csv
    #sleep 1
    #kill -9 $FUNC_PID
done
