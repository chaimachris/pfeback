# DeliverWholesale API - Endpoints Reference

This document lists the HTTP endpoints provided by the DeliverWholesale backend, expected request payloads, authorization requirements, and typical responses. Give this to the development team for implementation.

Notes:
- The API base path is /api
- Authentication uses JWT tokens in the Authorization header: `Authorization: Bearer <token>`
- Roles used: Admin, Client (where indicated)
- Date/time values use ISO 8601 where applicable

---

## Auth

### POST /api/auth/register
- Auth: Public
- Body (application/json):
  - FullName: string (required)
  - Email: string (required, email)
  - Password: string (required)
- Success: 200 OK
  - { message: string }
- Notes: Creates a new user and triggers email confirmation flow.

### GET /api/auth/confirm-email?email={email}&token={token}
- Auth: Public
- Query params:
  - email: string
  - token: string
- Success: 200 OK
  - { message: string }

### POST /api/auth/login
- Auth: Public
- Body (application/json):
  - Email: string (required)
  - Password: string (required)
- Success: 200 OK
  - { token: string, role: string, email: string, fullName: string }
- Error: 400/exception if credentials invalid or email not confirmed.

### POST /api/auth/logout
- Auth: Bearer (any authenticated user)
- Body: none
- Success: 200 OK
  - { message: "Déconnexion réussie." }
- Notes: Token is revoked server-side.

---

## Products
Base: /api/products

### POST /api/products
- Auth: none (controller not decorated) — check business rules (may require auth depending on configuration)
- Content-Type: multipart/form-data
- Body (form):
  - libelle: string
  - Description: string
  - PrixVente: decimal
  - idCategorie: int
  - NbUnite: int
  - seuil: int
  - prixModifiable: bool
  - Image: file (optional)
- Success: 200 OK
  - returns created product object (implementation-specific)

### GET /api/products
- Auth: Public
- Success: 200 OK
  - returns list of products

### PUT /api/products/{id}
- Auth: Public
- Content-Type: multipart/form-data
- Body (form):
  - libelle: string? (optional)
  - Description: string? (optional)
  - NouveauPrixVente: decimal? (optional) — will create a new prix entry if provided
  - idCategorie: int? (optional)
  - seuil: int? (optional)
  - prixModifiable: bool? (optional)
  - Image: file? (optional)
- Success: 200 OK (empty body)
- NotFound: 404 if product not found

### DELETE /api/products/{id}
- Auth: Public
- Success: 200 OK
- NotFound: 404 if product not found

### POST /api/products/{id}/prix
- Auth: Public
- Body (application/json):
  - Valeur: decimal
- Success: 200 OK
  - { PrixVenteId: int }
- Notes: id (route) is assigned to dto.idP by controller.

### GET /api/products/{id}/prix
- Auth: Public
- Success: 200 OK
  - returns price history for product

---

## Categories
Base: /api/categories

### GET /api/categories
- Auth: Public
- Success: 200 OK
  - returns list of categories

### POST /api/categories
- Auth: Bearer (Role: Admin)
- Body (application/json): CategoryDto
  - (CategoryDto fields as implemented in code: typically name, etc.)
- Success: 200 OK
  - returns created category object

### PUT /api/categories/{id}
- Auth: Bearer (Role: Admin)
- Body (application/json): CategoryDto
- Success: 204 No Content
- NotFound: 404 if category not found

### DELETE /api/categories/{id}
- Auth: Bearer (Role: Admin)
- Success: 204 No Content
- NotFound: 404 if category not found

---

## Orders
Base: /api/order
Class: [Authorize] — requires authentication

### POST /api/order
- Auth: Bearer
- Body (application/json):
  - Items: array of { ProduitId: int, Quantite: int }
- Success: 200 OK
  - { Message: string, OrderId: int }
- Error: 400 Bad Request on failure

### GET /api/order
- Auth: Bearer
- Success: 200 OK
  - returns list of orders

### GET /api/order/{id}
- Auth: Bearer
- Success: 200 OK
  - returns single order object
- NotFound: 404 if order not found

### DELETE /api/order/{id}
- Auth: Bearer
- Success: 200 OK
  - { Message: "Commande supprimée" }
- NotFound: 404 if order not found

### PUT /api/order/{id}/status
- Auth: Bearer
- Body (application/json):
  - Statut: string
- Success: 200 OK
  - { message: "Statut mis à jour" }
- NotFound: 404 if order not found

---

## Deliveries
Base: /api/delivery
Class: [Authorize(Roles = "Admin")] — Admin only

### POST /api/delivery
- Auth: Bearer (Role: Admin)
- Body (application/json):
  - OrderId: int
  - AdresseLivraison: string
  - DateLivraisonPrevue: DateTime (ISO 8601)
- Success: 200 OK
  - returns created delivery object

### GET /api/delivery
- Auth: Bearer (Role: Admin)
- Success: 200 OK
  - returns list of deliveries

### PUT /api/delivery/{id}/status
- Auth: Bearer (Role: Admin)
- Body (application/json):
  - Statut: DeliveryStatus (enum)
  - DateLivraisonReelle: DateTime? (optional)
- Success: 200 OK
- NotFound: 404 if delivery not found

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
