# Cluster Challenge
Test bench to run tests against a Kubernetes cluster.

## Test case 1

- Two nodes are deployed (Worker1 and Worker2)
- Worker1 is successfully sending and receiveing messages to/from Worker2
- Worker2 is restarted (2-6 second downtime)
- Worker1 fails to reconnect to restarted Worker2

### Details
- Proto.Cluster.Identity.MongoDb 0.12.0
- Tested in AKS and Kubernetes Docker for Windows

### Log files

- [Successful communication][1] 
- [Successful identitystorage][2] 
- [Failed restart][3] 
- [Failed identitystorage][4] 

[1]: <https://raw.githubusercontent.com/rickevry/ClusterChallenge/main/Testscases/FailedNodeRestart/1_successful_communication.txt?token=AAOR45O3I57FXUZCNZPXATTAI7DCU>
[2]: <https://raw.githubusercontent.com/rickevry/ClusterChallenge/main/Testscases/FailedNodeRestart/2_successful_identitystorage.txt?token=AAOR45LNVI342M76RPQYLATAI7DJM>
[3]: <https://raw.githubusercontent.com/rickevry/ClusterChallenge/main/Testscases/FailedNodeRestart/3_failed_restart.txt?token=AAOR45JDR7AIHVXUBAHYTUTAI7DMW>
[4]: <https://raw.githubusercontent.com/rickevry/ClusterChallenge/main/Testscases/FailedNodeRestart/4_failed_identitystorage.txt?token=AAOR45O47BICDTX6QI4N7Z3AI7DPK>

### Prebuilt containers (DockerHub)
- rickevry/cluster-challenge-worker1:1.0
- rickevry/cluster-challenge-worker2:1.0

## Test case 2

- Start Worker2 in Visual Studio 2019 (instance 1)
- Start Worker1 in Visual Studio 2019 (instance 2)
- Verify that everything is running OK
- Go to taskmanager and end task Worker2
- Wait 1 second
- Start Worker2 again

### Details
- Worker1 and Worker2 runs in VS
- Consul runs in Docker
- Mongo runs in Docker
- Proto.Cluster.Identity.MongoDb 0.12.0


### Log files

- [Failed restart][5] 

[5]: <https://https://raw.githubusercontent.com/rickevry/ClusterChallenge/main/Testscases/LocalFailedNodeRestart/log.txt>

