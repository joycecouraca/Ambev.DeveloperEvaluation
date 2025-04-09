# DeveloperStore - Sales API

This repository contains the implementation of a sales API for the DeveloperStore system. The API manages sales records, providing information about sale numbers, sale dates, customers, products, discounts, cancellation status, and more.

## Table of Contents

1. [Project Description](#project-description)
2. [Technologies Used](#technologies-used)
3. [Installation and Setup](#installation-and-setup)
4. [Project Structure](#project-structure)
5. [API Features](#api-features)
6. [Business Rules](#business-rules)
7. [Event Publishing](#event-publishing)
8. [Testing](#testing)
9. [Usage Example](#usage-example)
10. [Products API](#products-api)
11. [Swagger Documentation](#swagger-documentation)
12. [Product Categories](#product-categories)

## Project Description

The DeveloperStore API manages sales records, offering features to:

- Create sales.
- Update sale information.
- Cancel sales.
- Manage sale items (add, remove, update quantities).
- Apply discounts based on the number of items purchased.
- Cancel and delete sale items.
- Publish domain events related to sales, such as `SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled`.

## Technologies Used

- **.NET 8.0**: Main framework for building the API.
- **Entity Framework Core 8.0**: ORM for database communication with PostgreSQL.
- **AutoMapper**: For object-to-object mapping between DTOs and entities.
- **FluentAssertions**: Library to simplify assertions in tests.
- **xUnit**: Unit testing framework.
- **MediatR**: A library for handling requests and responses with the mediator pattern.
- **Serilog (optional)**: For logging purposes.
- **PostgreSQL**: Database used to store the sales data.
- **InMemory Database**: Used for integration testing, using Entity Framework Core's InMemory provider.

## Installation and Setup

### Prerequisites

Before running the project, you will need the following tools installed:

- **.NET SDK 8.0 or higher**: To build and run the application.
- **PostgreSQL**: Database system used in this project.
- **Postman** or **Insomnia** (for testing the API).

### Steps to Run the Project:

1. **Clone the repository:**

   ```bash
   git clone https://github.com/your-username/developerstore.git
   ```

2. **Install dependencies:**
   In the project directory, run the following command:

   ```bash
   dotnet restore
   ```

3. **Configure the database:**
   Ensure that your PostgreSQL connection string is configured in the `appsettings.json`:

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=DeveloperStoreDb;Username=your_user;Password=your_password;"
   }
   ```

4. **Apply database migrations:**
   After configuring the connection string, run the migrations:

   ```bash
   dotnet ef database update
   ```

5. **Run the project:**
   Now, simply run the project:

   ```bash
   dotnet run
   ```

6. **Testing the API:**

   - Access the API via your browser or use Postman to make requests.

## Project Structure

- **`/Controllers`**: API controllers containing the sale-related endpoints.
- **`/Domain`**: Contains domain entities like `Sale`, `SaleItem`, `Product`.
- **`/Application`**: Application layer with commands and handlers related to creating, updating, and canceling sales.
- **`/Infrastructure`**: Contains implementations for repositories and external dependencies like the database.
- **`/Shared`**: Contains shared classes and interfaces, such as `IUnitOfWork`, `IDomainEvent`, `BaseEntity`.
- **`/Tests`**: Contains unit and integration tests for the application functionality, using **InMemory** Entity Framework for integration tests.

## API Features

### Main Endpoints:

- **POST /api/sales**: Create a new sale.
- **PUT /api/sales/{id}**: Update an existing sale.
- **POST /api/sales/{id}/cancel**: Cancel a sale.
- **POST /api/sales/{id}/items**: Add items to a sale.
- **DELETE /api/sales/{id}/items**: Remove or cancel items from a sale.

### Business Rules:

- **Quantity-based Discounts**:

  - Purchases of 4 or more identical items: 10% discount.
  - Purchases between 10 and 20 identical items: 20% discount.
  - It is not possible to sell more than 20 identical items.
  - Purchases below 4 items do not have any discount.

- **Sale Cancellation**:

  - Once a sale is canceled, it cannot be edited anymore.

- **Item Cancellation**:

  - Items can be canceled individually within a sale.

## Products API

The Products API allows you to manage products in the system. Products can be created, updated, and deleted.

### Endpoints for Products:

- **GET /api/products**: Retrieve a list of all products.
- **GET /api/products/{id}**: Get details of a specific product by ID.
- **POST /api/products**: Create a new product.
- **PUT /api/products/{id}**: Update an existing product.
- **DELETE /api/products/{id}**: Delete a product.

### Example: Create a Product

**Request:**

```http
POST /api/products
Content-Type: application/json

{
    "name": "Beer",
    "description": "Cold and refreshing",
    "price": 10.99,
    "quantity": 100,
    "category": "Beverage"
}
```

**Response:**

```
{ "id": "product-guid", "name": "Beer", "description": "Cold and refreshing", "price": 10.99, "quantity": 100, "category": "Beverage" }
```

## Product Categories

When creating or updating products, you can assign a category from the following list:

- **Others**
- **Electronics**
- **Clothing**
- **Home Appliances**
- **Books**
- **Toys**
- **Groceries**
- **Furniture**
- **Beauty Products**
- **Sports Equipment**
- **Automotive**

These categories are represented as strings in the system but are mapped to enum values for better clarity.

For example, when creating a product in the **Electronics** category, the `category` field will be `"Electronics"`. The underlying enum will map this to the value `1`.

## Swagger Documentation

The API is fully documented using **Swagger**. To view and test the API endpoints interactively, follow these steps:

1. Run the application.

2. Open your browser and navigate to the Swagger UI at:

   ```
   http://localhost:5000/swagger
   ```

3. This will show a comprehensive list of all the available API endpoints, and you can test them directly from the browser.

## Event Publishing

When significant actions occur, such as the creation, modification, or cancellation of a sale, domain events are triggered. These events are processed and can be used to trigger other services or log actions.

- **SaleCreatedDomainEvent**: Triggered when a sale is created.
- **SaleModifiedDomainEvent**: Triggered when a sale is modified.
- **SaleCancelledDomainEvent**: Triggered when a sale is canceled.
- **ItemCancelledDomainEvent**: Triggered when an item in a sale is canceled.

Example event log:

```
Publish SaleCreatedDomainEvent - Sale created with ID: 12345
```

## Testing

The project includes unit tests for business logic and integration tests to ensure that the API functions correctly. Integration tests use **InMemory Entity Framework Core database** for testing purposes.

### How to Run Tests:

1. **Restore dependencies:**

   ```
   dotnet restore
   ```



1. **Run the tests:**

   ```
   dotnet test
   ```



Tests are located in the `Tests` directory.

## Usage Example

### Create a Sale:

**Request:**

```
POST /api/sales Content-Type: application/json { "customerId": "customer-guid", "creatorId": "creator-guid", "soldAt": "2025-04-09T14:00:00Z", "branchName": "Branch 1" }
```



**Response:**

```
{ "id": "sale-guid", "saleNumber": 1234567890123, "status": "Created", "createdAt": "2025-04-09T14:00:00Z" }
```

##
