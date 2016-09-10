FROM node:slim

COPY ./ /data/web/

RUN cd /data/web/ && npm --loglevel=warn install --production && npm run gulp -- prod

VOLUME /data/web/static/