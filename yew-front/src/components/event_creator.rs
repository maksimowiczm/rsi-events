use crate::api::event_service::EventEntity;
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

#[derive(Debug)]
pub enum Msg {
    Create,
    Update,
    Wait,
}

#[derive(Default)]
pub struct EventCreator {
    refs: HashMap<CreatorField, NodeRef>,
    error: bool,
    waiting: bool,
}

impl EventCreator {
    fn get_field_value(&mut self, field: &CreatorField) -> String {
        let value = self
            .refs
            .get(field)
            .unwrap()
            .cast::<HtmlInputElement>()
            .unwrap()
            .value();

        if value.is_empty() {
            self.error = true;
        }

        value
    }
}

#[derive(Clone, PartialEq, Properties)]
pub struct EventCreatorProps {
    pub event: Option<EventEntity>,
}

impl Component for EventCreator {
    type Message = Msg;
    type Properties = EventCreatorProps;

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
            Msg::Create => {
                let name = self.get_field_value(&CreatorField::Name);
                let description = self.get_field_value(&CreatorField::Description);
                let event_type = self.get_field_value(&CreatorField::Type);
                let date = self.get_field_value(&CreatorField::Date);

                if self.error {
                    return true;
                }

                let navigator = ctx.link().navigator().unwrap();

                wasm_bindgen_futures::spawn_local(async move {
                    let event_service = crate::api::event_service::EventService::new();
                    event_service
                        .create_event(name, description, event_type, date)
                        .await;

                    navigator.push(&crate::Route::Home);
                });

                ctx.link().send_message(Msg::Wait);

                false
            }
            Msg::Update => {
                let name = self.get_field_value(&CreatorField::Name);
                let description = self.get_field_value(&CreatorField::Description);
                let event_type = self.get_field_value(&CreatorField::Type);
                let date = self.get_field_value(&CreatorField::Date);

                if self.error {
                    return true;
                }

                let navigator = ctx.link().navigator().unwrap();
                let id = ctx.props().event.as_ref().unwrap().id.clone();

                wasm_bindgen_futures::spawn_local(async move {
                    let event_service = crate::api::event_service::EventService::new();

                    event_service
                        .update_event(&id, name, description, event_type, date)
                        .await;

                    navigator.push(&crate::Route::Home);
                });

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
        let event = ctx.props().event.as_ref();

        let button = if self.waiting {
            match event {
                Some(_) => html! {
                    <button class={"bg-blue-500 text-white p-2 rounded-md"}>{"Updating Event..."}</button>
                },
                None => html! {
                    <button class={"bg-blue-500 text-white p-2 rounded-md"}>{"Creating Event..."}</button>
                },
            }
        } else {
            match event {
                Some(_) => html! {
                    <button onclick={ ctx.link().callback(|_: MouseEvent| Msg::Update)} class={"bg-blue-500 text-white p-2 rounded-md"}>{"Update Event"}</button>
                },
                None => html! {
                    <button onclick={ ctx.link().callback(|_: MouseEvent| Msg::Create)} class={"bg-blue-500 text-white p-2 rounded-md"}>{"Create Event"}</button>
                },
            }
        };

        let error = match self.error {
            true => html! {
                <div class={"text-red-500"}>{"All fields are required"}</div>
            },
            false => html! {},
        };

        let name_value = event.map_or_else(|| "".to_string(), |e| e.title.clone());
        let description_value = event.map_or_else(|| "".to_string(), |e| e.description.clone());
        let event_type_value = event.map_or_else(|| "".to_string(), |e| e.event_type.clone());
        let date_value = event.map_or_else(|| "".to_string(), |e| e.date.clone());

        html! {
            <div class={"p-2"} >
                <div class={"flex flex-col"}>
                    <label for={"event-name"}>{"Event Name"}</label>
                    <input value={name_value} ref={name} id={"event-name"} type={"text"} class={"bg-gray-50 border border-gray-300 text-gray-900"} required={true} />
                </div>
                <div class={"flex flex-col"}>
                    <label for={"event-description"}>{"Event Description"}</label>
                    <textarea value={description_value} ref={description} id={"event-description"} class={"bg-gray-50 border border-gray-300 text-gray-900"} required={true} />
                </div>
                <div class={"flex flex-col"}>
                    <label for={"event-type"}>{"Event Type"}</label>
                    <select value={event_type_value} ref={event_type} id={"event-type"}>
                        <option>{"Workshop"}</option>
                        <option>{"Conference"}</option>
                        <option>{"Seminar"}</option>
                    </select>
                </div>
                <div class={"flex flex-col"}>
                    <label for={"event-date"}>{"Event Date"}</label>
                    <input value={date_value} ref={date} id={"event-date"} type={"date"} class={"bg-gray-50 border border-gray-300 text-gray-900"} required={true} />
                </div>
                { error }
                { button }
            </div>
        }
    }
}
