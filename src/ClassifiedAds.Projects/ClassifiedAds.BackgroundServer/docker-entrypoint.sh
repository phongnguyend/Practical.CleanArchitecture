#!/bin/bash
while ! nc -z migrator 80;
do
    echo migrator is not ready, sleeping;
    sleep 5;
done;
echo migrator is ready!;

echo sleep more 30s before starting!;
sleep 30;

cd /ClassifiedAds.Projects && dotnet ClassifiedAds.BackgroundServer.dll
