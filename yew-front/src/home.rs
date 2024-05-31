use crate::api::event_service::EventService;
use crate::components::event_list::EventList;
use yew::prelude::*;

#[function_component]
pub fn Home() -> Html {
    let event_service = EventService::new();
    let list = use_state(|| vec![]);
    {
        let list = list.clone();
        use_effect_with((), move |_| {
            wasm_bindgen_futures::spawn_local(async move {
                list.set(event_service.get_events().await);
            });
            || {}
        });
    }

    html! {
            <EventList events={(*list).clone()} />
    }
}
