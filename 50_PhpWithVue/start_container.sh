#!/bin/bash
YELLOW='\033[0;33m'   # Yellow
NC='\033[0m'          # No Color

IMAGE=php_devserver
DATA_VOL="$(pwd)/htdocs"
DIR=$(pwd)

if [ -z "$(command -v npm)" ]; then
    echo -e ${YELLOW}Node package manager npm not found. Is Node.js installed?${NC}
    read
    exit 1
fi

docker ps &> /dev/null
if [ $? != 0 ]; then
    echo -e ${YELLOW}Cannot read docker images. Is Docker Desktop running?${NC}
    read
    exit 2
fi

if [ -z "$(docker images -a | grep $IMAGE)" ]; then
    echo Image $IMAGE not found, we will build...
    cd "$DIR/php-docker"
    docker build -t $IMAGE .
    [ $? != 0 ] && read && exit 3
fi

# Build Vue.js app
cd "$DIR/vue_client"
echo Build Vue.js app in $DIR/vue_client
npm i && npm run build

# Start docker container for created image
cd "$DIR"
docker rm -f php_api_demo_app &> /dev/null
MSYS_NO_PATHCONV=1 docker run -p 443:443 -v "$DATA_VOL:/var/www/html" -d --name php_api_demo_app $IMAGE > /dev/null
[ $? != 0 ] && read && exit 5

echo
echo wwwroot is $DATA_VOL
echo
echo Container started, you can visit https://localhost in your browser.
echo Press ENTER to close this window.
read
