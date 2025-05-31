# API Reference

This API reference documents all the REST endpoints present in the backend (web_server) for authentication, user management, mantrap management, biometrics enrollment and well as access control monitoring.

These endpoints are mostly utilised by the frontend (web_client) for administrators and security personnel to manage the platform via the browser.

## Authentication

### [POST] /api/auth/login

Login to the security portal.

Access: `Admin, Security`

**Parameters**

| Name     | Type   | Mandatory | Description    |
|----------|--------|-----------|----------------|
| username | string | Yes       | Login username |
| password | string | Yes       | Login password |

**Example Request**
```
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "P@ssw0rd"
}
```

**Example Response**
```
{
  "data": {
    "id": 1,
    "role": "admin",
    "username": "admin"
  },
  "message": "Login successful"
}
```

### [GET] /api/auth/logout

Logout from the security portal.

Access: `Admin, Security`

**Example Request**
```
GET http://localhost:5000/api/auth/logout
```

**Example Response**
```
{
  "message": "Logout successful"
}
```

## User

### [POST] /api/user/list

List users in the system.

Access: `Admin`

**Parameters**

| Name     | Type    | Mandatory | Description      |
|----------|---------|-----------|------------------|
| id       | integer | Yes       | ID of user       |
| username | string  | Yes       | Username of user |
| email    | string  | Yes       | Email of user    |
| role     | string  | Yes       | Role of users    |

**Example Request**
```
POST http://localhost:5000/api/user/list
Content-Type: application/json

{
  "role": "admin"
}
```

**Example Response**
```
{
  "data": [
    {
      "account_locked": false,
      "alert_subscribed": false,
      "biometrics_enrolled": false,
      "email": "admin@spmovy.com",
      "id": 1,
      "location": "Outside",
      "role": "admin",
      "username": "admin"
    }
  ]
}
```

### [POST] /api/user/add

Add new user into the system.

Access: `Admin`

**Parameters**

| Name             | Type    | Mandatory | Description                                  |
|------------------|---------|-----------|----------------------------------------------|
| username         | string  | Yes       | Username of user                             |
| email            | string  | Yes       | Email of user                                |
| password         | string  | Yes       | Password of user                             |
| role             | string  | Yes       | User role <br/> (admin, security, personnel) |
| alert_subscribed | boolean | Yes       | Alert subscription status                    |

**Example Request**
```
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "security",
  "email": "security@spmovy.com",
  "password": "P@ssw0rd",
  "role": "security",
  "alert_subscribed": false
}
```

**Example Response**
```
{
  "message": "User added successfully"
}
```

### [POST] /api/user/update/<user_id>

Update user details.

Access: `Admin`

**Parameters**

| Name             | Type    | Mandatory | Description                                  |
|------------------|---------|-----------|----------------------------------------------|
| user_id          | string  | Yes       | ID of user (URL parameter)                   |
| username         | string  | No        | Username of user                             |
| email            | string  | No        | Email of user                                |
| password         | string  | No        | Password of user                             |
| role             | string  | No        | User role <br/> (admin, security, personnel) |
| alert_subscribed | boolean | No        | Alert subscription status                    |

**Example Request**
```
POST http://localhost:5000/api/user/update/1
Content-Type: application/json

{
  "email": "security+1@spmovy.com",
  "password": "P@ssw0rd123",
  "alert_subscribed": true
}
```

**Example Response**
```
{
  "message": "User updated successfully"
}
```

### [DELETE] /api/user/delete/<user_id>

Delete user from the system. Cannot delete currently logged in account.

Access: `Admin`

**Parameters**

| Name             | Type    | Mandatory | Description                |
|------------------|---------|-----------|----------------------------|
| user_id          | string  | Yes       | ID of user (URL parameter) |

**Example Request**
```
DELETE http://localhost:5000/api/user/delete/1
```

**Example Response**
```
{
  "message": "User deleted successfully"
}
```

## Mantrap

### [POST] /api/mantrap/list

List mantraps in the system.

Access: `Admin, Security`

**Parameters**

| Name     | Type    | Mandatory | Description         |
|----------|---------|-----------|---------------------|
| location | string  | No        | Location of mantrap |

**Example Request**
```
POST http://localhost:5000/api/mantrap/list
Content-Type: application/json

{
  "location": "E2-05-05"
}
```

**Example Response**
```
{
  "data": [
    {
      "entry_gate_status": "CLOSED",
      "exit_gate_status": "CLOSED",
      "id": 1,
      "location": "E2-05-05",
      "token": "a1b2c3d4e5d6f7"
    }
  ]
}
```

### [POST] /api/mantrap/add

Add a new mantrap into the system.

Access: `Admin`

**Parameters**

| Name     | Type    | Mandatory | Description         |
|----------|---------|-----------|---------------------|
| location | string  | No        | Location of mantrap |

**Example Request**
```
POST http://localhost:5000/api/mantrap/add
Content-Type: application/json

{
  "location": "E5-05-06"
}
```

**Example Response**
```
{
  "message": "Mantrap added successfully"
}
```

### [POST] /api/mantrap/update/<mantrap_id>

Update mantrap details.

Access: `Admin`

**Parameters**

| Name       | Type    | Mandatory | Description                                 |
|------------|---------|-----------|---------------------------------------------|
| mantrap_id | string  | No        | ID of the mantrap to update (URL parameter) |
| location   | string  | No        | Location of mantrap                         |

**Example Request**
```
POST http://localhost:5000/api/mantrap/update/2
Content-Type: application/json

{
  "location": "E2-05-07"
}
```

