FROM postgres:latest

RUN apt-get update && apt-get install -y postgresql-contrib

COPY ./initdb/ /docker-entrypoint-initdb.d/