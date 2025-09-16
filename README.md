# UpShop API

This is the backend API for the **UpShop** application. It provides product data and supports endpoints used by the frontend.

---

## 🚀 Getting Started

Follow these steps to set up and run the API locally.

### 1. Clone the Repository

```bash
git clone https://github.com/LunganiNjilo/UpShopApi.git
cd UpShopApi
```
### 2. Set Up the API in Docker

Build and start the API using Docker Compose:

```bash
docker compose build
docker compose up
```
After this, the API will be running at: http://localhost:5000

### 3. Test API Endpoints

Check that the API is working by visiting these endpoints in a browser or using Postman:

- Get all products:
```bash
http://localhost:5000/api/products
```
- Get a single product by SKU:
  
```bash
http://localhost:5000/api/products/SKU-0001
```

### 🧪 Running Tests
The test project is located at:
```text
..\UpShopApi\UpShopTests
```
To run the tests from the terminal:

```bash
cd ..\UpShopApi\UpShopTests
dotnet test
```

### 4.Database Information

The API uses SQLite, so no additional database setup is required. The database file is included in the project:

```bash
..\UpShopApi\UpShopApi\Infrastructure\Sql
```

### 5. Example API Response
Here’s an example response for the endpoint /api/products/SKU-0001:

```json
{
  "id": "b1a7d2f0-0001-4b10-9aee-123456789001",
  "sku": "SKU-0001",
  "name": "Classic White T-Shirt",
  "price": 199.99,
  "availableQuantity": 50,
  "imageUrl": "/images/products/SKU-0001.webp",
  "featured": true,
  "onSpecial": false
}
```

### 6.Notes
- Ensure Docker is installed and running before starting the API.
- Architecture is Clean: Controller → Services → Infrastructure, keeping business logic separate from infrastructure and endpoints.
