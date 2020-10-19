# Solution Structure
![alt text](/docs/imgs/code-solution-structure-microservices.png)

# Build & Deploy to Kubernetes

- Build
  ```
  docker-compose build
  ```

- Tag
  ```
  docker tag classifiedads.gateways.webapi phongnguyend/classifiedads.gateways.webapi
  docker tag classifiedads.services.auditlog.api phongnguyend/classifiedads.services.auditlog.api
  docker tag classifiedads.services.auditlog.grpc phongnguyend/classifiedads.services.auditlog.grpc
  docker tag classifiedads.services.identity.api phongnguyend/classifiedads.services.identity.api
  docker tag classifiedads.services.identity.authserver phongnguyend/classifiedads.services.identity.authserver
  docker tag classifiedads.services.identity.grpc phongnguyend/classifiedads.services.identity.grpc
  docker tag classifiedads.services.notification.api phongnguyend/classifiedads.services.notification.api
  docker tag classifiedads.services.notification.background phongnguyend/classifiedads.services.notification.background
  docker tag classifiedads.services.notification.grpc phongnguyend/classifiedads.services.notification.grpc
  docker tag classifiedads.services.product.api phongnguyend/classifiedads.services.product.api
  docker tag classifiedads.services.storage.api phongnguyend/classifiedads.services.storage.api
  ```

- Push
  ```
  docker push phongnguyend/classifiedads.gateways.webapi
  docker push phongnguyend/classifiedads.services.auditlog.api
  docker push phongnguyend/classifiedads.services.auditlog.grpc
  docker push phongnguyend/classifiedads.services.identity.api
  docker push phongnguyend/classifiedads.services.identity.authserver
  docker push phongnguyend/classifiedads.services.identity.grpc
  docker push phongnguyend/classifiedads.services.notification.api
  docker push phongnguyend/classifiedads.services.notification.background
  docker push phongnguyend/classifiedads.services.notification.grpc
  docker push phongnguyend/classifiedads.services.product.api
  docker push phongnguyend/classifiedads.services.storage.api
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
  helm install myrelease .helm/microservices
  helm list
  helm upgrade myrelease .helm/microservices
  ```

- UnInstall
  ```
  helm uninstall myrelease
  ```