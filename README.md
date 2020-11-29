# Publish / Subscribe messaging
A publish/subscribe pattern using [MassTransit](https://masstransit-project.com/) library and RabbitMq for messaging.

# Introduction
In distributed architecture, different components of the system often need to communicate to other components and send some information about the events that happened on their side. They can notify interested consumer application(s) asynchronously, using  messages.

Asynchronous messaging is an effective way to decouple publishers from consumers, and avoid blocking the publisher to wait for a response.

## Audience
TODO

## Goals
TODO


## Prerequisites
You should have a basic understanding of .Net Core worker service projects and RabbitMQ.

## MassTransit
MassTransit is an open-source message bus framework for .NET which can route messages over MSMQ, RabbitMQ, Azure Service Bus, .... 

It also supports multicast, observers, storing messages for audits, versioning, encryption, sagas, retries, transactions, distributed systems, and other features.

More information about MassTransit can be found [here](https://masstransit-project.com/getting-started/).

## RabbitMQ
RabbitMQ is an open-source message-broker software that originally implemented the Advanced Message Queuing Protocol. It monitors whether the message has been delivered or not, using an acknowledge.

## High-level architecture
![Alt text](docs/publish-subscribe.jpg?raw=true "Title")

## How to run the application
### RabbitMq settings
Add RabbitMq settings in appsettings.json of **MessageProcessor** and **MessagePublisher** projects
``` json
...
"MassTransitSettings": {
    "RabbitMqSettings": {
        "Host": "localhost",
        "UserName": "myRabbitAdmin",
        "Password": "password1",
        "PublishExchangeName": "my-exchange-1",
        "ListenerQueueName": "my-listener-queue-1"
    },
    ...
}
...
```
You can use docker to setup a RabbitMq:
``` powershell
docker run -it --rm --hostname my-local-rabbit --name my-rabbit-container-1 -e RABBITMQ_DEFAULT_USER=myRabbitAdmin -e RABBITMQ_DEFAULT_PASS=password1 -p 5672:5672 -p 15672:15672 rabbitmq:3-management
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
