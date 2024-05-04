- Check Bicep Version
```
az bicep version
```
- Install Bicep
```
az bicep install
```

- Create Resource Group
```
az group create --name "ClassifiedAds_DEV" \
                --location "southeastasia" \
                --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD" "ResourceType=Mixed"
```

- Deploy Resources
```
az deployment group create --resource-group ClassifiedAds_DEV --template-file main.bicep
```

- Create Lock
```
az lock create --lock-type CanNotDelete \
               --name CanNotDelete \
               --resource-group ClassifiedAds_DEV
```

- Clean Up
```
az lock delete --name CanNotDelete --resource-group ClassifiedAds_DEV
az group delete --name "ClassifiedAds_DEV" --yes
```
