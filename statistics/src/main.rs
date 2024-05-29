use futures_lite::stream::StreamExt;
use lapin::options::{BasicAckOptions, BasicConsumeOptions, QueueDeclareOptions};
use lapin::types::FieldTable;
use serde::Deserialize;
use std::collections::HashMap;

#[derive(Debug, Deserialize)]
#[allow(dead_code)]
struct EventCreate {
    title: String,
    date: String,
}

#[derive(Debug, Deserialize)]
#[allow(dead_code)]
struct EventVisit {
    id: String,
}

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let addr = "amqp://localhost:5672/%2f";

    let connection =
        lapin::Connection::connect(&addr, lapin::ConnectionProperties::default()).await?;

    let channel_events = connection.create_channel().await?;

    let queue_options = QueueDeclareOptions {
        durable: true,
        ..Default::default()
    };

    let _ = channel_events
        .queue_declare("events", queue_options, FieldTable::default())
        .await?;

    let mut consumer = channel_events
        .clone()
        .basic_consume(
            "events",
            "statistics",
            BasicConsumeOptions::default(),
            FieldTable::default(),
        )
        .await?;

    let mut visits = HashMap::new();

    while let Some(delivery) = consumer.next().await {
        let delivery = delivery?;

        if let Ok(visit) = serde_json::from_slice::<EventVisit>(&delivery.data) {
            let visits = *visits
                .entry(visit.id.clone())
                .and_modify(|c| *c += 1)
                .or_insert(0);

            println!("Event {} has {} visits", visit.id, visits + 1)
        } else if let Ok(event) = serde_json::from_slice::<EventCreate>(&delivery.data) {
            println!("Event created: {:?}", event);
        }

        channel_events
            .basic_ack(delivery.delivery_tag, BasicAckOptions::default())
            .await?;
    }

    connection.close(200, "Bye").await?;

    Ok(())
}
