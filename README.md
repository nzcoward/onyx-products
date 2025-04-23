# Onyx Products

## Yagni
I'm a big advocate of the YAGNI principle (You Aren't Gonna Need It). I believe in keeping things simple and only adding features when they are actually needed. This approach helps to avoid unnecessary complexity and keeps the codebase clean and maintainable.

The problem scope is very limited, so wary to add features that are not strictly necessary.

Build for evolution, not for perfection. Start with a simple solution and iterate on it as needed. 

I believe in focusing on the core functionality and delivering a product that meets the immediate needs of the users. The result is a shorter feedback loop, more engaged customers, and just better quality software overall.


## Still...


## Considerations

### a) DTOS
The api is the only place we consider DTOs (in the form of Requests/Responses). We don't need to pass DTOs through layers internally - it is just extra overhead.

### b) Encapsulation
Requests own their own validation (we could use a rule engine to inject these still) and domain objects own any mutations. This codebase does not have any of that really...

### c) Minimal APIs
Unlike controllers where we might consider constructor injection, method injection in minimal APIs makes it clear the scope of the dependencies when passing into the function.

### d) Standardisation
Using a response DTO means we can give meaning to our responses upon which clients can take action. It might be fine to consider a poco style response, but using e.g. JSON:API will help to inform the caller what they can (or can not) do.

### e)
