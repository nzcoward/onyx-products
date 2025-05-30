# Onyx Products Api

## Yagni
I'm a big advocate of the YAGNI principle (You Aren't Gonna Need It). I believe in keeping things simple and only adding features when they are actually needed. This approach helps to avoid unnecessary complexity and keeps the codebase clean and maintainable.

The problem scope is very limited, so wary to add features that are not strictly necessary.

## Still...
Let's add some supporting features such as hosting the API and the DB in containers.
Might as well show how we might consider setting up a more comprehsensive service environment (with a few shortcuts for the purposes of this exercise).

## API Manual Testing
Basic manual testing instructions are in the docs folder [Testing Documents](https://github.com/nzcoward/onyx-products/tree/main/docs)

## Automated Testing
There are 2 test projects.
1. Unit tests to cover some simple units
2. DB 'Integration' tests - ef core in-memory was used for brevity, but things are almost set up to spin up ephemerial environments

And...
3. .http files for (Manual) API testing - this is a full-fat hosted service, but again, we could change up some config to make it more ephermerial. There is no test harness around these, but would continue to investigate.

The unit tests require testing preview features enabled if you are running < VS2022 17.13
[Testing in VS2022](https://github.com/thomhurst/TUnit/tree/150339d8b8954928e4019562895062203b1248af#visual-studio)

## Call-outs

### a) DTOS
The api is the only place we consider DTOs (in the form of Requests/Responses). We don't need to pass DTOs through layers internally - it is just extra overhead.

### b) Encapsulation
Requests own their own validation (we could use a rule engine to inject these still) and domain objects own any mutations. This codebase does not have any of that really...
Domain models should maintain their own state and only provide the appropriate external methods that allow callers to make only *valid* changes. This project still uses an IProductsService, but I'd likely flesh it out further by encapsulating into the model that is also used by EF Core.

### c) Minimal APIs
Unlike controllers where we might consider constructor injection, method injection in minimal APIs makes it clear the scope of the dependencies when passing into the function.
Minimal APIs (in the form I have configured them) are pretty wordy. I do tend to prefer extension methods, but there can be a lot of them when configured manually like this. Generally consider custom extensions that set sensible defaults.

### d) Standardisation
Using a response DTO means we can give meaning to our responses upon which clients can take action. It might be fine to consider a poco style response, but using e.g. JSON:API will help to inform the caller what they can (or can not) do.
For this example, I did not add JSON:API formattered responses, as it is really only adds value when we have a more hypermedia style API that allows actions/navigation that the model can define.

### e) What about {insert your mediatr/cqrs pattern here}?
Using commands/queries/handlers is fine, but consider maintenance vs. decoupling payoffs. It's a lot of abstraction and overhead; often not worth it.

### f) Repositories???
Proper encapsulation in the domain model reduces the need to consider repositories. Generally I would argue that repositories breed more anti-patterns than they resolve.
EFCore gives us an element of abstraction (can be one of a number of data providers). Repositories can result in mismanaged transaction scopes/atomicity, breaking SRP, odd mixes of BL and data, etc.

## Beyond the current state...
There is some tidying to do. Would want to review the Domain project and understand what belongs in there, or elsewhere (with the proviso that it should deliver what is needed *now* not to guess what is needed *later* vs. what is in Api (generally it feels ok, except for the extensions for the builder/app)

I added otel config with a console exporter, but don't specify spans, nor do I consider metrics, but this is something I'd be looking into with even a small expansion; observability early is really important.
I have also not used logging extensively, and need to consider how much is required vs tracing...




# Wider distributed system?
Funnily enough, the test calls for an API for querying and creating products; followed up with its place within a wider microservice architecture e.g. alongside an orders service.
I'd probably challenge the format *already*.

Should we really be designing an API that is likely used to fulfill queries for a customer-facing UI? More likely we have a supply-chain management-esque system where we are placing orders with them, completing those order, which is kicking off e.g. `ProductReceived` events, and those are the source data for our products service (not a POST).

This might be a fast, read-only service driving part of a composable UI or product search, but again, do we want to deliver 'products' or some other aggregated model that might deliver more value.

The order *process*, driven by user-interaction can have work moved async, to e.g. payments processing, jit supply or warehouse, notifications, shipping, etc. services. One might consider orchestration (with or without sagas) or choreography (with event chaining) but without a wider context, it feels like any *guesses* this early are too early.
