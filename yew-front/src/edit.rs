use crate::api::event_service::{EventEntity, EventService};
use crate::components::event_creator::EventCreator;
use crate::Route;
use yew::prelude::*;
use yew_router::prelude::*;

#[derive(PartialEq, Properties)]
pub struct EditProps {
    pub id: String,
}

enum EventState {
    Loading,
    Loaded(EventEntity),
    NotFound,
}

#[function_component]
pub fn Edit(props: &EditProps) -> Html {
    let event = use_state(|| EventState::Loading);
    {
        let event = event.clone();
        let id = props.id.clone();
        use_effect_with((), move |_| {
            let event_service = EventService::new();
            wasm_bindgen_futures::spawn_local(async move {
                let fetched_event = event_service.get_event(&id).await;
                if let Some(fetched) = fetched_event {
                    event.set(EventState::Loaded(fetched));
                } else {
                    event.set(EventState::NotFound);
                }
            });
            || {}
        });
    }

    match &*event {
        EventState::Loading => html! { <div>{"loading"}</div> },
        EventState::Loaded(evt) => html! { <div><EventCreator event={evt.clone()} /></div> },
        _ => html! {<Redirect<Route> to={Route::Home} /> },
    }
}
