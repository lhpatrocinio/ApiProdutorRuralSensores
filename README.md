# 📡 AgroSolutions - API de Sensores

Microsserviço responsável pelo **gerenciamento de sensores IoT** e **registro de leituras** dos equipamentos instalados nos talhões da plataforma AgroSolutions.

## Visão Geral

| Item | Detalhe |
|------|---------|
| **Porta padrão** | 5002 |
| **Banco de dados** | AgroSensores (SQL Server) |
| **Endpoints** | 21 |
| **Testes unitários** | 42 |
| **Autenticação** | JWT Bearer (obrigatório) |

## Responsabilidades

- Gerenciar sensores IoT associados aos talhões
- Registrar e consultar leituras de sensores (temperatura, umidade, pH, precipitação, etc.)
- Publicar evento `SensorDataReceivedEvent` via RabbitMQ a cada nova leitura registrada
- Alimentar os dados base para o motor de alertas da API de Monitoramento

## Estrutura do Projeto (Clean Architecture)

```
ApiProdutorRuralSensores/
├── ProdutorRuralSensores.Domain/         # Entidades, interfaces, value objects
├── ProdutorRuralSensores.Application/    # Use cases, DTOs, event publishers
├── ProdutorRuralSensores.Infrastructure/ # EF Core, SQL Server, RabbitMQ Publisher
├── ProdutorRuralSensores.Api/            # Controllers, middlewares, Swagger
└── ProdutorRuralSensores.Tests/          # Testes unitários (xUnit + Moq)
```

## Endpoints

### Sensores (`/api/v1/Sensores`)

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET`    | `/` | Lista todos os sensores |
| `GET`    | `/{id}` | Busca sensor por ID |
| `GET`    | `/talhao/{talhaoId}` | Lista sensores de um talhão |
| `POST`   | `/` | Cadastra novo sensor |
| `PUT`    | `/{id}` | Atualiza sensor |
| `DELETE` | `/{id}` | Remove sensor |
| `PATCH`  | `/{id}/ativar` | Ativa sensor |
| `PATCH`  | `/{id}/desativar` | Desativa sensor |

### Leituras (`/api/v1/Leituras`)

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET`    | `/` | Lista todas as leituras |
| `GET`    | `/{id}` | Busca leitura por ID |
| `GET`    | `/sensor/{sensorId}` | Leituras de um sensor |
| `GET`    | `/talhao/{talhaoId}` | Leituras de um talhão |
| `GET`    | `/talhao/{talhaoId}/ultima` | Última leitura do talhão |
| `GET`    | `/talhao/{talhaoId}/periodo` | Leituras por período |
| `POST`   | `/` | Registra nova leitura (publica evento) |
| `POST`   | `/lote` | Registra leituras em lote |
| `GET`    | `/estatisticas/talhao/{talhaoId}` | Estatísticas do talhão |
| `GET`    | `/export/talhao/{talhaoId}` | Exporta leituras CSV |
| `DELETE` | `/{id}` | Remove leitura |

## Como Executar Localmente

### Pré-requisitos

- .NET 8 SDK
- SQL Server + RabbitMQ rodando (via Docker — ver [AgroSolutions-Infra](https://github.com/marceloms17/AgroSolutions-Infra))
- Token JWT obtido via [API de Autenticação](https://github.com/lhpatrocinio/ApiProdutorRuralAutenticacao)

### Executar

```powershell
git clone https://github.com/lhpatrocinio/ApiProdutorRuralSensores.git
cd ApiProdutorRuralSensores
dotnet restore
dotnet run --project ProdutorRuralSensores.Api
```

Swagger disponível em: `http://localhost:5002/swagger`

### Executar Testes

```powershell
dotnet test
```

## Mensageria RabbitMQ

Este serviço **publica** eventos a cada leitura registrada:

| Direção | Exchange | Routing Key | Evento |
|---------|----------|-------------|--------|
| **Publica** | `agro.events` | `sensor.data.{talhaoId}` | `SensorDataReceivedEvent` → consumido pela API de Monitoramento |

### Payload do evento `SensorDataReceivedEvent`

```json
{
  "SensorId": "guid",
  "TalhaoId": "guid",
  "TipoSensor": "Temperatura",
  "Valor": 38.5,
  "Unidade": "°C",
  "DataLeitura": "2026-02-27T10:00:00Z"
}
```

## Dados de Seed

- 13 sensores distribuídos nos talhões
- ~14.248 leituras históricas (30 dias × 24h × 13 sensores)

## Tecnologias

- .NET 8 / ASP.NET Core
- Entity Framework Core 8 + SQL Server
- RabbitMQ (MassTransit) — Publisher
- JWT Bearer Authentication
- Polly (Resilience: Retry, Circuit Breaker)
- Swagger / OpenAPI
- xUnit + Moq + FluentAssertions
- GitHub Actions (CI/CD)