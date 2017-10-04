#!/usr/bin/env sh
kontena service exec --shell kafka-cluster/kafka /usr/bin/kafka-topics \
    --create \
    --zookeeper \$KAFKA_ZOOKEEPER_CONNECT:2181 \
    --replication-factor 2 \
    --partitions 1 \
    --config cleanup.policy=compact \
    --config retention.bytes=-1 \
    --config retention.ms=-1  \
    --topic eventbus.products

kontena service exec --shell kafka-cluster/kafka /usr/bin/kafka-topics \
    --create \
    --zookeeper \$KAFKA_ZOOKEEPER_CONNECT:2181 \
    --replication-factor 2 \
    --partitions 1 \
    --config cleanup.policy=compact \
    --config retention.bytes=-1 \
    --config retention.ms=-1  \
    --topic eventbus.purchases

kontena service exec --shell kafka-cluster/kafka /usr/bin/kafka-topics \
    --create \
    --zookeeper \$KAFKA_ZOOKEEPER_CONNECT:2181 \
    --replication-factor 2 \
    --partitions 1 \
    --config cleanup.policy=compact \
    --config retention.bytes=-1 \
    --config retention.ms=-1  \
    --topic eventbus.customers
