use futures_lite::stream::StreamExt;
use lapin::options::{BasicAckOptions, BasicConsumeOptions, QueueDeclareOptions};
use lapin::types::FieldTable;
use serde::Deserialize;

#[derive(Debug, Deserialize)]
#[allow(dead_code)]
struct Event {
    title: String,
    date: String,
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

    while let Some(delivery) = consumer.next().await {
        let delivery = delivery?;
        let data = delivery.data;

        let event: Event = serde_json::from_slice(&data).unwrap();
        dbg!(event);

        channel_events
            .basic_ack(delivery.delivery_tag, BasicAckOptions::default())
            .await?;
    }

    connection.close(200, "Bye").await?;

    Ok(())
}
