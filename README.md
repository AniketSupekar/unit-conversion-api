# Unit Conversion API

A REST API built with ASP.NET Core 9 that converts values between units of measurement.

**Repo:** https://github.com/AniketSupekar/unit-conversion-api

---

## Run Locally

```bash
git clone https://github.com/AniketSupekar/unit-conversion-api.git
cd unit-conversion-api
dotnet restore
cd src/UnitConversionApi
dotnet run
```

Open `http://localhost:5000` — Swagger UI loads automatically.

## Run Tests

```bash
dotnet test
```

41 tests, all passing.

---

## Endpoints

### `POST /api/convert`

```json
{
  "value": 100,
  "fromUnit": "celsius",
  "toUnit": "fahrenheit"
}
```

```json
{
  "inputValue": 100,
  "fromUnit": "celsius",
  "toUnit": "fahrenheit",
  "result": 212,
  "category": "temperature"
}
```

### `GET /api/units`

Returns all supported units grouped by category.

---

## Supported Categories

| Category | Examples |
|----------|---------|
| Length | meter, kilometer, mile, foot, inch, yard |
| Temperature | celsius, fahrenheit, kelvin, rankine |
| Weight | kilogram, gram, pound, ounce, tonne |
| Area | square meter, hectare, acre, square mile |
| Volume | liter, milliliter, gallon, pint, cup |
| Speed | km/h, mph, m/s, knots |

Unit names are case-insensitive. Abbreviations like `km`, `kg`, `mph` all work.

---

## How It Works

**Linear conversions** (length, weight, area, volume, speed) use a base unit pivot:
```
result = value × (fromFactor / toFactor)
```
Adding a new unit is one line in `UnitDefinitions.cs`.

**Temperature** is handled separately because scales have different zero points everything converts through Celsius as a middle step. Absolute zero is validated.

**Each category has its own converter** implementing `IUnitConverter`. Adding a new category means one new file. Nothing existing changes.

**Errors are handled in one place** : a middleware catches all exceptions and returns consistent JSON error responses. Controllers have no try/catch.

---

## Error Codes

| Code | Meaning |
|------|---------|
| `UNSUPPORTED_UNIT` | Unit not recognised |
| `INCOMPATIBLE_UNITS` | Units from different categories |
| `INVALID_VALUE` | Physically impossible value |
| `VALIDATION_ERROR` | Missing request fields |

---

## Project Structure

```
src/UnitConversionApi/
├── Controllers/    # HTTP routing only
├── Converters/     # One class per category
├── Data/           # All unit definitions
├── Exceptions/     # Custom exception types
├── Middleware/     # Global error handling
├── Models/         # Request/response shapes
└── Services/       # Business logic

tests/UnitConversionApi.Tests/
├── TemperatureConverterTests.cs
├── LinearConverterTests.cs
├── ConversionServiceTests.cs
└── ConversionApiIntegrationTests.cs
```