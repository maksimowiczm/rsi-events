use crate::api::event_service::EventService;
use crate::components::event_list::EventList;
use crate::Route;
use gloo_console::info;
use yew::prelude::*;
use yew_router::prelude::*;

#[function_component]
pub fn Home() -> Html {
    let search = use_state(|| None::<String>);
    let search_from = use_state(|| None::<String>);
    let search_to = use_state(|| None::<String>);

    let search_ref = use_node_ref();
    let search_from_ref = use_node_ref();
    let search_to_ref = use_node_ref();

    let list = use_state(|| vec![]);
    {
        let list = list.clone();
        let search = search.clone();
        let search_from = search_from.clone();
        let search_to = search_to.clone();
        use_effect_with(
            (search.clone(), search_from.clone(), search_to.clone()),
            move |_| {
                let event_service = EventService::new();
                wasm_bindgen_futures::spawn_local(async move {
                    let dates = if let (Some(from), Some(to)) =
                        ((*search_from).as_deref(), (*search_to).as_deref())
                    {
                        info!((*search).as_deref(), from, to);
                        Some((from, to))
                    } else {
                        None
                    };

                    let events = event_service.get_events((*search).as_deref(), dates).await;
                    list.set(events);
                });
                || {}
            },
        );
    }

    let on_search = {
        let search = search.clone();
        let search_ref = search_ref.clone();
        let search_from = search_from.clone();
        let search_from_ref = search_from_ref.clone();
        let search_to = search_to.clone();
        let search_to_ref = search_to_ref.clone();
        move |_| {
            let search_ref = search_ref.cast::<web_sys::HtmlInputElement>().unwrap();
            let value = search_ref.value();
            if value.is_empty() {
                search.set(None);
            } else {
                search.set(Some(value));
            }

            let search_from_ref = search_from_ref.cast::<web_sys::HtmlInputElement>().unwrap();
            let value = search_from_ref.value();
            if value.is_empty() {
                search_from.set(None);
            } else {
                search_from.set(Some(value));
            }

            let search_to_ref = search_to_ref.cast::<web_sys::HtmlInputElement>().unwrap();
            let value = search_to_ref.value();
            if value.is_empty() {
                search_to.set(None);
            } else {
                search_to.set(Some(value));
            }
        }
    };

    html! {
        <div class={"p-2"}>
            <div class={"p-2"}>
                <button class={"bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"}>
                    <Link<Route> to={Route::Create}>{ "create new event" }</Link<Route>>
                </button>
            </div>

            <div class={"flex"}>
                <div class={"text-2xl"}>{"Events"}</div>
                <input ref={search_ref} type="text" class={"ml-2 pl-2 border-2 grow"} placeholder={"Search"} />
                <div class={"flex justify-center items-center px-2"}>{"from"}</div>
                <input ref={search_from_ref} type="date" class={"ml-2 border-2"} />
                <div class={"flex justify-center items-center px-2"}>{"to"}</div>
                <input ref={search_to_ref} type="date" class={"ml-2 border-2"} />
                <button onclick={on_search.clone()} class={"bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full ml-2"}>{"Search"}</button>
            </div>
            <EventList events={(*list).clone()} />
        </div>
    }
}
