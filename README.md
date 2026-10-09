# Ecommerce — .NET 10 (Blazor + ASP.NET Core Web API + EF Core)

Migración del proyecto Angular + Spring Boot a **Blazor Web App** (Interactive Server) + **Web API con Controllers** + **EF Core 10** (PostgreSQL), con **C# 14**.

## Estructura

```
Ecommerce.slnx
src/
  Ecommerce.Shared/            ViewModels y enums compartidos (los "contratos" entre API y Blazor)
    ViewModels/
    Enums/

  Ecommerce.Api/               API REST  (Controller -> Service -> DbContext)
    Controllers/               ArticlesController, FavouritesController, CartController
    Services/                  IArticleService/ArticleService, IFavouriteService/..., ICartService/...
    Domain/                    entidades EF (las de ecommerce.entity)
    Data/                      AppDbContext, DataSeeder
    Infrastructure/            excepciones -> ProblemDetails, registro de servicios

  Ecommerce.Web/               Blazor  (Page -> Component / State -> ApiClient -> HTTP)
    Pages/                     ArticlesPage, CartPage, NotFound
    Components/                Catalog/, Cart/, Common/, Layout/
    ApiClients/                ArticlesApiClient, FavouritesApiClient, CartApiClient (+ interfaces)
    State/                     CartState, FavouritesState, ToastService, CurrentCustomer
    Models/                    ArticleFilter (lógica de filtrado de la UI)
    Infrastructure/            registro de servicios y opciones
```

## Flujo de una petición

```
[Navegador] --SignalR--> [Ecommerce.Web: ArticlesPage]
                               │ inyecta IArticlesApiClient
                               ▼
                    ArticlesApiClient.GetAllAsync()
                               │ HTTP  GET http://localhost:5080/api/articles
                               ▼
[Ecommerce.Api]   ASP.NET Core routing:  [Route("api/articles")] + [HttpGet]
                               ▼
                    ArticlesController.GetAll()
                               │ inyecta IArticleService
                               ▼
                    ArticleService.GetCatalogAsync()  ->  AppDbContext  ->  PostgreSQL
                               ▼
                    List<ArticleViewModel>  --JSON-->  vuelve por el mismo camino
```

El ApiClient y el Controller **sólo se conocen por la URL** (y por los ViewModels compartidos). Cada ApiClient tiene comentado encima de cada método el endpoint al que llama. Blazor Server ejecuta el ApiClient en el servidor, así que no hace falta CORS.

## Arranque local

**Requisitos:** [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) y PostgreSQL (local o con `docker compose up -d db`).

```bash
dotnet run --project src/Ecommerce.Api     # API    -> http://localhost:5080
dotnet run --project src/Ecommerce.Web     # Blazor -> http://localhost:5173  (otra terminal)
```

- Conexión a la BD: `src/Ecommerce.Api/appsettings.Development.json` → `Database=ecommerce_dotnet` (usuario/contraseña `postgres`). Se crea sola. Usa una BD **distinta** de la del proyecto Java: si ya existen tablas con otro esquema, `EnsureCreated` no las toca.
- En Development la API crea las tablas y ejecuta el **DataSeeder** al arrancar (`Database:AutoInitialize`). Es idempotente.
- URL de la API vista desde Blazor: `Api:BaseUrl` en `src/Ecommerce.Web/appsettings.json`.

### Datos del DataSeeder
Los del script `insert_queries_start_project.sql` (mismos Guids) + franjas de recogida + el cliente de demo `d0474d9a-d3ed-433c-a5d0-f79585222e12` con un favorito y un carrito guardado.

## API

| Método y ruta | Controller.Acción | Respuesta |
|---|---|---|
| `GET /api/articles` | `ArticlesController.GetAll` | 200 `ArticleViewModel[]` |
| `GET /api/customers/{id}/favourites` | `FavouritesController.GetAll` | 200 `Guid[]` |
| `PUT /api/customers/{id}/favourites/{articleId}` | `FavouritesController.Add` | 204 |
| `DELETE /api/customers/{id}/favourites/{articleId}` | `FavouritesController.Remove` | 204 |
| `GET /api/customers/{id}/cart` | `CartController.Get` | 200 `CartViewModel` |
| `PUT /api/customers/{id}/cart` | `CartController.Save` | 204 |
| `DELETE /api/customers/{id}/cart` | `CartController.Clear` | 204 |

Errores en formato ProblemDetails: los services lanzan `NotFoundException` (404) o `RuleViolationException` (400) y `ApiExceptionHandler` las traduce, así los controllers no repiten `if (x is null) return NotFound();`. Ejemplos de peticiones: `src/Ecommerce.Api/Ecommerce.Api.http`. OpenAPI: `/openapi/v1.json`.

## Componentes

- `CartState` y `FavouritesState` (scoped = una sesión) guardan el estado y emiten `Changed`; usan los ApiClients. Quien lo muestra se suscribe (`AppHeader`, `CartPage`, `ArticlesPage`); quien lo modifica llama al estado (`ArticleCard`, `CartItemRow`).
- `ArticleCard` sólo recibe `Article`; tienda elegida, cantidad pendiente y aviso de stock son estado propio.
- Parámetros / `EventCallback` sólo donde hay entrada/salida real: `ArticleFilters` (`@bind-Filter`), `StoreSelector`, `QuantityStepper`, `ConfirmDialog`, `CategorySection`.
- Los componentes nunca usan `HttpClient`: pasan por el ApiClient (o por el State que lo envuelve).

## Migraciones EF (opcional)
Se usa `EnsureCreated`. Para migraciones: `dotnet tool install --global dotnet-ef`, `dotnet ef migrations add Initial --project src/Ecommerce.Api` y cambia `EnsureCreatedAsync()` por `MigrateAsync()` en `InitialiseDatabaseAsync`.

## Estado de la verificación
`dotnet build` de la versión anterior compiló sin errores en tu máquina; esta versión reorganizada no se ha compilado aquí (no hay SDK en mi entorno), así que haz un `dotnet build` primero.
