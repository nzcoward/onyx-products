# Onyx Products

## Yagni
I'm a big advocate of the YAGNI principle (You Aren't Gonna Need It). I believe in keeping things simple and only adding features when they are actually needed. This approach helps to avoid unnecessary complexity and keeps the codebase clean and maintainable.

The problem scope is very limited, so wary to add features that are not strictly necessary.

## Still...
Let's add some supporting features such as hosting the API and the DB in containers.

## API Manual Testing
Basic manual testing instructions are in the docs folder [Testing Documents](https://github.com/nzcoward/onyx-products/tree/main/docs)

## Features

### a) DTOS
The api is the only place we consider DTOs (in the form of Requests/Responses). We don't need to pass DTOs through layers internally - it is just extra overhead.

### b) Encapsulation
Requests own their own validation (we could use a rule engine to inject these still) and domain objects own any mutations. This codebase does not have any of that really...
Domain models should maintain their own state and only provide the appropriate external methods that allow callers to make only *valid* changes.

### c) Minimal APIs
Unlike controllers where we might consider constructor injection, method injection in minimal APIs makes it clear the scope of the dependencies when passing into the function.
Minimal APIs (in the form I have configured them) are pretty wordy. I do tend to prefer extension methods, but there can be a lot of them when configured manually like this. Generally consider custom extensions that set sensible defaults.

### d) Standardisation
Using a response DTO means we can give meaning to our responses upon which clients can take action. It might be fine to consider a poco style response, but using e.g. JSON:API will help to inform the caller what they can (or can not) do.
For this example, I did not add JSON:API formattered responses, as it is really only adds value when we have a more hypermedia style API that allows actions/navigation that the model can define.