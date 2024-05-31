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
    Wait,
}

#[derive(Default)]
pub struct EventCreator {
    refs: HashMap<CreatorField, NodeRef>,
    error: bool,
    waiting: bool,
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
                let mut error = false;

                let mut get_field_value = |field: &CreatorField| {
                    let value = self
                        .refs
                        .get(field)
                        .unwrap()
                        .cast::<HtmlInputElement>()
                        .unwrap()
                        .value();

                    if value.is_empty() {
                        error = true;
                    }

                    value
                };

                let name = get_field_value(&CreatorField::Name);
                let description = get_field_value(&CreatorField::Description);
                let event_type = get_field_value(&CreatorField::Type);
                let date = get_field_value(&CreatorField::Date);

                if error {
                    self.error = true;
                    return true;
                }

                {
                    let navigator = ctx.link().navigator().unwrap();

                    wasm_bindgen_futures::spawn_local(async move {
                        let event_service = crate::api::event_service::EventService::new();
                        event_service
                            .create_event(name, description, event_type, date)
                            .await;

                        navigator.push(&crate::Route::Home);
                    });
                }

                ctx.link().send_message(Msg::Wait);

                false
            }
            Msg::Wait => {
                self.waiting = true;
                true
            }
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let name = self.refs.get(&CreatorField::Name).unwrap();
        let description = self.refs.get(&CreatorField::Description).unwrap();
        let event_type = self.refs.get(&CreatorField::Type).unwrap();
        let date = self.refs.get(&CreatorField::Date).unwrap();

        let button = if self.waiting {
            html! {
                <button class={"bg-blue-500 text-white p-2 rounded-md"}>{"Creating Event..."}</button>
            }
        } else {
            html! {
                <button onclick={ ctx.link().callback(|_: MouseEvent| Msg::Submit)} class={"bg-blue-500 text-white p-2 rounded-md"}>{"Create Event"}</button>
            }
        };

        let error = if self.error {
            html! {
                <div class={"text-red-500"}>{"All fields are required"}</div>
            }
        } else {
            html! {}
        };

        html! {
            <div class={"p-2"} >
                <div class={"flex flex-col"}>
                    <label for={"event-name"}>{"Event Name"}</label>
                    <input ref={name} id={"event-name"} type={"text"} class={"bg-gray-50 border border-gray-300 text-gray-900"} required={true} />
                </div>
                <div class={"flex flex-col"}>
                    <label for={"event-description"}>{"Event Description"}</label>
                    <textarea ref={description} id={"event-description"} class={"bg-gray-50 border border-gray-300 text-gray-900"} required={true} />
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
                    <input ref={date} id={"event-date"} type={"date"} class={"bg-gray-50 border border-gray-300 text-gray-900"} required={true} />
                </div>
                { error }
                { button }
            </div>
        }
    }
}
