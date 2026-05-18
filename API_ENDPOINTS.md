# DeliverWholesale API - Endpoints Reference

This document lists the HTTP endpoints provided by the DeliverWholesale backend, expected request payloads, authorization requirements, and typical responses.

## Basics

- Base path: /api
- Auth: JWT Bearer in the Authorization header: Bearer <token>
- Roles used: Admin, Client (where indicated)
- Date/time values: ISO 8601 strings
- JSON enum values are serialized as strings

---

## Auth (/api/auth)

### POST /api/auth/register
- Auth: Public
- Body (application/json): RegisterDto
  - FullName: string
  - Email: string
  - Password: string
- Returns: 200 OK
  - { message: string }

### GET /api/auth/confirm-email
- Auth: Public
- Query params:
  - email: string
  - token: string
- Returns: 200 OK
  - { message: string }

### POST /api/auth/login
- Auth: Public
- Body (application/json): LoginDto
  - Email: string
  - Password: string
- Returns: 200 OK
  - { token: string, role: string, email: string, fullName: string }
- Errors: 400/exception when credentials are invalid or email not confirmed

### POST /api/auth/logout
- Auth: Bearer
- Body: none
- Returns: 200 OK
  - { message: "Déconnexion réussie." }

---

## Products (/api/products)

### POST /api/products
- Auth: Public (no controller-level authorization)
- Content-Type: multipart/form-data
- Body (form): ProductCreateDto
  - libelle: string
  - Description: string
  - PrixVente: decimal
  - idCategorie: int
  - NbUnite: int
  - seuil: int
  - prixModifiable: bool
  - Image: file (optional)
- Returns: 200 OK
  - created product (handler result)

### GET /api/products
- Auth: Public
- Returns: 200 OK
  - list of products (handler result)

### GET /api/products/{id}
- Auth: Public
- Returns: 200 OK
  - product details (handler result)
- Errors: 404 Not Found if product does not exist

### PUT /api/products/{id}
- Auth: Public
- Content-Type: multipart/form-data
- Body (form): ProductUpdateDto
  - libelle: string?
  - Description: string?
  - NouveauPrixVente: decimal?
  - idCategorie: int?
  - seuil: int?
  - prixModifiable: bool?
  - Image: file?
- Returns: 200 OK (empty body)
- Errors: 404 Not Found if product does not exist

### DELETE /api/products/{id}
- Auth: Public
- Returns: 200 OK (empty body)
- Errors: 404 Not Found if product does not exist

### POST /api/products/{id}/prix
- Auth: Public
- Body (application/json): PrixVenteCreateDto
  - Valeur: decimal
- Returns: 200 OK
  - { PrixVenteId: int }
- Notes: idP is taken from the route and set by the controller

### GET /api/products/{id}/prix
- Auth: Public
- Returns: 200 OK
  - list of PrixVenteReadDto
    - Id: int
    - idP: int
    - Valeur: decimal
    - Date: DateTime

---

## Panier (/api/panier)

All endpoints require authentication.

### GET /api/panier
- Auth: Bearer
- Returns: 200 OK
  - PanierDto
    - UserId: int
    - TotalPrix: decimal
    - Items: list of PanierItemDto

### POST /api/panier/items
- Auth: Bearer
- Body (application/json): AddToPanierDto
  - ProduitId: int
  - Quantite: int
- Returns: 200 OK
  - PanierDto

### PUT /api/panier/items/{produitId}
- Auth: Bearer
- Body (application/json): UpdatePanierItemDto
  - Quantite: int
- Returns: 200 OK
  - PanierDto
- Notes: Quantite = 0 removes the item.

### DELETE /api/panier/items/{produitId}
- Auth: Bearer
- Returns: 200 OK
  - PanierDto

### DELETE /api/panier/clear
- Auth: Bearer
- Returns: 200 OK
  - PanierDto

### POST /api/panier/checkout
- Auth: Bearer
- Returns: 200 OK
  - { Message: string, OrderId: int }
- Notes: Creates an Order from panier items and clears the panier.

---

## Categories (/api/categories)

