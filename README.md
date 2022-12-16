# Publish / Subscribe messaging
A publish / subscribe pattern with .Net 5.0 Worker and using [MassTransit](https://masstransit-project.com/) library and RabbitMq for messaging.

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

## Prerequisites
You should have a basic understanding of .Net 5.0 worker service projects and RabbitMQ.
TODO:

## MassTransit
MassTransit is an open-source message bus framework for .NET which can route messages over MSMQ, RabbitMQ, Azure Service Bus, .... 

It also supports multicast, observers, storing messages for audits, versioning, encryption, sagas, retries, transactions, distributed systems, and other features.

More information about MassTransit can be found [here](https://masstransit-project.com/getting-started/).

## RabbitMQ
RabbitMQ is an open-source message-broker software that originally implemented the Advanced Message Queuing Protocol. It monitors whether the message has been delivered or not, using an acknowledge. By using a message-broker like RabbitMQ, each component can focus on its core responsibilities, while the messaging infrastructure handles everything required to reliably route messages to multiple consumers.

## High-level architecture
![Alt text](docs/publish-subscribe.jpg?raw=true "Title")


## How to run the application without docker-compose
### Configure [docker-compose](docker-compose.yml)
* Network

Define a shared network between all services:
``` yaml
networks:
  my-network-name:
```

* Volumes

You need to create a volume so you can persist the MongoDB data even when the container is stopped:
``` yaml
volumes:
  mongodb_data:
    name: mongodata
```

* RabbitMQ
``` yaml
rabbitmq:
    container_name: rabbitmq
    hostname: rabbitmq
    ports:
      - 5672:5672
      - 15672:15672
    environment:
      - RABBITMQ_DEFAULT_USER=rabbit
      - RABBITMQ_DEFAULT_PASS=rabbit
    image: rabbitmq:3-management
    networks:
      - my-network-name
```

* MongoDB

Note that, I exposed mongodb process to port 27018 as I already have a mongodb installed on my host using the default 27017 port.

When you're using containers in windows, you need to define the path for volume like this, otherwise it's throwing error:
``` yaml
mongo:
    container_name: mongo
    image: mongo:windowsservercore
    restart: always
    volumes:
      - mongodb_data:c:/data/db
    ports:
      - 27018:27017
    networks:
      - my-network-name
```

* Subscriber

Configure the subscriber (MessageProcessor worker service project):
``` yaml
messageprocessor:
    image: ${DOCKER_REGISTRY-}messageprocessor
    build:
      context: .
      dockerfile: src\MessageProcessor\Dockerfile
    restart: on-failure
    depends_on:
      - mongo
      - rabbitmq
    networks:
      - my-network-name
```

* Publisher
Now confiure the publisher web api project:
``` yaml
  messagepublisherapi:
    image: ${DOCKER_REGISTRY-}messagepublisherapi
    build:
      context: .
      dockerfile: src\MessagePublisherApi\Dockerfile
    depends_on:
      - rabbitmq
    networks:
      - my-network-name
```

Run the following docker-compose command from your command line or just simply run the whole application from Visual Studio 2022.
``` powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

Then you can send messages using MessagePublisherApi web api application. You can use the swagger or the following curl command:
``` powershell
curl -X POST "https://localhost:63769/Publisher/publish" -H  "accept: */*" -H  "Content-Type: application/json" -d "{\"data\":\"this is your message data!\"}"
```
Note that, the port might be different.

## How to run the application without docker-compose
### Setup RabbitMQ on container on Windows
``` powershell
docker run -it --rm --hostname rabbitmq --name my-rabbitmq -e RABBITMQ_DEFAULT_USER=rabbit -e RABBITMQ_DEFAULT_PASS=rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Add RabbitMq settings in appsettings.json of **MessageProcessor** and **MessagePublisher** projects:
Note that, you need to change the hostname to *localhost* when running your application from container host.
``` json
...
"MassTransitSettings": {
    "RabbitMqSettings": {
        "Host": "localhost",
        "UserName": "rabbit",
        "Password": "rabbit",
        "PublishExchangeName": "my-exchange-1",
        "ListenerQueueName": "my-listener-queue-1"
    },
    ...
}
...
```

### Setup MongoDB on container on Windows
First you need to create a volume so you can persist the data even when the container is stopped:
``` powershell
docker volume create --name=mongodata
```
Then, the next step will pull the database image if it doesn’t already exist, and will launch a running instance using the mounted volume that we created in previous step:
``` powershell
docker run -it --rm --name mongodb -v mongodata:/data/db -p 27017:27017 mongo:windowsservercore
```
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

## TODO
1. add git hub action for CI / CD
1. add healthcheck for subscriber