# Order Endpoints Guide

## Overview
This guide describes all the order endpoints available in the DeliverWholesale API. The API uses JWT Bearer token authentication and implements role-based access control for admins and users.

**Base URL:** `/api/orders`  
**Authentication:** JWT Bearer Token (required for all endpoints except where noted)  
**Dates Format:** ISO 8601 (e.g., `2026-05-18T12:00:00Z`)

---

## Enum Values

### StatutOrder (Order Status)
- `EnAttente` - Pending (initial status)
- `Confirmee` - Confirmed
- `Livree` - Delivered
- `Annulee` - Cancelled

---

## Endpoints

### 1. Create Order
**POST** `/api/orders`

Creates a new order from the authenticated user's cart.

**Authentication:** Required (Any authenticated user)  
**Authorization:** Users can create orders for themselves  
**Role Required:** None (all authenticated users)

**Request Headers:**
```
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "articles": [
    {
      "produitId": 1,
      "quantite": 5,
      "prixUnitaire": 100.50
    }
  ],
  "adresseLivraison": "123 Rue Main, City, Country",
  "fraisLivraison": 50.00
}
```

**Success Response (200 OK):**
```json
{
  "message": "Commande créée avec succès",
  "orderId": 123
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "Erreur lors de la création de la commande",
  "error": "Details of the error"
}
```

**Notes:**
- User ID is automatically extracted from JWT token
- Order date is automatically set to current UTC time
- Initial status is always `EnAttente`
- Articles are validated before creation

---

### 2. Get All Orders (Admin Only)
**GET** `/api/orders`

Retrieves all orders in the system. **Admin only.**

**Authentication:** Required (JWT Bearer Token)  
**Authorization:** **Admin only** - `[Authorize(Roles = "Admin")]`  
**Role Required:** `Admin`

