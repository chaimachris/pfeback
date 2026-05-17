**DeliverWholesale API Endpoints**

**Overview**
Base URL: /api
Auth: JWT Bearer for endpoints marked "Auth: Bearer" (use Authorization: Bearer <token>).
Roles: Admin or Client enforced where noted.
Enums: serialized as strings (JsonStringEnumConverter enabled).
SignalR hub: /hubs/notifications (token via access_token query param).
Dates: use ISO 8601 strings (e.g., 2026-05-18T12:00:00Z).

**Enum Values**
DeliveryStatus: EnAttente | Confirmee | Livree | Annulee
StatutOrder: EnAttente | Confirmee | Livree | Annulee
TypeMouvement: Entree | Sortie | Ajustement
NotificationType: NEW_ORDER | CANCEL_ORDER | NEW_RECLAMATION

**Auth**
**POST /api/auth/register**
Auth: Public
Input (JSON): RegisterDto
Output 200:
```json
{ "message": "string" }
```
Notes: message is a server string indicating success or email delivery status.

**GET /api/auth/confirm-email**
Auth: Public
Query: email (string), token (string)
Output 200:
```json
{ "message": true }
```
Notes: message is boolean (true if confirmed, false if not found).

**POST /api/auth/login**
Auth: Public
Input (JSON): LoginDto
Output 200:
```json
{ "token": "string", "role": "Admin|Client", "email": "string", "fullName": "string" }
```
Errors: invalid credentials or unconfirmed email throw exceptions.

**POST /api/auth/logout**
Auth: Bearer
Input: none
Output 200:
```json
{ "message": "string" }
```

**Categories**
**GET /api/categories**
Auth: Public
Input: none
Output 200: List<Categorie> (includes Produits)

**POST /api/categories**
Auth: Bearer (Admin)
Input (JSON): CategoryDto
Output 200: Categorie
Notes: ParentId from CategoryDto is currently ignored in handler.

**PUT /api/categories/{id}**
Auth: Bearer (Admin)
Input (JSON): CategoryDto
Output 204: No content

**DELETE /api/categories/{id}**
Auth: Bearer (Admin)
Output 204: No content

**Products**
**POST /api/products**
Auth: Public
Content-Type: multipart/form-data
Input (form-data): ProductCreateDto
Output 200:
```json
123
```
Notes: response body is the new product id (int).

**GET /api/products**
Auth: Public
Output 200: List<Produit> (includes Categorie, PrixVentes, StockLots)
Notes: IsActive filter is commented out, so inactive products may appear.

**GET /api/products/{id}**
Auth: Public
Output 200: Produit (includes Categorie, PrixVentes, StockLots)
Output 404: empty body

**PUT /api/products/{id}**
Auth: Public
Content-Type: multipart/form-data
Input (form-data): ProductUpdateDto
Output 200: empty body
Output 404: empty body
Notes: NouveauPrixVente creates a new PrixVente entry.

**DELETE /api/products/{id}**
Auth: Public
Output 200: empty body
Output 404: empty body
Notes: soft delete (IsActive = false).

**POST /api/products/{id}/prix**
Auth: Public
Input (JSON): PrixVenteCreateDto (idP is overridden by route id)
Output 200:
```json
{ "PrixVenteId": 123 }
```

**GET /api/products/{id}/prix**
Auth: Public
Output 200: List<PrixVente>

**Achat Lot (Purchases)**
**POST /api/achatlot**
Auth: Public
Input (JSON): CreateAchatLotCommand
Output 200:
```json
{ "message": "string", "AchatLotId": 123 }
```

**GET /api/achatlot**
Auth: Public
Output 200: List<AchatLot> (includes Produit, StockLots)

**GET /api/achatlot/{id}**
Auth: Public
Output 200: AchatLot (includes Produit, StockLots)
Output 404: string message

**DELETE /api/achatlot/{id}**
Auth: Public
Output 200: string message
Output 404: string message

**Stock**
**POST /api/stock**
Auth: Bearer (Admin)
Input (JSON): AddStockLotDto
Output 200:
```json
123
```
Notes: response body is stockLotId (int).

