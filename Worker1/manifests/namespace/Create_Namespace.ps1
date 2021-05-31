kubectl create namespace clusterchallenge --context docker-desktop

kubectl apply -f .\Role-devmanager.yaml -n clusterchallenge --context docker-desktop
kubectl apply -f .\Role-binding.yaml -n clusterchallenge --context docker-desktop