### GET /api/categories
- Auth: Public
- Returns: 200 OK
  - list of categories (handler result)

### POST /api/categories
- Auth: Bearer (Role: Admin)
- Body (application/json): CategoryDto
  - Nom: string
  - Description: string
  - ParentId: int?
- Returns: 200 OK
  - created category (handler result)

### PUT /api/categories/{id}
- Auth: Bearer (Role: Admin)
- Body (application/json): CategoryDto
- Returns: 204 No Content
- Errors: 404 Not Found with "Catégorie introuvable"

### DELETE /api/categories/{id}
- Auth: Bearer (Role: Admin)
- Returns: 204 No Content
- Errors: 404 Not Found with "Catégorie introuvable"

---

## Orders (/api/order)

All endpoints require authentication.

### POST /api/order
- Auth: Bearer
- Body (application/json): OrderCreateDto
  - Items: array of OrderItemDto
    - ProduitId: int
    - Quantite: int
- Returns: 200 OK
  - { Message: string, OrderId: int }
- Errors: 400 Bad Request with { Message, Error }

### GET /api/order
- Auth: Bearer
- Returns: 200 OK
  - list of orders (handler result)

### GET /api/order/{id}
- Auth: Bearer
- Returns: 200 OK
  - order details (handler result)
- Errors: 404 Not Found with "Commande introuvable"

### DELETE /api/order/{id}
- Auth: Bearer
- Returns: 200 OK
  - { Message: "Commande supprimée" }
- Errors: 404 Not Found with "Commande introuvable"

### PUT /api/order/{id}/status
- Auth: Bearer
- Body (application/json): UpdateOrderStatusDto
  - Statut: string
- Returns: 200 OK
  - { message: "Statut mis à jour" }
- Errors: 404 Not Found with "Commande introuvable"

---

## Deliveries (/api/delivery)

All endpoints require Admin role.

### POST /api/delivery
- Auth: Bearer (Role: Admin)
- Body (application/json): CreateDeliveryDto
  - OrderId: int
  - AdresseLivraison: string
  - DateLivraisonPrevue: DateTime
- Returns: 200 OK
  - created delivery (handler result)

### GET /api/delivery
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - list of deliveries (handler result)

### PUT /api/delivery/{id}/status
- Auth: Bearer (Role: Admin)
- Body (application/json): UpdateDeliveryStatusDto
  - Statut: DeliveryStatus
  - DateLivraisonReelle: DateTime?
- Returns: 200 OK
  - "Statut livraison mis à jour"
- Errors: 404 Not Found

### GET /api/delivery/today/products
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - list of products for today (handler result)

### GET /api/delivery/today/clients
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - list of clients for today (handler result)

### PUT /api/delivery/{id}/date
- Auth: Bearer (Role: Admin)
- Body (application/json): DateTime (raw JSON string or ISO date)
- Returns: 200 OK
  - "Date de livraison mise à jour"
- Errors: 404 Not Found

---

## Stock (/api/stock)

All endpoints require Admin role.

### POST /api/stock
- Auth: Bearer (Role: Admin)
- Body (application/json): AddStockLotDto
  - AchatLotId: int
  - Quantite: int
  - PrixAchatTotal: decimal
  - Fournisseur: string?
  - Unite: string?
- Returns: 200 OK
  - stock update result (handler result)

### GET /api/stock
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - list of StockDetailsDTO
    - StockLotId: List<int>
    - Product: Produit
    - QuantiteTotalRestante: decimal
    - Transations: List<Transaction>

### GET /api/stock/{produitId}
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - stock details for product (handler result)

---

## Reclamation (/api/reclamation)

### POST /api/reclamation
- Auth: Bearer (Role: Client)
- Body (application/json): CreateReclamationDto
  - OrderId: int
  - Sujet: string
  - Description: string
- Returns: 200 OK
  - created reclamation (handler result)

### GET /api/reclamation/mes-reclamations
- Auth: Bearer (Role: Client)
- Returns: 200 OK
  - list of reclamations for current user

