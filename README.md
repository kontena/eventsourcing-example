# Event Sourcing Microservices Example

This repository is an example of a microservices style application using the event sourcing pattern, with Kafka as both the event publishing mechanism and the events source of truth.  It contains a mixture of C# (.Net Core 2.0), Ruby and TypeScript.

## Setup
1. You will need a Kontena Platform.  See our [quick-start guide](https://kontena.io/docs/quick-start.html) for help getting started.
2. Install the [Zookeeper](https://github.com/kontena/kontena-stacks/tree/master/zookeeper) and [Kafka](https://github.com/kontena/kontena-stacks/tree/master/kafka) stacks on your Kontena Platform.
3. Install the eventsourcing-tutorial stack: `$ kontena stack install kontena/eventsourcing-example`

