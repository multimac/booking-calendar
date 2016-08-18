docker-machine start
eval $(docker-machine env)

docker-machine ssh default 'sudo mkdir --parents /e/Documents/development'
docker-machine ssh default 'sudo mount -t vboxsf e/Documents/development /e/Documents/development'
