FROM lukemathwalker/cargo-chef:latest as chef

RUN rustup target add wasm32-unknown-unknown
ADD https://github.com/thedodd/trunk/releases/download/v0.17.2/trunk-x86_64-unknown-linux-gnu.tar.gz ./tmp
RUN cd /tmp && tar xf trunk-x86_64-unknown-linux-gnu.tar.gz && chmod +x trunk && mv trunk /bin
WORKDIR /app


FROM chef AS planner

COPY ./Cargo.toml ./Cargo.lock ./
COPY ./src ./src
RUN cargo chef prepare


FROM chef AS builder

COPY --from=planner /app/recipe.json .
RUN cargo chef cook --release
COPY ./src ./src
COPY ./index.html ./
COPY ./tailwind.config.js ./
COPY ./Trunk.toml ./
RUN trunk build

FROM builder AS serve

CMD [ "trunk", "serve" ]