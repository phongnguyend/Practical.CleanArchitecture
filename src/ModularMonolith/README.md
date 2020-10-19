# Solution Structure
![alt text](/docs/imgs/code-solution-structure-modular-monolith.png)

# Build & Deploy to Kubernetes

- Build
  ```
  docker-compose build
  ```

- Tag
  ```
  docker tag classifiedads.modularmonolith.backgroundserver phongnguyend/classifiedads.modularmonolith.backgroundserver
  docker tag classifiedads.modularmonolith.migrator phongnguyend/classifiedads.modularmonolith.migrator
  docker tag classifiedads.modularmonolith.webapi phongnguyend/classifiedads.modularmonolith.webapi
  docker tag classifiedads.modularmonolith.identityserver phongnguyend/classifiedads.modularmonolith.identityserver
  ```

- Push
  ```
  docker push phongnguyend/classifiedads.modularmonolith.backgroundserver
  docker push phongnguyend/classifiedads.modularmonolith.migrator
  docker push phongnguyend/classifiedads.modularmonolith.webapi
  docker push phongnguyend/classifiedads.modularmonolith.identityserver
  ```

- Apply
  ```
  kubectl apply -f .k8s
  kubectl get all
  kubectl get services
  kubectl get pods
  ```

- Delete
  ```
  kubectl delete -f .k8s
  ```
  
- Use Helm
  ```
  helm install myrelease .helm/modularmonolith
  helm list
  helm upgrade myrelease .helm/modularmonolith
  ```

- UnInstall
  ```
  helm uninstall myrelease
  ```