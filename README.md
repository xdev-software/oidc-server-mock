# OpenId Connect Server Mock - XDEV Edition

> [!NOTE]
> This is a fork of [Soluto/oidc-server-mock](https://github.com/Soluto/oidc-server-mock)

This project allows you to run configurable mock server with OpenId Connect functionality.

> [!IMPORTANT]
> Free for development, testing and personal projects.<br/>
> For production you need to purchase [Duende IdentityServer license](https://duendesoftware.com/products/identityserver).

## Simple Configuration

Use the following to pull the image:

```bash
docker pull xdevsoftware/oidc-server-mock:latest
```

This is the sample of using the server in `docker-compose` configuration:

```yaml
version: '3'
services:
  oidc-server-mock:
    container_name: oidc-server-mock
    image: xdevsoftware/oidc-server-mock:latest
    ports:
      - '4011:80'
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      SERVER_OPTIONS_INLINE: |
        {
          "AccessTokenJwtType": "JWT",
          "Discovery": {
            "ShowKeySet": true
          },
          "Authentication": {
            "CookieSameSiteMode": "Lax",
            "CheckSessionCookieSameSiteMode": "Lax"
          }
        }
      LOGIN_OPTIONS_INLINE: |
        {
          "AllowRememberLogin": false
        }
      LOGOUT_OPTIONS_INLINE: |
        {
          "AutomaticRedirectAfterSignOut": true
        }
      API_SCOPES_INLINE: |
        - Name: some-app-scope-1
        - Name: some-app-scope-2
      API_RESOURCES_INLINE: |
        - Name: some-app
          Scopes:
            - some-app-scope-1
            - some-app-scope-2
      USERS_CONFIGURATION_INLINE: |
        [
          {
            "SubjectId":"1",
            "Username":"User1",
            "Password":"pwd",
            "Claims": [
              {
                "Type": "name",
                "Value": "Sam Tailor",
                "ValueType": "string"
              },
              {
                "Type": "email",
                "Value": "sam.tailor@gmail.com",
                "ValueType": "string"
              },
              {
                "Type": "some-api-resource-claim",
                "Value": "Sam's Api Resource Custom Claim",
                "ValueType": "string"
              },
              {
                "Type": "some-api-scope-claim",
                "Value": "Sam's Api Scope Custom Claim",
                "ValueType": "string"
              },
              {
                "Type": "some-identity-resource-claim",
                "Value": "Sam's Identity Resource Custom Claim",
                "ValueType": "string"
              }
            ]
          }
        ]
      CLIENTS_CONFIGURATION_PATH: /tmp/config/clients-config.json
      ASPNET_SERVICES_OPTIONS_INLINE: |
        { 
          "ForwardedHeadersOptions": { 
            "ForwardedHeaders" : "All"
          }
        }
    volumes:
      - .:/tmp/config:ro
```

When `clients-config.json` is as following:

```json
[
  {
    "ClientId": "implicit-mock-client",
    "Description": "Client for implicit flow",
    "AllowedGrantTypes": ["implicit"],
    "AllowAccessTokensViaBrowser": true,
    "RedirectUris": ["http://localhost:3000/auth/oidc", "http://localhost:4004/auth/oidc"],
    "AllowedScopes": ["openid", "profile", "email"],
    "IdentityTokenLifetime": 3600,
    "AccessTokenLifetime": 3600
  },
  {
    "ClientId": "client-credentials-mock-client",
    "ClientSecrets": ["client-credentials-mock-client-secret"],
    "Description": "Client for client credentials flow",
    "AllowedGrantTypes": ["client_credentials"],
    "AllowedScopes": ["some-app-scope-1"],
    "ClientClaimsPrefix": "",
    "Claims": [
      {
        "Type": "string_claim",
        "Value": "string_claim_value",
        "ValueType": "string"
      },
      {
        "Type": "json_claim",
        "Value": "[\"value1\", \"value2\"]",
        "ValueType": "json"
      }
    ]
  }
]
```

This is the sample of using the server in `Dockerfile` configuration:

```
# Use the base image
FROM xdevsoftware/oidc-server-mock:0.8.6

# Set environment variables
# additional configuration can be found in the readme
# https://github.com/Soluto/oidc-server-mock/blob/master/README.md?plain=1#L145
ENV ASPNETCORE_ENVIRONMENT=Development
ENV SERVER_OPTIONS_INLINE="{ \
  \"AccessTokenJwtType\": \"JWT\", \
  \"Discovery\": { \
    \"ShowKeySet\": true \
  }, \
  \"Authentication\": { \
    \"CookieSameSiteMode\": \"Lax\", \
    \"CheckSessionCookieSameSiteMode\": \"Lax\" \
  } \
}"
ENV USERS_CONFIGURATION_INLINE="[ \
  { \
    \"SubjectId\": \"1\", \
    \"Username\": \"User1\", \
    \"Password\": \"pwd\", \
    \"Claims\": [ \
      { \
        \"Type\": \"name\", \
        \"Value\": \"Sam Tailor\", \
        \"ValueType\": \"string\" \
      }, \
      { \
        \"Type\": \"email\", \
        \"Value\": \"sam.tailor@gmail.com\", \
        \"ValueType\": \"string\" \
      }, \
      { \
        \"Type\": \"some-api-resource-claim\", \
        \"Value\": \"Sam's Api Resource Custom Claim\", \
        \"ValueType\": \"string\" \
      }, \
      { \
        \"Type\": \"some-api-scope-claim\", \
        \"Value\": \"Sam's Api Scope Custom Claim\", \
        \"ValueType\": \"string\" \
      }, \
      { \
        \"Type\": \"some-identity-resource-claim\", \
        \"Value\": \"Sam's Identity Resource Custom Claim\", \
        \"ValueType\": \"string\" \
      } \
    ] \
  } \
]"
ENV CLIENTS_CONFIGURATION_INLINE="[ \
  { \
    \"ClientId\": \"some-client-di\", \
    \"ClientSecrets\": [\"some-client-Secret\"], \
    \"Description\": \"Client for authorization code flow\", \
    \"AllowedGrantTypes\": [\"authorization_code\"], \
    \"RequirePkce\": false, \
    \"AllowAccessTokensViaBrowser\": true, \
    \"RedirectUris\": [\"http://some-callback-url"], \
    \"AllowedScopes\": [\"openid\", \"profile\", \"email\"], \
    \"IdentityTokenLifetime\": 3600, \
    \"AccessTokenLifetime\": 3600, \
    \"RequireClientSecret\": false \
  } \
]"
ENV ASPNET_SERVICES_OPTIONS_INLINE="{ \
  \"ForwardedHeadersOptions\": { \
    \"ForwardedHeaders\": \"All\" \
  } \
}"

# Expose the port
EXPOSE 80

# Command to run the application
CMD ["dotnet", "Soluto.OidcServerMock.dll"]
```

Clients configuration should be provided. Test user configuration is optional (used for implicit flow only).

There are two ways to provide configuration for supported scopes, clients and users. You can either provide it inline as environment variable:

- `SERVER_OPTIONS_INLINE`
- `LOGIN_OPTIONS_INLINE`
- `LOGOUT_OPTIONS_INLINE`
- `API_SCOPES_INLINE`
- `USERS_CONFIGURATION_INLINE`
- `CLIENTS_CONFIGURATION_INLINE`
- `API_RESOURCES_INLINE`
- `IDENTITY_RESOURCES_INLINE`

  or mount volume and provide the path to configuration json as environment variable:

- `SERVER_OPTIONS_PATH`
- `LOGIN_OPTIONS_PATH`
- `LOGOUT_OPTIONS_PATH`
- `API_SCOPES_PATH`
- `USERS_CONFIGURATION_PATH`
- `CLIENTS_CONFIGURATION_PATH`
- `API_RESOURCES_PATH`
- `IDENTITY_RESOURCES_PATH`

The configuration format can be Yaml or JSON both for inline or file path options.

In order to be able to override standard identity resources set `OVERRIDE_STANDARD_IDENTITY_RESOURCES` env var to `True`.

## Base path

The server can be configured to run with base path. So all the server endpoints will be also available with some prefix segment.
For example `http://localhost:8080/my-base-path/.well-known/openid-configuration` and `http://localhost:8080/my-base-path/connect/token`.
Just set `BasePath` property in `ASPNET_SERVICES_OPTIONS_INLINE/PATH` env var.

## Custom endpoints

### User management

Users can be added (in future also removed and altered) via `user management` endpoint.

- Create new user: `POST` request to `/api/v1/user` path.
  The request body should be the `User` object. Just as in `USERS_CONFIGURATION`.
  The response is subjectId as sent in request.

- Get user: `GET` request to `/api/v1/user/{subjectId}` path.
  The response is `User` object

- Update user `PUT` request to `/api/v1/user` path. (**Not implemented yet**)
  The request body should be the `User` object. Just as in `USERS_CONFIGURATION`.
  The response is subjectId as sent in request.

  > If user doesn't exits it will be created.

- Delete user: `DELETE` request to `/api/v1/user/{subjectId}` path. (**Not implemented yet**)
  The response is `User` object

## HTTPS

To use `https` protocol with the server just add the following environment variables to the `docker run`/`docker-compose up` command, expose ports and mount volume containing the pfx file:

```yaml
environment:
  ASPNETCORE_URLS: https://+:443;http://+:80
  ASPNETCORE_Kestrel__Certificates__Default__Password: <password for pfx file>
  ASPNETCORE_Kestrel__Certificates__Default__Path: /path/to/pfx/file
volumes:
  - ./local/path/to/pfx/file:/path/to/pfx/file:ro
ports:
  - 8080:80
  - 8443:443
```

---

## Cookie SameSite mode

Since Aug 2020 Chrome has a new [secure-by-default model](https://blog.chromium.org/2019/10/developers-get-ready-for-new.html) for cookies, enabled by a new cookie classification system. Other browsers will join in near future.

There are two ways to use `oidc-server-mock` with this change.

1. Run the container with HTTPS enabled (see above).
2. Change cookies `SameSite` mode from default `None` to `Lax`. To do so just add the following to `SERVER_OPTIONS_INLINE` (or the file at `SERVER_OPTIONS_PATH`):

```javascript
{
  // Existing configuration
  // ...
  "Authentication": {
    "CookieSameSiteMode": "Lax",
    "CheckSessionCookieSameSiteMode": "Lax"
  }
}
```
