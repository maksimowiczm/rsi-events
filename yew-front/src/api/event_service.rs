use gloo_net::http::Request;
use serde::Deserialize;

#[derive(Clone, PartialEq, Deserialize, Debug)]
pub struct EventEntity {
    pub id: String,
    #[serde(rename = "name")]
    pub title: String,
    pub description: String,
    #[serde(rename = "type")]
    pub event_type: String,
    pub date: String,
}

pub struct EventService {}

impl EventService {
    pub fn new() -> Self {
        EventService {}
    }

    pub async fn get_events(&self) -> Vec<EventEntity> {
        let url = "/api/events";
        let fetched_events: Vec<EventEntity> = Request::get(url)
            .send()
            .await
            .unwrap()
            .json()
            .await
            .unwrap();

        fetched_events
    }
}
