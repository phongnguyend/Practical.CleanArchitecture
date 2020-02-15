#!/bin/bash
while ! nc -z migrator 80;
do
    echo migrator is not ready, sleeping;
    sleep 5;
done;
echo migrator is ready!;

while ! nc -z rabbitmq 15672;
do
    echo rabbitmq is not ready, sleeping;
    sleep 5;
done;
echo rabbitmq is ready!;

echo sleep 30s before starting!;
sleep 30;

curl -i -u guest:guest -H "content-type:application/json" -XPUT -d '{"durable":true}' http://rabbitmq:15672/api/queues/%2f/classifiedadds_fileuploaded
curl -i -u guest:guest -H "content-type:application/json" -XPUT -d '{"durable":true}' http://rabbitmq:15672/api/queues/%2f/classifiedadds_filedeleted
curl -i -u guest:guest -H "content-type:application/json" -XPOST -d '{"routing_key": "classifiedadds_fileuploaded", "arguments": {}}' http://rabbitmq:15672/api/bindings/%2f/e/amq.direct/q/classifiedadds_fileuploaded
curl -i -u guest:guest -H "content-type:application/json" -XPOST -d '{"routing_key": "classifiedadds_filedeleted", "arguments": {}}' http://rabbitmq:15672/api/bindings/%2f/e/amq.direct/q/classifiedadds_filedeleted
echo finished setting up rabbitmq;

cd /ClassifiedAds.Projects && dotnet ClassifiedAds.WebMVC.dll
