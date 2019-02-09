#!/bin/sh

export working=$(pwd)/.working

mkdir -p .working/amps

docker-compose down
docker-compose up