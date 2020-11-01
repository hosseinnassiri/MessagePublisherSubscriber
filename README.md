# MessagePublisherSubscriber
A publish/subscribe pattern using [MassTransit](https://masstransit-project.com/) library and RabbitMq for messaging.

![Alt text](docs/publish-subscribe.jpg?raw=true "Title")

## Prerequisites
* RabbitMq
* MongoDb
* .Net Core

## How to run the application
### RabbitMq settings
Add RabbitMq settings in appsettings.json of **MessageProcessor** and **MessagePublisher** projects
``` json
...
"MassTransitSettings": {
    "RabbitMqSettings": {
        "Host": "localhost",
        "UserName": "myrabbitAdmin",
        "Password": "password$1",
        "PublishExchangeName": "my-exchange-1",
        "ListenerQueueName": "my-listener-queue-1"
    },
    ...
}
...
```
You can use docker to setup a RabbitMq:
``` powershell
docker run -it --rm --hostname my-local-rabbit --name my-rabbit-container-1 -e RABBITMQ_DEFAULT_USER=myrabbitAdmin -e RABBITMQ_DEFAULT_PASS=password$1 -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```
### MongoDb settings
Configure MongoDb in the appsettings.json in **MessageProcessor** to store the received messages automatically:
``` json
...
"MassTransitSettings": {
    ...
    "MongoDbAuditStore": {
        "Connection": "mongodb://localhost:27017/",
        "DatabaseName": "MessageAuditStore",
        "CollectionName": "messages"
    }
},
...
```
