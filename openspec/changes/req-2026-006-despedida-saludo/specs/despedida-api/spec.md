# Capability: despedida-api

## ADDED Requirements

### Requirement: Despedida personalizada
`GET /despedida` SHALL respond `200 OK` with the plain-text body
`Hasta pronto, <nombre>` when the request includes a non-empty `nombre` query
parameter.

#### Scenario: Nombre presente
- **GIVEN** a request to `GET /despedida?nombre=Ana`
- **WHEN** the endpoint handles it
- **THEN** the response is `200 OK` with body `Hasta pronto, Ana`

### Requirement: Nombre requerido
`GET /despedida` SHALL respond `400 Bad Request` with an `ErrorDto`
(`Codigo`, `Mensaje`) when the `nombre` query parameter is missing or empty.

#### Scenario: Nombre ausente
- **GIVEN** a request to `GET /despedida` without a `nombre` query parameter
- **WHEN** the endpoint handles it
- **THEN** the response is `400 Bad Request` and its body has
  `Codigo: "NOMBRE_REQUERIDO"`