**Request Headers:**
```
Authorization: Bearer <admin_token>
```

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "userId": 5,
    "dateCommande": "2026-05-18T10:30:00Z",
    "totalProduits": 500.00,
    "fraisLivraison": 50.00,
    "totalFinal": 550.00,
    "statut": "EnAttente",
    "orderDetails": [
      {
        "id": 1,
        "orderId": 1,
        "produitId": 10,
        "quantite": 5,
        "prixUnitaire": 100.00,
        "produit": {
          "id": 10,
          "nom": "Product Name",
          "description": "Product description"
        }
      }
    ],
    "delivery": null
  }
]
```

**Error Response (403 Forbidden):**
If user is not an admin:
```json
{
  "message": "Access denied"
}
```

**Notes:**
- Returns **ALL orders** in the system (not filtered by user)
- Only accessible to admins
- Includes order details and delivery information
- Users cannot access this endpoint

---

### 3. Get User's Own Orders
**GET** `/api/orders`

Retrieves all orders for the authenticated user.

**Authentication:** Required (JWT Bearer Token)  
**Authorization:** None - All authenticated users can use this  
**Role Required:** None

**Request Headers:**
```
Authorization: Bearer <user_token>
```

**Success Response (200 OK):**
```json
[
  {
    "id": 123,
    "userId": 5,
    "dateCommande": "2026-05-18T10:30:00Z",
    "totalProduits": 500.00,
    "fraisLivraison": 50.00,
    "totalFinal": 550.00,
    "statut": "EnAttente",
    "orderDetails": [
      {
        "id": 1,
        "orderId": 123,
        "produitId": 10,
        "quantite": 5,
        "prixUnitaire": 100.00,
        "produit": {
          "id": 10,
          "nom": "Product Name",
          "description": "Product description"
        }
      }
    ],
    "delivery": {
      "id": 1,
      "orderId": 123,
      "adresse": "123 Rue Main, City, Country",
      "statut": "EnAttente",
      "dateCreation": "2026-05-18T10:30:00Z"
    }
  }
]
```

**Error Response (401 Unauthorized):**
```json
{
  "message": "Utilisateur non authentifié."
}
```

**Notes:**
- Returns **only the authenticated user's orders**
- Automatically filtered by user ID from JWT token
- Includes order details and delivery info
- Cannot see other users' orders

---

### 4. Get Order By ID
**GET** `/api/orders/{id}`

Retrieves a specific order by ID.

**Authentication:** Required (JWT Bearer Token)  
**Authorization:** Users see their own orders; admins can see all orders  
**Role Required:** None

**Request Parameters:**
- `id` (integer, required): Order ID

**Request Headers:**
```
Authorization: Bearer <token>
```

**Example Request:**
```
GET /api/orders/123
Authorization: Bearer <token>
```

**Success Response (200 OK):**
```json
{
  "id": 123,
  "userId": 5,
  "dateCommande": "2026-05-18T10:30:00Z",
  "totalProduits": 500.00,
  "fraisLivraison": 50.00,
  "totalFinal": 550.00,
  "statut": "EnAttente",
  "orderDetails": [
    {
      "id": 1,
      "orderId": 123,
      "produitId": 10,
      "quantite": 5,
      "prixUnitaire": 100.00,
      "produit": {
        "id": 10,
        "nom": "Product Name",
        "description": "Product description"
      }
    }
  ],
  "delivery": {
    "id": 1,
    "orderId": 123,
    "adresse": "123 Rue Main, City, Country",
    "statut": "EnAttente",
    "dateCreation": "2026-05-18T10:30:00Z"
  }
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "Commande introuvable"
}
```

**Notes:**
- Regular users can only view their own orders
- Admins can view any order
- Returns order details and delivery information
- If user tries to access another user's order, returns 404

---

### 5. Update Order Status (Admin Only)
**PUT** `/api/orders/{id}/status`

Updates the status of an order. **Admin only.**

**Authentication:** Required (JWT Bearer Token)  
**Authorization:** **Admin only** - `[Authorize(Roles = "Admin")]`  
**Role Required:** `Admin`

**Request Parameters:**
- `id` (integer, required): Order ID

**Request Headers:**
```
Authorization: Bearer <admin_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "statut": "Confirmee"
}
```

**Valid Status Values:**
- `EnAttente` - Pending
- `Confirmee` - Confirmed
- `Livree` - Delivered
- `Annulee` - Cancelled

**Success Response (200 OK):**
```json
{
  "message": "Statut mis à jour"
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "Commande introuvable"
}
```

**Error Response (403 Forbidden):**
If user is not an admin:
```json
{
  "message": "Access denied"
}
```

**Error Response (400 Bad Request):**
If invalid status provided:
```json
{
  "message": "Statut invalide"
}
```

**Example Usage:**
```bash
curl -X PUT http://localhost:5000/api/orders/123/status \
  -H "Authorization: Bearer <admin_token>" \
  -H "Content-Type: application/json" \
  -d '{"statut": "Confirmee"}'
```

**Notes:**
- Only admins can update order status
- Regular users cannot update order status (returns 403)
- Invalid status values throw an error
- Order must exist (returns 404 if not found)

---

### 6. Delete Order
**DELETE** `/api/orders/{id}`

Deletes/cancels an order.

**Authentication:** Required (JWT Bearer Token)  
**Authorization:** Users can delete their own orders; admins can delete any order  
**Role Required:** None

**Request Parameters:**
- `id` (integer, required): Order ID

**Request Headers:**
```
Authorization: Bearer <token>
```

**Success Response (200 OK):**
```json
{
  "message": "Commande supprimée"
}
```

**Error Response (404 Not Found):**
```json
{
  "message": "Commande introuvable"
}
```

**Example Usage:**
```bash
curl -X DELETE http://localhost:5000/api/orders/123 \
  -H "Authorization: Bearer <token>"
