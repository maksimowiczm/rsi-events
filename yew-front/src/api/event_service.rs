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

    pub async fn get_events(
        &self,
        title: Option<&str>,
        dates: Option<(&str, &str)>,
    ) -> Vec<EventEntity> {
        let url = if let (Some(title), Some(dates)) = (title, dates) {
            format!(
                "/api/events?title={}start={}&end={}",
                title, dates.0, dates.1
            )
        } else if let (None, Some(dates)) = (title, dates) {
            format!("/api/events?start={}&end={}", dates.0, dates.1)
        } else if let (Some(title), None) = (title, dates) {
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

    pub async fn get_event(&self, id: &str) -> Option<EventEntity> {
        let url = format!("/api/events/{}", id);
        let response = Request::get(&url).send().await.unwrap();

        if response.status() == 200 {
            response.json().await.ok()
        } else {
            None
        }
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

    pub async fn update_event(
        &self,
        id: &str,
        name: String,
        description: String,
        event_type: String,
        date: String,
    ) {
        let url = format!("/api/events/{}", id);
        let event = EventEntity {
            id: "".to_string(),
            title: name,
            description,
            event_type,
            date,
        };
        Request::put(&url)
            .header("Content-Type", "application/json")
            .body(serde_json::to_string(&event).unwrap())
            .unwrap()
            .send()
            .await
            .unwrap();
    }

    pub async fn delete_event(&self, id: &str, token: &str) -> bool {
        let url = format!("/api/events/{}", id);
        Request::delete(&url)
            .header("Authorization", &format!("Bearer {}", token))
            .send()
            .await
            .unwrap()
            .ok()
    }
}
