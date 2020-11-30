# Publish / Subscribe messaging
A publish/subscribe pattern using [MassTransit](https://masstransit-project.com/) library and RabbitMq for messaging.

## Introduction
In distributed architecture, different components of the system often need to communicate to other components and send some information about the events that happened on their side (An event is something that happened in a component, a change or an action that has taken place). They can notify interested consumer application(s) asynchronously, using  messages.

The Publish-Subscribe pattern builds on the Observer pattern by decoupling subjects from observers via asynchronous messaging.

Asynchronous messaging is an effective way to decouple publishers from consumers, and avoid blocking the publisher to wait for a response. Publishers and consumers can be maintained independently. Therefore, it increases scalability and improves responsiveness of the publisher. The publisher can publish a message, and then return to its core processing responsibilities. The messaging infrastructure is responsible for ensuring messages are delivered to interested subscribers.

Asynchronous messaging helps applications to handle failures more effectively, and it can also be used for scheduled or background processing. It also helps a smoother and simpler integration between systems / components using different platforms, programming languages, or communication protocols, as well as between on-premises systems and applications running in the cloud.

## When to use publish / subscribe pattern
* A component needs to notfiy multiple consumers of an event that happened on its side, and doesn't need to wait for a reponse from consumers real-time.
* A component needs to communicate with other components independently.
* Having eventual consitency is not an issue for the application.

## Goals
To have a template for implementing Publish / Subscribe pattern in .net using RabbitMQ as message broker, ready to be used in real applications.

## TODO
1. convert to .net 5
1. use mongodb in docker
1. use docker compose for the whole project

## Prerequisites
You should have a basic understanding of .Net Core worker service projects and RabbitMQ.

## MassTransit
MassTransit is an open-source message bus framework for .NET which can route messages over MSMQ, RabbitMQ, Azure Service Bus, .... 

It also supports multicast, observers, storing messages for audits, versioning, encryption, sagas, retries, transactions, distributed systems, and other features.

More information about MassTransit can be found [here](https://masstransit-project.com/getting-started/).

## RabbitMQ
RabbitMQ is an open-source message-broker software that originally implemented the Advanced Message Queuing Protocol. It monitors whether the message has been delivered or not, using an acknowledge. By using a message-broker like RabbitMQ, each component can focus on its core responsibilities, while the messaging infrastructure handles everything required to reliably route messages to multiple consumers.

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
