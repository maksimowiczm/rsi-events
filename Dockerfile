FROM nginx:1.25.3-alpine

RUN apk update && \
    apk add --no-cache openssl && \
    openssl req -x509 -nodes -days 365 \
    -subj  "/C=BS/ST=Cat Island/O=Codend/CN=localhost💀" \
    -newkey rsa:2048 -keyout /etc/ssl/private/codend-selfsigned.key \
    -out /etc/ssl/certs/codend-selfsigned.crt;