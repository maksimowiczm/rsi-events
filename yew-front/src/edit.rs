use crate::api::event_service::{EventEntity, EventService};
use crate::components::event_creator::EventCreator;
use std::ops::Deref;
use yew::prelude::*;

#[derive(PartialEq, Properties)]
pub struct EditProps {
    pub id: String,
}

#[function_component]
pub fn Edit(props: &EditProps) -> Html {
    let event = use_state(|| None::<EventEntity>);
    {
        let event = event.clone();
        let id = props.id.clone();
        use_effect_with((), move |_| {
            let event_service = EventService::new();
            wasm_bindgen_futures::spawn_local(async move {
                let fetched_event = event_service.get_event(&id).await;
                event.set(Some(fetched_event));
            });
            || {}
        });
    }

    html! {
        <div>
            <EventCreator event={event.clone().deref().clone()} />
        </div>
    }
}