**GET /api/stock**
Auth: Bearer (Admin)
Output 200: List<StockDetailsDTO>

**GET /api/stock/{produitId}**
Auth: Bearer (Admin)
Output 200: List<StockLot> (includes AchatLot)

**Panier (Cart)**
**GET /api/panier**
Auth: Bearer
Output 200: PanierDto

**POST /api/panier/items**
Auth: Bearer
Input (JSON): AddToPanierDto
Output 200: PanierDto
Output 400: "Quantite invalide."
Output 404: "Produit introuvable."

**PUT /api/panier/items/{produitId}**
Auth: Bearer
Input (JSON): UpdatePanierItemDto
Output 200: PanierDto
Output 400: "Quantite invalide."
Output 404: "Panier introuvable." or "Produit introuvable dans le panier."

**DELETE /api/panier/items/{produitId}**
Auth: Bearer
Output 200: PanierDto
Output 404: "Panier introuvable." or "Produit introuvable dans le panier."

**DELETE /api/panier/clear**
Auth: Bearer
Output 200: PanierDto
Output 404: "Panier introuvable."

**POST /api/panier/checkout**
Auth: Bearer
Input: none
Output 200:
```json
{ "Message": "string", "OrderId": 123 }
```
Output 400: "Panier vide."

**Orders**
**POST /api/order**
Auth: Bearer
Input (JSON): OrderCreateDto
Output 200:
```json
{ "Message": "string", "OrderId": 123 }
```
Output 400:
```json
{ "Message": "string", "Error": "string" }
```

**GET /api/order**
Auth: Bearer
Output 200: List<Order> (current user only, includes OrderDetails+Produit and Delivery)

**GET /api/order/{id}**
Auth: Bearer
Output 200: Order (current user only, includes OrderDetails+Produit and Delivery)
Output 404: "Commande introuvable"

**DELETE /api/order/{id}**
Auth: Bearer
Output 200:
```json
{ "Message": "string" }
```
Output 404: "Commande introuvable"
Notes: sets Statut = Annulee and reverts stock; does not delete rows.

**PUT /api/order/{id}/status**
Auth: Bearer
Input (JSON): UpdateOrderStatusDto (Statut string)
Output 200:
```json
{ "message": "string" }
```
Output 404: "Commande introuvable"
Notes: Statut must parse to StatutOrder (case-insensitive).

**Deliveries**
**POST /api/delivery**
Auth: Bearer (Admin)
Input (JSON): CreateDeliveryDto
Output 200: Delivery

**GET /api/delivery**
Auth: Bearer (Admin)
Output 200: List<Delivery> (includes Order)

**PUT /api/delivery/{id}/status**
Auth: Bearer (Admin)
Input (JSON): UpdateDeliveryStatusDto
Output 200:
```json
"string"
```
Output 404: empty body
Notes: if Statut == Livree, DateLivraisonReelle is set to now.

**GET /api/delivery/today/products**
Auth: Bearer (Admin)
Output 200:
```json
[{ "Produit": "string", "Quantite": 1, "Prix": 10.5, "OrderId": 123 }]
```

**GET /api/delivery/today/clients**
Auth: Bearer (Admin)
Output 200:
```json
[{ "DeliveryId": 1, "OrderId": 123, "Adresse": "string", "Statut": "EnAttente" }]
```

**PUT /api/delivery/{id}/date**
Auth: Bearer (Admin)
Input (JSON): DateTime string
Output 200:
```json
"string"
```
Output 404: empty body

**Reclamations**
**POST /api/reclamation**
Auth: Bearer (Client)
Input (JSON): CreateReclamationDto
Output 200: Reclamation

**GET /api/reclamation/mes-reclamations**
Auth: Bearer (Client)
Output 200: List<Reclamation>

**GET /api/reclamation**
Auth: Bearer (Admin)
Output 200: List<Reclamation> (includes Order, ResolvedByUser)

**PUT /api/reclamation/{id}/traiter**
Auth: Bearer (Admin)
Input (JSON): TraiterReclamationDto
Output 200: Reclamation
Notes: if Status == "Résolue", DateResolution and ResolvedByUserId are set.

