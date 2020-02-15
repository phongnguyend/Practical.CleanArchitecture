#!/bin/bash
while ! nc -z db 1433;
do
    echo db is not ready, sleeping;
    sleep 5;
done;
echo db is ready!;

echo sleep more 30s before starting!;
sleep 30;

cd /ClassifiedAds.Projects && dotnet ClassifiedAds.Migrator.dll