**Example Response**
```
{
  "message": "Mantrap updated successfully"
}
```

### [DELETE] /api/mantrap/delete/<mantrap_id>

Remove mantrap from the system.

Access: `Admin`

**Parameters**

| Name       | Type    | Mandatory | Description                                 |
|------------|---------|-----------|---------------------------------------------|
| mantrap_id | string  | Yes       | ID of the mantrap to delete (URL parameter) |

**Example Request**
```
DELETE http://localhost:5000/api/mantrap/delete/2
```

**Example Response**
```
{
  "message": "Mantrap deleted successfully"
}
```

### [GET] /api/mantrap/<mantrap_id>/\<action>

Manually override gate status of mantrap.

Access: `Admin, Security`

**Parameters**

| Name       | Type    | Mandatory | Description                                                 |
|------------|---------|-----------|-------------------------------------------------------------|
| mantrap_id | string  | Yes       | ID of mantrap (URL parameter)                               |
| action     | string  | Yes       | Action to perform on the gate (open, close) (URL parameter) |

**Example Request**
```
GET http://localhost:5000/api/mantrap/1/open
```

**Example Response**
```
{
  "message": "Command sent to open mantrap"
}
```

## Biometrics

### [POST] /api/biometrics/enroll/<user_id>

Enroll user biometrics.

Access: `Admin`

**Parameters**

| Name    | Type   | Mandatory | Description                                                                   |
|---------|--------|-----------|-------------------------------------------------------------------------------|
| user_id | string | Yes       | ID of user (URL parameter)                                                    |
| face_i  | file   | Yes       | Image of user face (5 required, named face_0, face_1, face_2, face_3, face_4) |
| voice   | file   | Yes       | Audio of user voice                                                           |

**Example Request**
```
POST http://localhost:5000/api/biometrics/enroll/1
Content-Type: multipart/form-data; boundary=WebAppBoundary

--WebAppBoundary
Content-Disposition: form-data; name="face_0"; filename="face.png"
Content-Type: image/png

< ./face.png
--WebAppBoundary
Content-Disposition: form-data; name="face_1"; filename="face.png"
Content-Type: image/png

< ./face.png
--WebAppBoundary
Content-Disposition: form-data; name="face_2"; filename="face.png"
Content-Type: image/png

< ./face.png
--WebAppBoundary
Content-Disposition: form-data; name="face_3"; filename="face.png"
Content-Type: image/png

< ./face.png
--WebAppBoundary
Content-Disposition: form-data; name="face_4"; filename="face.png"
Content-Type: image/png

< ./face.png
--WebAppBoundary
Content-Disposition: form-data; name="voice"; filename="voice.wav"
Content-Type: audio/wav

< ./voice.wav
--WebAppBoundary
```

**Example Response**
```
{
  "message": "Biometrics enrolled successfully"
}
```

### [DELETE] /api/biometrics/delete/<user_id>

Delete user biometrics.

Access: `Admin`

**Parameters**

| Name             | Type    | Mandatory | Description                          |
|------------------|---------|-----------|--------------------------------------|
| user_id          | string  | Yes       | ID of user whose biometric to delete |

**Example Request**
```
DELETE http://localhost:5000/api/biometrics/delete/1
```

**Example Response**
```
{
  "message": "Biometrics deleted successfully"
}
```

### [GET] /api/biometrics/embeddings/\<token>/\<filename>

Retrieve latest biometrics embeddings. Called by edge device (Raspberry Pi) biometrics are added or removed.

Access: `Edge Device (Raspberry Pi)`

**Parameters**

| Name     | Type    | Mandatory | Description                             |
|----------|---------|-----------|-----------------------------------------|
| token    | string  | Yes       | Access token to biometrics (pre-shared) |
| filename | string  | Yes       | Biometric filename                      |

**Example Request**
```
### Get face embeddings
GET http://localhost:5000/api/biometrics/embeddings/2e048d59-cbfb-4444-a8b9-7d90430fa6ce/face_embeddings.pkl

### Get voice embeddings
GET http://localhost:5000/api/biometrics/embeddings/2e048d59-cbfb-4444-a8b9-7d90430fa6ce/voiceprints.pkl
```

**Example Response**
```
face_embeddings.pkl
voiceprints.pkl
```

## Access Logs

### [POST] /api/log/list

List system logs.

Access: `Admin, Security`**Parameters**

| Name       | Type    | Mandatory | Description                |
|------------|---------|-----------|----------------------------|
| category   | string  | Yes       | Category of logs to filter |
| user_id    | string  | Yes       | User ID logs to filter     |
| mantrap_id | string  | Yes       | Mantrap ID logs to filter  |

**Example Request**
```
{
  "user_id": 3,
  "category": "verification_successful"
}
```

**Example Response**
```
{
  "data": [
    {
      "category": "verification_successful",
      "file": null,
      "id": 142,
      "mantrap_id": null,
      "message": "User ernestfoo verified successfully",
      "timestamp": "Thu, 27 March     2025 15:51:11",
      "user_id": 3
    }
  ]
}
```



### [GET] /api/log/file/\<filename>

Get file (image) associated with log.

Access: `Admin, Security`

**Parameters**

| Name     | Type    | Mandatory | Description                       |
|----------|---------|-----------|-----------------------------------|
| filename | string  | Yes       | Filename retrieved from logs data |

**Example Request**
```
GET http://localhost:5000/api/log/file/multi_20250309223558333.png
```

**Example Response**
```
multi_20250309223558333.png
```