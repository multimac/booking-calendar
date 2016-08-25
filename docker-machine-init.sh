#!/bin/bash -e

indent() { sed 's/^/  /'; }

echo 'Launching docker machine...'
docker-machine start | indent

echo 'Configuring environment...'
eval $(docker-machine env) | indent

echo 'Mapping local folders...'
docker-machine ssh default 'sudo mkdir --parents /e/Documents/development' | indent
docker-machine ssh default 'sudo mount -t vboxsf e/Documents/development /e/Documents/development' | indent
