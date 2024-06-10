use crate::api::event_service::EventService;
use yew::prelude::*;
use yew_router::prelude::RouterScopeExt;

pub struct Delete {
    input_ref: NodeRef,
    state: Msg,
}

#[derive(PartialEq, Properties)]
pub struct DeleteProps {
    pub id: String,
}

#[derive(Debug)]
pub enum Msg {
    Delete,
    Wait,
    Error,
}

impl Component for Delete {
    type Message = Msg;
    type Properties = DeleteProps;

    fn create(_: &Context<Self>) -> Self {
        Self {
            input_ref: NodeRef::default(),
            state: Msg::Delete,
        }
    }

    fn update(&mut self, ctx: &Context<Self>, msg: Self::Message) -> bool {
        match msg {
            Msg::Delete => {
                let input_ref = self.input_ref.clone();
                let id = ctx.props().id.clone();

                let token = input_ref
                    .cast::<web_sys::HtmlInputElement>()
                    .unwrap()
                    .value();

                let link = ctx.link().clone();
                let navigator = link.navigator().unwrap();

                wasm_bindgen_futures::spawn_local(async move {
                    let event_service = EventService::new();
                    let result = event_service.delete_event(&id, &token).await;
                    if result {
                        navigator.push(&crate::Route::Home);
                    } else {
                        link.send_message(Msg::Error);
                    }
                });

                ctx.link().send_message(Msg::Wait);

                false
            }
            Msg::Wait => false,
            Msg::Error => {
                self.state = Msg::Delete;
                true
            }
        }
    }

    fn view(&self, ctx: &Context<Self>) -> Html {
        let on_delete = { ctx.link().callback(|_: MouseEvent| Msg::Delete) };

        let message = match self.state {
            Msg::Wait => "Deleting event...",
            Msg::Error => "Error deleting event",
            _ => "",
        };

        html! {
            <div class={"p-3"}>
                <div>{format!("Delete event with id: {}", ctx.props().id)}</div>
                <div>{"Provide a authentication token:"}</div>
                <input
                    ref={self.input_ref.clone()}
                    type={"text"}
                    class={"bg-gray-50 border border-gray-300 text-gray-900 my-2"}
                    required={true}
                />
                <div class={"text-red-500"}>{message}</div>
                <div>
                    <button onclick={on_delete} class={"bg-red-500 text-white p-2 rounded-md"}>{"delete"}</button>
                </div>
            </div>
        }
    }
}
