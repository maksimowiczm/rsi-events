use crate::api::event_service::EventEntity;
use crate::Route;
use yew::prelude::*;
use yew_router::prelude::*;

#[derive(PartialEq, Properties)]
pub struct EventListProps {
    pub events: Vec<EventEntity>,
}

#[function_component]
pub fn EventList(EventListProps { events }: &EventListProps) -> Html {
    let list: Vec<_> = events
        .iter()
        .enumerate()
        .map(|(i, event)| {
            html! {
                <EventComponent event={event.clone()} key={i} />
            }
        })
        .collect();

    html! {
        <div class={"p-2"}>
            {list}
        </div>
    }
}

#[derive(PartialEq, Properties)]
pub struct EventComponentProps {
    event: EventEntity,
}

#[function_component]
pub fn EventComponent(EventComponentProps { event }: &EventComponentProps) -> Html {
    html! {
        <div class={"border-2 mt-2 p-2 flex flex-row"}>
            <div class={"flex grow flex-col"}>
                <div class={"flex"}>
                    <div>{"Title:"}</div>
                    <div class={"pl-2"}>{&event.title}</div>
                </div>
                <div class={"flex"}>
                    <div>{"Description:"}</div>
                    <div class={"pl-2"}>{&event.description}</div>
                </div>
                <div class={"flex"}>
                    <div>{"Type:"}</div>
                    <div class={"pl-2"}>{&event.event_type}</div>
                </div>
                <div class={"flex"}>
                    <div>{"Date:"}</div>
                    <div class={"pl-2"}>{&event.date}</div>
                </div>
            </div>
            <div class={"flex flex-col"}>
                <div class={"grow"}></div>
                <button class={"m-2 bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full"}>
                    <Link<Route> to={Route::Edit { id: event.id.clone() }}>{"edit"}</Link<Route>>
                </button>
                <button class={"m-2 bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full "}>
                    <a href={format!("/api/events/{}/pdf", event.id)} target={"_blank"}>
                        {"pdf"}
                    </a>
                </button>
            </div>
        </div>
    }
}
