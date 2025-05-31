# MQTT Broker Image
This image is based on the [eclipse-mosquitto](https://hub.docker.com/_/eclipse-mosquitto) image. It is a MQTT broker that can be used to publish and subscribe to messages. The image has been modified to include a configuration file that requires a username and password to connect to the broker. The username and password are to be set as environment variables when running the container.

## Building the Image
1. Create a buildx builder
```bash
docker buildx create \
--name container-builder \
--driver docker-container \
--bootstrap
```
2. Build the image and push it to the Docker Hub
```bash
docker buildx build \
--tag <username>/mqtt-broker:latest \
--platform linux/amd64,linux/arm64 \
--builder container-builder \
--push .
```

## Running the Container
To run 
```bash
docker run -d --name mqtt-broker \
-p 1883:1883 \
-e MOSQUITTO_USERNAME=mosquitto \
-e MOSQUITTO_PASSWORD=mosquitto \
<username>/mqtt-broker:latest
```