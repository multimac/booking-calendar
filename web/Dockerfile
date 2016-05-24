FROM nginx:latest
COPY nginx.conf /etc/nginx/nginx.conf

COPY ./static/ ./www/ /data/web/

EXPOSE 80
VOLUME /data/web/static/ /data/web/www/