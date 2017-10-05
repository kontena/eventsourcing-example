# Event Sourcing Microservices Example

This repository is an example of a microservices style application using the event sourcing pattern, with Kafka as both the event publishing mechanism and the events source of truth.  It contains a mixture of C# (.Net Core 2.0), Ruby and TypeScript.

Kafka and Zookeeper are installed as Kontena Stacks, and are based on fantastic work done by the Harvard Center for Geographic Analysis (CGA) for their [hhypermap-bop project](https://github.com/cga-harvard/hhypermap-bop).

## Setup

First things first, you will need a running Kontena Platform to install everything on to.  Please take a look at our [quick-start guide](https://kontena.io/docs/quick-start.html).

If you don't already have one, the easiest way to get started is to head over to https://cloud.kontena.io and set up a new trial account.  Follow the instructions to create a brand new hosted Kontena Platform, as well as provisioning some new server nodes on the cloud provider of your choice.

Once your Kontena Platform is ready, it's time to install ZooKeeper.  We will do this by installing the ready-made [Zookeeper stack](https://github.com/kontena/kontena-stacks/tree/master/zookeeper).  I'm assuming you are using the integrated Kontena web terminal shell, but you can run all of these commands yourself using the [Kontena CLI](https://kontena.io/docs/tools/cli.html) and optional [Kontena Shell (Kosh)](https://github.com/kontena/kontena-plugin-shell).

- Create a new data volume: `volume create --scope instance --driver local zookeeper-cluster-data`
- Install the stack: `stack install kontena/zookeeper-cluster`

Next up is Kafka.

- Create a new data volume: `volume create --scope instance --driver local kafka-cluster-data`
- Install the stack: `stack install kontena/kafka-cluster`

Once that completes, let's connect an interactive shell to our Kafka service and make sure everything is working.

- Start the shell: `service exec -it kafka-cluster/kafka bash`
- Change directory: `cd /usr/bin`
- Create a topic: `./kafka-topics --create --zookeeper $KAFKA_ZOOKEEPER_CONNECT:2181 --replication-factor 1 --partitions 1 --topic mytopic`
- List back our topic: `./kafka-topics --list --zookeeper $KAFKA_ZOOKEEPER_CONNECT:2181`

Now, staying in our shell, let's create the topics we need for our demo application:

- `./kafka-topics --create --zookeeper $KAFKA_ZOOKEEPER_CONNECT:2181 --replication-factor 2 --partitions 1 --config cleanup.policy=compact --config retention.bytes=-1 --config retention.ms=-1 --topic eventbus.products`
- `./kafka-topics --create --zookeeper $KAFKA_ZOOKEEPER_CONNECT:2181 --replication-factor 2 --partitions 1 --config cleanup.policy=compact --config retention.bytes=-1 --config retention.ms=-1 --topic eventbus.customers`
- `./kafka-topics --create --zookeeper $KAFKA_ZOOKEEPER_CONNECT:2181 --replication-factor 2 --partitions 1 --config cleanup.policy=compact --config retention.bytes=-1 --config retention.ms=-1 --topic eventbus.purchases`

And, finally, let's install our demo application:
- `stack install kontena/eventsourcing-example`

On the Kontena Cloud web UI (or Kontena CLI `node show` command), get the public IP address of one of your nodes and enter it in the browser.  You should see a dashboard page showing customers and products.

To enter data in the system, we need to run the included CLI shell application, which is actually just the Ruby interactive REPL (aka IRB), with some classes for interacting with our microservices (strongly inspired by Stripe's Ruby SDK).  C# may be a great language for building microservices, but an interactive CLI experience is not one of it's strong suites.  As our microservices are running inside the Kontena network, we need to run our shell inside the network as well.  Again, Kontena's interactive service exec feature to the rescue!

- Start the shell: `service exec -it eventsourcing-example/client ./shell`
- Create a product: `p = Product.create name: 'Butter', price: 1.50`
- Create a customer: `c = Customer.create first_name: 'Danny', last_name: 'Tester'`
- Create a purchase: `Purchase.create product_id: p.id, customer_id: c.id, total: 1.99`
- List all customers: `customers = Customer.list`
- Retrieve a single customer: `c = Customer.retrieve('id123')`
- Update a retrieved customer: `c.first_name = 'Ted'; c.save;`
- Delete a retrieved customer: `c.delete`

As you modify data in the system, you should see it immediately update in the web UI, thanks to Kafka's publish and subscribe combined with realtime websockets.

## High Level Architecture

- 3 Microservices, each written in C# and Asp.Net Core 2.0
- A web dashboard for viewing data, also written in C# and Asp.Net Core 2.0 with TypeScript for the frontend.  Setting this up was an absolute breeze, thanks to the `dotnet` CLI tool: just run `dotnet new react` and you are up and running with a brand new web application, with React, TypeScript, Webpack and server side pre-rendering.  And the best part is, everything is handled by `dotnet` CLI.
- A CLI shell written in Ruby, for interacting with the data.
- A `kontena.yml` file that describes how to build and deploy our application.  Generally you will have one of these per code repository (so, in the real world we probably would have this split out into 5 Git repositories each with it's own `kontena.yml`).

## Building the Application

Just run `kontena stack build`, which will execute all of the `pre_build` hooks (`dotnet publish`), build Docker images and push them to Dockerhub (or whatever external repository you may be using).  Then a `kontena stack reg push` command is all it takes to push this entire stack to the public Kontena Stack registry.  Note that this is public only as of the time of this writing, but private stack registries are coming!

## Low Level Architecture

### The Microservices

At the heart of all of our microservices is a Class Library project `Kontena.EventSourcing`.  This contains shared classes for doing things like:

- Subscribing to a Kafka topic
- Publishing to Kafka topics
- Models for describing an Event
- An in-memory Repository class

Each service exposes a REST Web API for interacting with whatever type of data the service manages.  They all share the following characteristics in common:

- `GET` requests service data from an in-memory repository
- `POST`/`PUT`/`DELETE` requests create an `Event` object and publish it to a Kafka topic

Each service also subscribes to a Kafka topic for handling published events.  This subscriber takes the event and updates the in-memory repository.  The subscriber is configured to read from the beginning of the topic every time, which ensures that each service instance always has the latest data as it reads each event from the topic in order.

### The Dashboard

The dashboard:

- Hosts the Javascript frontend
- Subscribes to *all* of the Kafka topics
- Offers a websocket endpoint

The reason all of the topics are subscribed to is that, instead of querying each microservice for it's individual data and joining those results, the dashboard stores it's own version of the data in a format that is suitable for the types of queries it needs to process.  Some REST APIs are exposed for allowing the frontend to submit queries that join and aggregate the various data types, and it's not too hard to imagine replacing REST with something like GraphQL, also querying in-memory representations of our data.

The websocket part is actually a custom middleware class that also subscribes to the various topics.  As data comes in, a notification is sent to the user over the socket so that they can refresh their current view.  Of course in a real world application you would need some more sophisticated user management for each socket.
