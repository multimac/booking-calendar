FROM postgres:latest
COPY ./initdb/ /docker-entrypoint-initdb.d/

RUN apt-get update && apt-get install -y postgresql-contrib