**DELETE /api/reclamation/{id}**
Auth: Bearer
Output 200:
```json
{ "message": "string" }
```

**Profile**
**GET /api/profile**
Auth: Bearer
Output 200:
```json
{ "Id": 1, "Nom": "string", "Prenom": "string", "Email": "string", "Adresse": "string", "Role": "Admin|Client" }
```

**PUT /api/profile**
Auth: Bearer
Input (JSON): UpdateProfileDto
Output 200:
```json
"string"
```

**PUT /api/profile/change-password**
Auth: Bearer
Input (JSON): ChangePasswordDto
Output 200:
```json
"string"
```
Output 400: "Old password is incorrect"

**PUT /api/profile/update-delivery-address**
Auth: Bearer
Input (JSON): UpdateDeliveryAddressDto
Output 200:
```json
"string"
```

**Dashboard**
**GET /api/dashboard/stats**
Auth: Bearer (Admin)
Output 200:
```json
{ "TotalProduits": 0, "TotalCommandes": 0, "CommandesEnAttente": 0, "StockBas": 0, "StockNegatif": 0, "ChiffreAffairesMois": 0.0 }
```

**GET /api/dashboard/alertes**
Auth: Bearer (Admin)
Output 200:
```json
[{ "Produit": "string", "Stock": 0, "EstCritique": true }]
```

**Config**
**GET /api/config**
Auth: Bearer (Admin)
Output 200: Config

**PUT /api/config**
Auth: Bearer (Admin)
Input (JSON): Config
Output 204: No content
Output 404:
```json
"Configuration introuvable"
```

**SignalR Notifications**
**WS /hubs/notifications**
Auth: access_token query parameter (JWT)
Client-to-server methods:
- JoinAdminGroup()
- LeaveAdminGroup()
Server-to-client event:
- ReceiveNotification({ Message, Type, CreatedAt })

**Models (Request/Response Shapes)**
RegisterDto:
```json
{ "FullName": "string", "Email": "string", "Password": "string" }
```

LoginDto:
```json
{ "Email": "string", "Password": "string" }
```

CreateAchatLotCommand:
```json
{ "ProduitId": 1, "QuantiteAchetee": 10, "PrixUnitaire": 5.5, "Fournisseur": "string", "SupplierId": 1 }
```

CategoryDto:
```json
{ "Nom": "string", "Description": "string", "ParentId": 1 }
```

ProductCreateDto (multipart/form-data):
```
libelle: string
Description: string
PrixVente: number
idCategorie: int
NbUnite: int
seuil: int
prixModifiable: bool
Image: file (optional)
```

ProductUpdateDto (multipart/form-data):
```
libelle: string (optional)
Description: string (optional)
NouveauPrixVente: number (optional)
idCategorie: int (optional)
seuil: int (optional)
prixModifiable: bool (optional)
Image: file (optional)
```

PrixVenteCreateDto:
```json
{ "idP": 1, "Valeur": 10.5 }
```

AddStockLotDto:
```json
{ "AchatLotId": 1, "Quantite": 10, "PrixAchatTotal": 100.0, "Fournisseur": "string", "Unite": "string" }
```
Notes: Unite is not used in current handler.

OrderCreateDto:
```json
{ "Items": [ { "ProduitId": 1, "Quantite": 2 } ] }
```

UpdateOrderStatusDto:
```json
{ "Statut": "EnAttente" }
```

CreateDeliveryDto:
```json
{ "OrderId": 1, "AdresseLivraison": "string", "DateLivraisonPrevue": "2026-05-18T12:00:00Z" }
```

UpdateDeliveryStatusDto:
```json
{ "Statut": "EnAttente", "DateLivraisonReelle": "2026-05-18T12:00:00Z" }
```

UpdateProfileDto:
```json
{ "Nom": "string", "Prenom": "string", "Email": "string", "Adresse": "string" }
```

ChangePasswordDto:
```json
{ "OldPassword": "string", "NewPassword": "string" }
```

UpdateDeliveryAddressDto:
```json
{ "AdresseLivraisonActive": "string" }
```

AddToPanierDto:
```json
{ "ProduitId": 1, "Quantite": 2 }
```