### GET /api/reclamation
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - list of reclamations (includes Order and ResolvedByUser)

### PUT /api/reclamation/{id}/traiter
- Auth: Bearer (Role: Admin)
- Body (application/json): TraiterReclamationDto
  - Status: string
  - ReponseAdmin: string
- Returns: 200 OK
  - updated reclamation
- Notes: if Status == "Résolue", DateResolution and ResolvedByUserId are set

### DELETE /api/reclamation/{id}
- Auth: Bearer
- Returns: 200 OK
  - { message: "Réclamation supprimée avec succès" }

---

## Profile (/api/profile)

All endpoints require authentication.

### GET /api/profile
- Auth: Bearer
- Returns: 200 OK
  - { Id, Nom, Prenom, Email, Adresse, Role }
- Errors: 404 Not Found if user not found

### PUT /api/profile
- Auth: Bearer
- Body (application/json): UpdateProfileDto
  - Nom: string
  - Prenom: string
  - Email: string
  - Adresse: string?
- Returns: 200 OK
  - "Profile updated successfully"

### PUT /api/profile/change-password
- Auth: Bearer
- Body (application/json): ChangePasswordDto
  - OldPassword: string
  - NewPassword: string
- Returns: 200 OK
  - "Password updated successfully"
- Errors: 400 Bad Request with "Old password is incorrect"

### PUT /api/profile/update-delivery-address
- Auth: Bearer
- Body (application/json): UpdateDeliveryAddressDto
  - AdresseLivraisonActive: string
- Returns: 200 OK
  - "Delivery address updated successfully"

---

## Config (/api/config)

All endpoints require Admin role.

### GET /api/config
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - Config
    - Id: int
    - MontantMinimumCommande: decimal
    - ProfitPercentage: decimal
    - FraisLivraison: decimal
    - SeuilAlerteStockBas: int

### PUT /api/config
- Auth: Bearer (Role: Admin)
- Body (application/json): Config
  - Id: int
  - MontantMinimumCommande: decimal
  - ProfitPercentage: decimal
  - FraisLivraison: decimal
  - SeuilAlerteStockBas: int
- Returns: 204 No Content
- Errors: 404 Not Found with "Configuration introuvable"

---

## Dashboard (/api/dashboard)

All endpoints require Admin role.

### GET /api/dashboard/stats
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - stats payload (handler result)

### GET /api/dashboard/alertes
- Auth: Bearer (Role: Admin)
- Returns: 200 OK
  - alertes payload (handler result)

---

## AchatLot (/api/achatlot)

### POST /api/achatlot
- Auth: Public
- Body (application/json): CreateAchatLotCommand
  - ProduitId: int
  - QuantiteAchetee: int
  - PrixUnitaire: decimal
  - Fournisseur: string
  - SupplierId: int?
- Returns: 200 OK
  - { message: "Achat créé avec succès", AchatLotId: int }
- Notes: NumeroLot is generated server-side.

### GET /api/achatlot
- Auth: Public
- Returns: 200 OK
  - list of achat lots (handler result)

### GET /api/achatlot/{id}
- Auth: Public
- Returns: 200 OK
  - achat lot details (handler result)
- Errors: 404 Not Found with "Achat introuvable"

### DELETE /api/achatlot/{id}
- Auth: Public
- Returns: 200 OK
  - "Achat supprimé avec succès"
- Errors: 404 Not Found with "Achat introuvable"

---

## SignalR Hub

- Hub URL: /hubs/notifications
- Auth: Bearer via access_token query string (for SignalR)
- Hub methods:
  - JoinAdminGroup()
  - LeaveAdminGroup()

### GET /api/delivery/today/products
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns products scheduled for today's deliveries

### GET /api/delivery/today/clients
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns clients scheduled for today's deliveries

### PUT /api/delivery/{id}/date
- Auth: Bearer (Role: Admin)
- Body (DateTime) as JSON or raw (controller expects DateTime newDate in body)
- Success: 200 OK
  - { message: "Date de livraison mise à jour" }
- NotFound: 404 if delivery not found

---

## Profile
Base: /api/profile
Class: [Authorize]

