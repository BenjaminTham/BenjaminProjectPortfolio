1. Create a password file in the config directory

```bash
touch /config/passwd
```

2. Run Container

```bash
docker compose -p mqtt5 up -d
```

2. Check Container

```bash
docker ps
```

4. User Management

- Add user

```bash
docker exec -it mqtt5 chmod 0700 /mosquitto/config/passwd
docker exec -it mqtt5 mosquitto_passwd -c /mosquitto/config/passwd <username>
```

- Add additional users

```bash
docker exec -it mqtt5 mosquitto_passwd /mosquitto/config/passwd <username>
```

- Remove user

```bash
docker exec -it mqtt5 mosquitto_passwd -D /mosquitto/config/passwd <username>
```

5. Restart Container

```bash
docker restart mqtt5
```