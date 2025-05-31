FROM nginx:latest

COPY nginx.conf /etc/nginx/nginx.conf.template

RUN mkdir -p /etc/nginx/ssl

RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/nginx/ssl/selfsigned.key \
    -out /etc/nginx/ssl/selfsigned.crt \
    -subj "/C=SG/ST=Singapore/L=Singapore/O=GateGuard Security/OU=IT/CN=localhost"

EXPOSE 80 443

CMD ["sh", "-c", "envsubst '$$FRONTEND_SERVER $$BACKEND_SERVER' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf && nginx -g 'daemon off;'"]