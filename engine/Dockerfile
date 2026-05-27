FROM rust:1.80-slim-bookworm AS builder

RUN apt-get update && apt-get install -y protobuf-compiler libprotobuf-dev

WORKDIR /usr/src/app

COPY Cargo.toml Cargo.lock ./
COPY proto ./proto

RUN mkdir src && \
    echo "fn main() {println!(\"if you see this, the build broke\")}" > src/main.rs && \
    cargo build --release && \
    rm -rf src

COPY src ./src

RUN touch src/main.rs && cargo build --release

FROM debian:bookworm-slim

RUN apt-get update && apt-get install -y libssl3 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=builder /usr/src/app/target/release/engine /app/engine

EXPOSE 50051

CMD ["./engine"]