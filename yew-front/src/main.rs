use crate::api::event_service::EventService;
use components::event_list::EventList;
use yew::prelude::*;

mod api;
mod components;

#[function_component]
fn App() -> Html {
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
        <div>
            <EventList events={(*list).clone()} />
        </div>
    }
}

fn main() {
    yew::Renderer::<App>::new().render();
}