```

**Notes:**
- Users can delete their own orders
- Admins can delete any order
- Once deleted, the order cannot be recovered
- Attempting to delete a non-existent order returns 404

---

## Authentication & Authorization Summary

### User Roles
| Endpoint | Admin | User | Public |
|----------|-------|------|--------|
| `POST /orders` | ✅ Yes | ✅ Yes | ❌ No |
| `GET /orders` | ✅ All orders | ✅ Own orders | ❌ No |
| `GET /orders/{id}` | ✅ Any order | ✅ Own order | ❌ No |
| `PUT /orders/{id}/status` | ✅ Yes | ❌ No | ❌ No |
| `DELETE /orders/{id}` | ✅ Yes | ✅ Own order | ❌ No |

### Getting a JWT Token

To authenticate, you must first login:

**POST** `/api/auth/login`
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "Admin|Client",
  "email": "user@example.com",
  "fullName": "User Name"
}
```

Use the returned token in all subsequent requests:
```
Authorization: Bearer <token>
```

---

## Common Errors & Solutions

### 401 Unauthorized
**Cause:** Missing or invalid JWT token  
**Solution:** 
- Ensure you're sending `Authorization: Bearer <token>` header
- Verify the token hasn't expired
- Login again to get a fresh token

### 403 Forbidden
**Cause:** User doesn't have required role (e.g., trying to update status as a regular user)  
**Solution:**
- Only admins can call certain endpoints
- Check if your user has the `Admin` role
- Contact system administrator if needed

### 404 Not Found
**Cause:** Order doesn't exist or user doesn't have permission to view it  
**Solution:**
- Verify the order ID is correct
- Users can only see their own orders
- Admins can see all orders

### 400 Bad Request
**Cause:** Invalid input data or invalid status value  
**Solution:**
- Check request body format is correct JSON
- Ensure status is one of: `EnAttente`, `Confirmee`, `Livree`, `Annulade`
- Verify all required fields are present

---

## Usage Examples

### Example 1: User Creating an Order

1. **Login to get token:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "Client",
  "email": "user@example.com"
}
```

2. **Create an order:**
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "articles": [
      {
        "produitId": 1,
        "quantite": 5,
        "prixUnitaire": 100.50
      }
    ],
    "adresseLivraison": "123 Rue Main, City",
    "fraisLivraison": 50.00
  }'
```

3. **View their orders:**
```bash
curl -X GET http://localhost:5000/api/orders \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

---

### Example 2: Admin Managing Orders

1. **Login as admin:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "adminpass123"
  }'
```

2. **View all orders in system:**
```bash
curl -X GET http://localhost:5000/api/orders \
  -H "Authorization: Bearer <admin_token>"
```

3. **Update order status:**
```bash
curl -X PUT http://localhost:5000/api/orders/123/status \
  -H "Authorization: Bearer <admin_token>" \
  -H "Content-Type: application/json" \
  -d '{"statut": "Confirmee"}'
```

4. **View specific order:**
```bash
curl -X GET http://localhost:5000/api/orders/123 \
  -H "Authorization: Bearer <admin_token>"
```

---

## Data Models

### Order Object
```json
{
  "id": 123,
  "userId": 5,
  "dateCommande": "2026-05-18T10:30:00Z",
  "totalProduits": 500.00,
  "fraisLivraison": 50.00,
  "totalFinal": 550.00,
  "statut": "EnAttente",
  "orderDetails": [],
  "delivery": {}
}
```

### OrderCreateDto
```json
{
  "articles": [
    {
      "produitId": 1,
      "quantite": 5,
      "prixUnitaire": 100.50
    }
  ],
  "adresseLivraison": "Address",
  "fraisLivraison": 50.00
}
```

### UpdateOrderStatusDto
```json
{
  "statut": "Confirmee"
}
```

---

## Important Notes

1. **User Filtering:** When users retrieve orders, they automatically get filtered to see only their own orders
2. **Admin Access:** Admins can see all orders and manage them
3. **Status Updates:** Only admins can change order status
4. **Order Immutability:** Once created, order details cannot be modified; only status can change
5. **Dates:** All dates are in UTC ISO 8601 format
6. **Error Handling:** Always check response status codes and error messages
7. **Security:** Never expose tokens in logs or public places

---

## Support

For issues or questions about the Order API, please contact the development team.
