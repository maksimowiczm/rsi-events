use crate::api::event_service::EventService;
use crate::components::event_list::EventList;
use crate::Route;
use yew::prelude::*;
use yew_router::prelude::*;

#[function_component]
pub fn Home() -> Html {
    let search = use_state(|| None::<String>);
    let search_ref = use_node_ref();

    let list = use_state(|| vec![]);
    {
        let list = list.clone();
        let search = search.clone();
        use_effect_with(search.clone(), move |_| {
            let event_service = EventService::new();
            wasm_bindgen_futures::spawn_local(async move {
                let events = event_service.get_events((*search).as_deref()).await;
                list.set(events);
            });
            || {}
        });
    }

    let on_search = {
        let search = search.clone();
        let search_ref = search_ref.clone();
        move |_| {
            let search_ref = search_ref.cast::<web_sys::HtmlInputElement>().unwrap();
            let value = search_ref.value();
            if value.is_empty() {
                search.set(None);
                return;
            }

            search.set(Some(search_ref.value()));
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
                <button onclick={on_search.clone()} class={"bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-full ml-2"}>{"Search"}</button>
            </div>
            <EventList events={(*list).clone()} />
        </div>
    }
}
