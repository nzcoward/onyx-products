# API Manual Testing

## Use Swagger
The port is fixed at 62926 in the docket compose. To access the swagger UI go to:
https://localhost:62926/swagger/index.html

### Requests
If you want to query any endpoint other than auth, you will need to include a token. The OpenApi doc is configured to use a user-supplied bearer token for /products requests.

#### Token Request
Make a call to the auth endpoint to get a bearer token. The request is a POST to the /auth endpoint with the following body:
```json
{
  "username": "testuser",
  "password": "password123"
}
```
The username and password are hardcoded in the codebase.

![Authorize Request](https://github.com/nzcoward/onyx-products/blob/main/docs/images/auth-request.png?raw=true)

##### Response

The response will contains a bearer token that you can apply to the Swagger UI authenticate your requests.

```json
{
  "token": "YOUR TOKEN HERE - COPY THIS"
}
```

![Authorize Response](https://github.com/nzcoward/onyx-products/blob/main/docs/images/auth-response.png?raw=true)

#### Products Requests
In order to use the /products endpoints, you need to tell swagger UI to use the token from the previous response.

You add it in the top right using the Authorize button.

![Swagger Authorization](https://github.com/nzcoward/onyx-products/blob/main/docs/images/swagger-authorize.png?raw=true)

This will open a dialog where you can paste the token you received from the auth endpoint.

![Swagger Bearer](https://github.com/nzcoward/onyx-products/blob/main/docs/images/swagger-bearer.png?raw=true)