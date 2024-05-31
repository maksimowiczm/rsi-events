use crate::components::event_creator::EventCreator;
use yew::prelude::*;

#[function_component]
pub fn Create() -> Html {
    html! {
        <div>
            <EventCreator />
        </div>
    }
}
