FROM nginx:latest
COPY nginx.conf /etc/nginx/nginx.conf

COPY ./static/ /data/web/

EXPOSE 80
VOLUME /data/web/static/