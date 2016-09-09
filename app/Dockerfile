FROM node:slim

COPY ./ /data/web/

RUN cd /data/web/ && npm --loglevel=warn install --production

VOLUME /data/web/static/