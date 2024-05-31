use gloo_net::http::Request;
use serde::{Deserialize, Serialize};

#[derive(Clone, PartialEq, Debug, Deserialize, Serialize)]
pub struct EventEntity {
    pub id: String,
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

    pub async fn get_events(&self, title: Option<&str>) -> Vec<EventEntity> {
        let url = if let Some(title) = title {
            format!("/api/events?title={}", title)
        } else {
            "/api/events".to_string()
        };
        let fetched_events: Vec<EventEntity> = Request::get(&url)
            .send()
            .await
            .unwrap()
            .json()
            .await
            .unwrap();

        fetched_events
    }

    pub async fn create_event(
        &self,
        name: String,
        description: String,
        event_type: String,
        date: String,
    ) {
        let url = "/api/events";
        let event = EventEntity {
            id: "".to_string(),
            title: name,
            description,
            event_type,
            date,
        };
        Request::post(url)
            .header("Content-Type", "application/json")
            .body(serde_json::to_string(&event).unwrap())
            .unwrap()
            .send()
            .await
            .unwrap();
    }
}
