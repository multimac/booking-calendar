FROM node:slim
COPY ./ /data/web/

RUN apt-get update && apt-get install -y git

RUN cd /data/web/ && npm --loglevel=warn install --production
RUN cd /data/web/ && npm run jspm -- install && npm run typings -- install && npm run gulp -- prod

VOLUME /data/web/static/