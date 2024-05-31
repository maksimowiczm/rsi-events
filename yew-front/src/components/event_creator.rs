use std::collections::HashMap;
use web_sys::HtmlInputElement;
use yew::prelude::*;
use yew_router::prelude::*;

#[derive(Eq, Hash, PartialEq)]
enum CreatorField {
    Name,
    Description,
    Type,
    Date,
}

pub enum Msg {
    Submit,
}

#[derive(Default)]
pub struct EventCreator {
    refs: HashMap<CreatorField, NodeRef>,
}

impl Component for EventCreator {
    type Message = Msg;
    type Properties = ();

    fn create(_: &Context<Self>) -> Self {
        let mut refs = HashMap::new();
        refs.insert(CreatorField::Name, NodeRef::default());
        refs.insert(CreatorField::Description, NodeRef::default());
        refs.insert(CreatorField::Type, NodeRef::default());
        refs.insert(CreatorField::Date, NodeRef::default());
        Self {
            refs,
            ..Default::default()
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Submit => {
                let get_field_value = |field: &CreatorField| {
                    self.refs
                        .get(field)
                        .unwrap()
                        .cast::<HtmlInputElement>()
                        .unwrap()
                        .value()
                };

                let name = get_field_value(&CreatorField::Name);
                let description = get_field_value(&CreatorField::Description);
                let event_type = get_field_value(&CreatorField::Type);
                let date = get_field_value(&CreatorField::Date);

                wasm_bindgen_futures::spawn_local(async move {
                    let event_service = crate::api::event_service::EventService::new();
                    event_service
                        .create_event(name, description, event_type, date)
                        .await;
                });

                ctx.link().navigator().unwrap().push(&crate::Route::Home);

                false
            }
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let name = self.refs.get(&CreatorField::Name).unwrap();
        let description = self.refs.get(&CreatorField::Description).unwrap();
        let event_type = self.refs.get(&CreatorField::Type).unwrap();
        let date = self.refs.get(&CreatorField::Date).unwrap();

        html! {
            <div class={"p-2"} >
                <div class={"flex flex-col"}>
                    <label for={"event-name"}>{"Event Name"}</label>
                    <input ref={name} id={"event-name"} type={"text"} class={"bg-gray-50 border border-gray-300 text-gray-900"} />
                </div>
                <div class={"flex flex-col"}>
                    <label for={"event-description"}>{"Event Description"}</label>
                    <textarea ref={description} id={"event-description"} class={"bg-gray-50 border border-gray-300 text-gray-900"} />
                </div>
                <div class={"flex flex-col"}>
                    <label for={"event-type"}>{"Event Type"}</label>
                    <select ref={event_type} id={"event-type"}>
                        <option>{"Workshop"}</option>
                        <option>{"Conference"}</option>
                        <option>{"Seminar"}</option>
                    </select>
                </div>
                <div class={"flex flex-col"}>
                    <label for={"event-date"}>{"Event Date"}</label>
                    <input ref={date} id={"event-date"} type={"date"} class={"bg-gray-50 border border-gray-300 text-gray-900"} />
                </div>
                <button onclick={ ctx.link().callback(|_: MouseEvent| Msg::Submit)} class={"bg-blue-500 text-white p-2 rounded-md"}>{"Create Event"}</button>
            </div>
        }
    }
}
