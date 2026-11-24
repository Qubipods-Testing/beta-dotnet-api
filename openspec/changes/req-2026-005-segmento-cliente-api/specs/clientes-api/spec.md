# Capability: clientes-api

## ADDED Requirements

### Requirement: Segmento del cliente en la respuesta
`GET /api/clientes/{id}` SHALL include a `Segmento` field in the response body,
computed by the provider as `Frecuente` when `FechaAlta` is 6 months or more
before the current UTC instant, and `Nuevo` otherwise (BR-01).

#### Scenario: Cliente con alta de hace un año
- **GIVEN** a cliente whose `FechaAlta` is one year before today (UTC)
- **WHEN** a consumer calls `GET /api/clientes/{id}` for that cliente
- **THEN** the response is `200 OK` and its body has `Segmento: "Frecuente"`

#### Scenario: Cliente con alta de hace un mes
- **GIVEN** a cliente whose `FechaAlta` is one month before today (UTC)
- **WHEN** a consumer calls `GET /api/clientes/{id}` for that cliente
- **THEN** the response is `200 OK` and its body has `Segmento: "Nuevo"`

### Requirement: Cliente inexistente
`GET /api/clientes/{id}` SHALL respond `404 Not Found` with an `ErrorDto`
(`Codigo`, `Mensaje`) when no cliente with the given id exists.

#### Scenario: Id inexistente
- **GIVEN** no cliente exists with the requested id
- **WHEN** a consumer calls `GET /api/clientes/{id}`
- **THEN** the response is `404 Not Found` and its body has
  `Codigo: "CLIENTE_NO_ENCONTRADO"`

### Requirement: Contrato `clientes-api` publicado
`beta-dotnet-api` SHALL publish an `http-api` contract named `clientes-api`
documenting `GET /api/clientes/{id}` including the `Segmento` field, so that
consumer pods can build against a versioned definition instead of reimplementing
BR-01.

#### Scenario: Contrato describe la operación existente
- **GIVEN** the `clientes-api` contract package in this change
- **WHEN** it is validated against the `qubipods.contract/1` format
- **THEN** it declares `GET /api/clientes/{id}` with its 200 and 404 responses,
  each with at least one example, and passes `check-contracts.mjs`