UpdatePanierItemDto:
```json
{ "Quantite": 3 }
```

PanierDto:
```json
{ "UserId": 1, "TotalPrix": 0.0, "Items": [ { "ProduitId": 1, "Libelle": "string", "Quantite": 2, "PrixUnitaire": 10.5, "SousTotal": 21.0, "ImageUrl": "/images/products/x.jpg" } ] }
```

CreateReclamationDto:
```json
{ "OrderId": 1, "Sujet": "string", "Description": "string" }
```

TraiterReclamationDto:
```json
{ "Status": "En attente", "ReponseAdmin": "string" }
```
Notes: if Status equals "Résolue" (accent required), resolution fields are set.

StockDetailsDTO:
```json
{ "StockLotId": [1,2], "Product": { "idP": 1 }, "QuantiteTotalRestante": 10.0, "Transations": [ { "Id": 1, "StockLotId": 1, "OrderDetailId": 1, "Type": "Entree", "Quantite": 5, "DateMouvement": "2026-05-18T12:00:00Z" } ] }
```

Produit:
```json
{ "idP": 1, "libelle": "string", "Description": "string", "seuil": 0, "prixModifiable": false, "idCategorie": 1, "NbUnite": 1, "IsActive": true, "ImageUrl": "/images/products/x.jpg", "Categorie": { "Id": 1 }, "PrixVentes": [ { "Id": 1, "idP": 1, "Valeur": 10.5, "Date": "2026-05-18T12:00:00Z" } ], "StockLots": [ { "Id": 1, "AchatLotId": 1, "QuantiteRestante": 10, "DateReception": "2026-05-18T12:00:00Z", "ExpirationDate": null, "ProduitId": 1 } ], "StockDisponible": 10, "PrixVenteActuel": 10.5 }
```

PrixVente:
```json
{ "Id": 1, "idP": 1, "Valeur": 10.5, "Date": "2026-05-18T12:00:00Z" }
```

Categorie:
```json
{ "Id": 1, "Nom": "string", "Description": "string", "ParentId": null, "SousCategories": [], "Produits": [] }
```

AchatLot:
```json
{ "Id": 1, "ProduitId": 1, "Produit": { "idP": 1 }, "DateAchat": "2026-05-18T12:00:00Z", "QuantiteAchetee": 10, "PrixUnitaire": 5.0, "Fournisseur": "string", "SupplierId": 1, "NumeroLot": "string", "StockLots": [ { "Id": 1 } ] }
```

StockLot:
```json
{ "Id": 1, "AchatLotId": 1, "QuantiteRestante": 10, "DateReception": "2026-05-18T12:00:00Z", "ExpirationDate": null, "ProduitId": 1, "RowVersion": "base64" }
```

Order:
```json
{ "Id": 1, "UserId": 1, "DateCommande": "2026-05-18T12:00:00Z", "TotalProduits": 100.0, "FraisLivraison": 0.0, "TotalFinal": 100.0, "Statut": "EnAttente", "Delivery": { "Id": 1 }, "OrderDetails": [ { "Id": 1, "OrderId": 1, "ProduitId": 1, "Quantite": 2, "PrixUnitaire": 50.0, "SousTotal": 100.0, "Produit": { "idP": 1 } } ] }
```

Delivery:
```json
{ "Id": 1, "OrderId": 1, "AdresseLivraison": "string", "DateLivraisonPrevue": "2026-05-18T12:00:00Z", "DateLivraisonReelle": null, "Statut": "EnAttente" }
```

Reclamation:
```json
{ "Id": 1, "OrderId": 1, "UserId": "1", "Sujet": "string", "Description": "string", "Status": "En attente", "ReponseAdmin": "string", "DateCreation": "2026-05-18T12:00:00Z", "DateResolution": null, "ResolvedByUserId": null }
```

Config:
```json
{ "Id": 1, "MontantMinimumCommande": 100.0, "ProfitPercentage": 20.0, "FraisLivraison": 15.0, "SeuilAlerteStockBas": 10 }
```

Notes: many responses include EF navigation properties; cycles are ignored, so some nested properties may be omitted when already referenced.
