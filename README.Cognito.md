**List Users in the Cognito User Pool**:

To see all users in your Cognito User Pool, use the following AWS CLI command:

```sh
aws cognito-idp list-users --user-pool-id eu-north-1_P3EpEiwut --region eu-north-1
```

**Example Output**:
```json
{
    "Users": [
        {
            "Username": "user",
            "Attributes": [
                {
                    "Name": "email",
                    "Value": "orenkats95@gmail.com"
                },
                {
                    "Name": "sub",
                    "Value": "a09cb93c-c091-7085-d577-52113a8dc932"
                }
            ],
            "UserCreateDate": "2025-03-12T09:42:47.130000+02:00",
            "UserLastModifiedDate": "2025-03-12T10:28:13.241000+02:00",
            "Enabled": true,
            "UserStatus": "CONFIRMED"
        },
        {
            "Username": "admin",
            "Attributes": [
                {
                    "Name": "email",
                    "Value": "orenkats95@gmail.com"
                },
                {
                    "Name": "sub",
                    "Value": "a0ac192c-e061-7041-b485-6fc404967e6f"
                }
            ],
            "UserCreateDate": "2025-03-12T09:41:55.457000+02:00",
            "UserLastModifiedDate": "2025-03-12T10:23:56.350000+02:00",
            "Enabled": true,
            "UserStatus": "CONFIRMED"
        },
        {
            "Username": "manager",
            "Attributes": [
                {
                    "Name": "email",
                    "Value": "orenkats95@gmail.com"
                },
                {
                    "Name": "sub",
                    "Value": "b0ece98c-c071-7029-7b1d-cd61854cf522"
                }
            ],
            "UserCreateDate": "2025-03-12T09:42:23.040000+02:00",
            "UserLastModifiedDate": "2025-03-12T10:28:45.446000+02:00",
            "Enabled": true,
            "UserStatus": "CONFIRMED"
        }
    ]
}
```

---

**User Login Process**:

To authenticate as a manager and obtain a token, use the following AWS CLI command:

```sh
aws cognito-idp initiate-auth --auth-flow USER_PASSWORD_AUTH --client-id 68tmnu99ooeor1go32unal8iia --auth-parameters USERNAME=manager,PASSWORD=Orenkats95! --region eu-north-1
```

**Example Output**:
```json
{
    "ChallengeParameters": {},
    "AuthenticationResult": {
        "AccessToken": "eyJraWQiOiJadzUwUnR5UTVwU2F4cUxkTm91bE1xTXVNSllJTVQ5eGtzdTlnXC80aFdhYz0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJiMGVjZTk4Yy1jMDcxLTcwMjktN2IxZC1jZDYxODU0Y2Y1MjIiLCJjb2duaXRvOmdyb3VwcyI6WyJNYW5hZ2VyIl0sImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS1ub3J0aC0xLmFtYXpvbmF3cy5jb21cL2V1LW5vcnRoLTFfUDNFcEVpd3V0IiwiY2xpZW50X2lkIjoiNjh0bW51OTlvb2VvcjFnbzMydW5hbDhpaWEiLCJvcmlnaW5fanRpIjoiN2I2ZTlmOTItZDVhNi00YzFkLThkNzktNGYzOTA5MGMyMDQ0IiwiZXZlbnRfaWQiOiI3NWVhNzI5Ny0yNTY0LTRkMmUtYjc1OS00Y2YwMWJjMzAyOWMiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIiwiYXV0aF90aW1lIjoxNzQxODQzMDI3LCJleHAiOjE3NDE4NDY2MjcsImlhdCI6MTc0MTg0MzAyNywianRpIjoiYmIzNWNjZTMtNjJhOC00YTgwLWI5OWYtMTQ1NGI5YzZkNjQzIiwidXNlcm5hbWUiOiJtYW5hZ2VyIn0.dhtcVyUSeiJRfUQMQVwcIlJebPzRE-frN_wI3MQ3xV6V_y4lZY3muMJ5SSwP373gWEV688kaAas-5Zd-fs8cV9S8vB4V1YOGEHgHvOweqbI_34Os4kfqOU_3ic_i4QRKmQ3OPKBwd8LuduUuhrxj6Kailzhw-oFqGyuP5cxrgJHWOUHeSsBGFx7tRckAIM5hD_0zfJT6Uf1NhGK6bTwqYGFj04v2Ty2GDEEZMz6laKzcT7YtzvN7P5opguyplbkx06pAuAPYG53kmi3zkP7Ls0BzCn3C0mvR1bGFxJyQ7o3_b8JLBOTiQmK3Mck80LzR5Spo3GxdZYSdQjsNSBXhSw",
        "ExpiresIn": 3600,
        "TokenType": "Bearer"
    }
}
```

---
# Note:

The presence of "cognito:groups": ["Manager"] in the token indicates that the authenticated user belongs to the "Manager" group. This can be used for implementing role-based access control within your application. For example, endpoints that require managerial permissions can validate that the token includes this group claim before granting access.

```sh
"sub": "b0ece98c-c071-7029-7b1d-cd61854cf522",
"cognito:groups": [
"Manager"
],
"iss": "https://cognito-idp.eu-north-1.amazonaws.com/eu-north-1_P3EpEiwut",
"client_id": "68tmnu99ooeor1go32unal8iia",
"origin_jti": "7b6e9f92-d5a6-4c1d-8d79-4f39090c2044",
"event_id": "75ea7297-2564-4d2e-b759-4cf01bc3029c",
"token_use": "access",
"scope": "aws.cognito.signin.user.admin",
"auth_time": 1741843027,
"exp": 1741846627,
"iat": 1741843027,
"jti": "bb35cce3-62a8-4a80-b99f-1454b9c6d643",
"username": "manager"
}
```
---

