use yew::prelude::*;

mod api;
mod components;
mod create;
mod delete;
mod edit;
mod home;

use crate::create::Create;
use crate::delete::Delete;
use crate::edit::Edit;
use crate::home::Home;
use yew_router::prelude::*;

#[derive(Clone, Routable, PartialEq)]
pub enum Route {
    #[at("/")]
    Home,
    #[at("/create")]
    Create,
    #[at("/edit/:id")]
    Edit { id: String },
    #[at("/delete/:id")]
    Delete { id: String },
}

fn switch(routes: Route) -> Html {
    match routes {
        Route::Home => html! { <Home /> },
        Route::Create => html! { <Create /> },
        Route::Edit { id } => html! { <Edit id={id} /> },
        Route::Delete { id } => html! { <Delete id={id} /> },
    }
}

#[function_component]
fn App() -> Html {
    html! {
        <BrowserRouter>
            <Switch<Route> render={switch} />
        </BrowserRouter>
    }
}

fn main() {
    yew::Renderer::<App>::new().render();
}
