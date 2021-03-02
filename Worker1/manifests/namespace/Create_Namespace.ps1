kubectl create namespace clusterchallenge

kubectl apply -f .\Role-devmanager.yaml -n clusterchallenge
kubectl apply -f .\Role-binding.yaml -n clusterchallenge
