#!/bin/bash -e

docker-compose -f docker-compose.yml -f docker-compose.dev.yml $@