### GET /api/profile
- Auth: Bearer
- Success: 200 OK
  - { Id, Nom, Prenom, Email, Adresse, Role }

### PUT /api/profile
- Auth: Bearer
- Body (application/json):
  - Nom: string
  - Prenom: string
  - Email: string
  - Adresse: string?
- Success: 200 OK
  - { message: "Profile updated successfully" }
- NotFound: 404 if user not found

### PUT /api/profile/change-password
- Auth: Bearer
- Body (application/json):
  - OldPassword: string
  - NewPassword: string
- Success: 200 OK
  - { message: "Password updated successfully" }
- Error: 400 if old password incorrect

### PUT /api/profile/update-delivery-address
- Auth: Bearer
- Body (application/json):
  - AdresseLivraisonActive: string
- Success: 200 OK
  - { message: "Delivery address updated successfully" }

---

## Reclamations
Base: /api/reclamation

### POST /api/reclamation
- Auth: Bearer (Role: Client)
- Body (application/json): CreateReclamationDto (fields implemented in DTO)
- Success: 200 OK
  - returns created reclamation

### GET /api/reclamation/mes-reclamations
- Auth: Bearer (Role: Client)
- Success: 200 OK
  - returns list of reclamations for the authenticated user (ordered desc)

### GET /api/reclamation
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns list of reclamations (with related Order and ResolvedByUser)

### PUT /api/reclamation/{id}/traiter
- Auth: Bearer (Role: Admin)
- Body (application/json):
  - Status: string
  - ReponseAdmin: string
- Success: 200 OK
  - returns updated reclamation
- NotFound: 404 if reclamation not found

### DELETE /api/reclamation/{id}
- Auth: Bearer (any authenticated user)
- Success: 200 OK
  - { message: "Réclamation supprimée avec succès" }

---

## Stock
Base: /api/stock
Class: [Authorize(Roles = "Admin")] — Admin only

### POST /api/stock
- Auth: Bearer (Role: Admin)
- Body (application/json): AddStockLotDto
  - (see DTO: fields include product id, quantite, lot info, prix, type mouvement etc.)
- Success: 200 OK
  - returns result (implementation-specific)

### GET /api/stock
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns stock overview

### GET /api/stock/{produitId}
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns stock for given product id

---

## Config
Base: /api/config
Class: [Authorize(Roles = "Admin")]

### GET /api/config
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns Config object

### PUT /api/config
- Auth: Bearer (Role: Admin)
- Body (application/json): Config (entity)
- Success: 204 No Content
- NotFound: 404 if config not found

---

## Dashboard
Base: /api/dashboard
Class: [Authorize(Roles = "Admin")]

### GET /api/dashboard/stats
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns stats object

### GET /api/dashboard/alertes
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns list of alert objects

---

## AchatLot
Base: /api/AchatLot

### POST /api/AchatLot
- Auth: none (controller not decorated)
- Body (application/json):
  - ProduitId: int
  - QuantiteAchetee: int
  - PrixUnitaire: decimal
  - Fournisseur: string
  - NumeroLot: string
- Success: 200 OK
  - { message: "Achat créé avec succès", AchatLotId: int }

### GET /api/AchatLot
- Auth: Public
- Success: 200 OK
  - returns list of achat lots

### GET /api/AchatLot/{id}
- Auth: Public
- Success: 200 OK
  - returns achat lot object or 404 NotFound

### DELETE /api/AchatLot/{id}
- Auth: Public
- Success: 200 OK
  - "Achat supprimé avec succès" or 404 if not found

---

## SignalR - Notifications Hub
- Hub URL: /hubs/notifications
- Methods (client -> server):
  - JoinAdminGroup() : adds connection to "Admins" group
  - LeaveAdminGroup() : removes connection from "Admins" group
- Auth: JWT bearer may be provided via query string param `access_token` for SignalR connections (Program.cs supports this).

---

If you need a machine-readable OpenAPI/Swagger file, the project already registers Swagger in Program.cs and serves it at /swagger when running in Development.

End of document.
