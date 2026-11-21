# StockPortfolio

REST API для управления портфелем акций: регистрация пользователей, ведение списка акций,
комментарии и персональный портфель с данными из внешнего финансового API.

Пет-проект, написанный для отработки архитектуры backend-приложений на .NET:
слоистая структура, Repository Pattern, аутентификация по JWT и работа с реляционной БД через EF Core.

## Функциональность

- **Аккаунты**: регистрация и вход (ASP.NET Core Identity), выдача JWT-токена, роли `Admin` / `User` (сидируются при миграции)
- **Акции (Stocks)**: CRUD, фильтрация по символу и названию компании, сортировка, пагинация
- **Портфель (Portfolio)**: many-to-many связь «пользователь ↔ акция» — пользователь добавляет акции в свой портфель, видит только свои
- **Комментарии (Comments)**: CRUD к акциям, привязка к пользователю; пользователь может редактировать/удалять только свои
- **Интеграция с Financial Modeling Prep**: добавление акции в портфель по тикеру через внешний API (typed HttpClient)

## Стек

| Технология | Где используется |
|---|---|
| .NET 8, ASP.NET Core Web API | Основной фреймворк |
| EF Core 8 + SQL Server | Доступ к данным, Code First + миграции |
| ASP.NET Core Identity | Пользователи, роли, хеширование паролей |
| JWT Bearer (HmacSha512) | Аутентификация и авторизация |
| Swashbuckle / Swagger | Документация API, авторизация Bearer прямо в UI |
| Newtonsoft.Json | Сериализация, обработка циклических ссылок |
| xUnit | Проект юнит-тестов |

## Технические решения

- **Слоистая архитектура**: Controllers → DTO → Mappers → Repositories/Services → EF Core. Контроллеры не обращаются к DbContext напрямую.
- **Repository Pattern + Dependency Injection**: доступ к данным абстрагирован через интерфейсы (`IStockRepository`, `ICommentRepository`, `IPortfolioRepository`), регистрация через `AddScoped` — код тестируемый и заменяемый.
- **DTO + ручные мапперы**: контракты API отделены от доменных моделей; маппинг вынесен в отдельные классы (`Mappers/`).
- **Code First + миграции**: схема БД эволюционирует через миграции EF Core (init → Identity → SeedRole → Portfolio many-to-many → Comment one-to-one).
- **Сложные связи в БД**:
  - `Portfolio` — составной ключ `(AppUserId, StockId)`, many-to-many пользователь ↔ акция;
  - `Comment` — many-to-one к акции и пользователю, каскадные настройки связей.
- **Серверная фильтрация, сортировка и пагинация** через Query Object (`QueryObject`): параметры приходят в query string, строится `IQueryable` — фильтрация выполняется на уровне SQL, а не в памяти.
- **JWT-аутентификация**: токен подписывается SymmetricSecurityKey (HMAC-SHA512), валидируются issuer/audience/signing key, срок жизни — 7 дней; политика паролей: минимум 12 символов, обязательные регистр/цифры/спецсимволы.
- **Typed HttpClient для внешнего API** (`AddHttpClient<IFMPService, FMPService>`): интеграция с Financial Modeling Prep — поиск акции по тикеру и сохранение в БД.
- **Swagger с JWT-поддержкой**: схема Bearer-авторизации настроена в `AddSecurityDefinition`, что позволяет тестировать защищённые эндпоинты прямо из Swagger UI.
- **Newtonsoft.Json** с `ReferenceLoopHandling.Ignore` — корректная сериализация циклических навигационных свойств (Stock → Comments → Stock).

## Модель данных

```
AppUser 1───* Portfolio *───1 Stock 1───* Comment *───1 AppUser
```

- **Stock** — символ, название компании, цена покупки, дивиденды, отрасль, капитализация
- **Comment** — заголовок, содержание, автор, связанная акция
- **Portfolio** — связь пользователя и акции
- **AppUser** — расширение IdentityUser

## Основные эндпоинты

| Метод | Путь | Описание |
|---|---|---|
| POST | `/api/account/register` | Регистрация |
| POST | `/api/account/login` | Вход, получение JWT |
| GET | `/api/stock?symbol=&companyName=&sortBy=&pageNumber=&pageSize=` | Список акций с фильтрами |
| GET/POST/PUT/DELETE | `/api/stock/{id}` | CRUD акций |
| POST | `/api/portfolio/{symbol}` | Добавить акцию в портфель |
| GET/DELETE | `/api/portfolio` | Мой портфель / удалить |
| GET/POST/PUT/DELETE | `/api/comment` | CRUD комментариев |

## Запуск

```bash
cd api
dotnet ef database update          # применить миграции
dotnet run
```

Требуется SQL Server и строка подключения `DefaultConnection` в `appsettings.json`,
а также ключи `JWT:Issuer`, `JWT:Audience`, `JWT:SigningKey` и `FMPKey` (apikey Financial Modeling Prep).

После запуска документация доступна по адресу `/swagger`.
