# Order Book Execution

This project reads multiple exchange order books and calculates the best execution plan to buy or sell a given amount of BTC.

It includes the following parts:

- Core library with the matching logic
- Console app
- Web API
- Unit tests and integration tests

---



## How it works

1. The repository loads order books from the provided data file (one exchange per line).
2. The service aggregates all bids or asks from all exchanges.
3. For **buy** orders:
    - It takes the lowest asks first (cheapest prices).
4. For **sell** orders:
    - It takes the highest bids first (best prices).
5. It keeps filling the requested amount until:
    - the amount is fully filled
    - there is no more liquidity
    - there is no more balance on the exchanges

The result is an execution plan: a list of actions with exchange ID, price, amount, and the order type.  
The result also contains the information if the order was fully filled or not. If there isn’t enough liquidity or balance on the exchanges, the result contains the remaining unfilled amount.

---

## Assumptions

- The first value on each line of the data file (before the tab) is used as the `ExchangeId`.  
  It does looks like a UNIX timestamp, but the format wasn’t documented, so it is treated as a simple identifier.
- The instructions mention EUR/BTC balances, but no balance data is provided in the file.  
  Because of that, the current algorithm implementation does enforce balance constraints, but the balances are hardcoded to 1BTC and 10.000EUR per exchange.

These assumptions are based on the provided data and missing details in the specification.

---

## Prerequisites

- .NET SDK 9
- Docker
- Order books data file located in `data/order_books_data`. It's included in the git repository by default. 

## Running the console app

From the repository root:

```bash
dotnet run --project OrderBookExecution.Console -- buy 2.5
```

or 

```bash
dotnet run --project OrderBookExecution.Console -- sell 2.5
```

The console will print the execution actions.

## Running the Web API
From the Repository root: 

```bash
dotnet run --project OrderBookExecution.Api
```

By default, the API exposes:

Scalar UI: http://localhost:5298/scalar

OpenAPI JSON: http://localhost:5298/openapi/v1.json

Example request (POST /api/OrderBookExecution):

```json
{
  "orderType": "Buy",
  "amount": 1.5
}
```

The response is a list of execution actions with exchange ID, price, amount and order side. If the order could not be fully fulfilled, we indicate that via the `isFullyFilled` flag and return the unfilled amount.  

```json
{
  "executionActions": [
    {
      "orderType": "Buy",
      "exchangeId": "1548763106.9417",
      "amount": 0.01,
      "price": 2955.03
    }
  ],
  "isFullyFilled": true,
  "remainingAmountToFill": null
}
```
### Running the API through Docker

From the Repository root: 

```bash
docker compose up
```

This will build and run the docker image. 

The API will be exposed on the port :8080. 

Source data for order books will be pulled and integrated into the image from the same source as regular application build (`data/order_books_data`).

## Live Demo - AWS Deployment

The API is deployed to AWS with the following infrastructure:

- VPC with public/private subnet architecture
- Application Load Balancer for high availability
- Containerized deployment

**Live API endpoint:** `http://alb-orderbook-api-1491217945.eu-north-1.elb.amazonaws.com/`

API documentation: `http://alb-orderbook-api-1491217945.eu-north-1.elb.amazonaws.com/scalar/`

## Running tests
From the repository root:
```bash
dotnet test
```

The solution includes:

- Unit tests for the core execution logic
- Tests for the repository (file parsing / error handling)
- Integration tests for the API, using WebApplicationFactory<Program>