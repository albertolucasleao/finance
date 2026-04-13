# TCE Finance API

API backend para controle financeiro pessoal, desenvolvida como desafio técnico para o TCE-MG. A solução implementa cadastro e consulta de transacoes, visao consolidada mensal e auditoria de alteracoes, com foco em organizacao arquitetural e separacao de responsabilidades.

## Visao Geral

O sistema expoe endpoints para:

- CRUD de transacoes financeiras
- Consulta paginada com filtros por mes e categoria
- Dashboard mensal (saldo, receitas, despesas)
- Serie diaria para grafico
- Historico de alteracoes por transacao
- Listagem de categorias

O backend segue estilo arquitetural em camadas inspirado em Clean Architecture, usando casos de uso explicitos e abstracoes por interface.

## Arquitetura

### Estrutura da solucao

```text
src/
	Tce.Api/             # Camada de apresentacao (Minimal API)
	Tce.Application/     # Casos de uso, DTOs e contratos
	Tce.Domain/          # Entidades e regras de negocio
	Tce.Infrastructure/  # Persistencia (EF Core), queries (Dapper), seed e migrations
Tce.Tests/             # Testes unitarios (Domain e Application)
```

### Responsabilidades por camada

- Tce.Domain
	- Define entidades centrais (`Transaction`, `Category`, `User`, `TransactionHistory`) e enums de negocio.
	- Nao depende de frameworks.

- Tce.Application
	- Orquestra regras atraves de casos de uso (`Create`, `Update`, `Delete`, `GetById`, `GetPaged`).
	- Define interfaces de repositorio/consulta para inversao de dependencia.
	- Mapeia modelos de dominio para DTOs de resposta.

- Tce.Infrastructure
	- Implementa persistencia com EF Core (escrita e leitura paginada).
	- Implementa consultas analiticas com Dapper (summary, chart, history).
	- Configura mapeamentos do banco, migrations e seed inicial.

- Tce.Api
	- Define endpoints HTTP com Minimal API.
	- Configura DI, CORS, Swagger e inicializacao da base (migrate + seed).

### Padroes e principios aplicados

- Arquitetura em camadas com separacao de responsabilidades
- Use Case por funcionalidade
- Repository Pattern por contrato de persistencia
- CQRS leve: escrita via EF Core e leitura analitica via Dapper
- Inversao de dependencia via interfaces na camada Application

## Tecnologias Utilizadas

- .NET SDK: 10.0 (TargetFramework `net10.0`)
- ASP.NET Core Minimal API
- Entity Framework Core 10 (SQL Server)
- Dapper (consultas de leitura analitica)
- SQL Server 2022 (container)
- Swagger / OpenAPI (Swashbuckle)
- Docker e Docker Compose

### Testes

- xUnit
- Moq
- FluentAssertions
- AutoFixture + AutoMoq
- Bogus

## Fluxos Funcionais Principais

### 1) Criacao de transacao

1. Endpoint recebe `CreateTransactionDto`.
2. Use case valida existencia de categoria e usuario.
3. Entidade `Transaction` aplica regra de dominio (`amount > 0`).
4. Repositorio persiste a transacao.
5. Auditoria cria registro em `TransactionHistories` com `ChangeType.Created`.

### 2) Atualizacao e exclusao

- Update recupera entidade por id, aplica alteracao e grava historico `Updated`.
- Delete remove transacao e grava historico `Deleted`.

### 3) Consulta paginada com filtros

- Filtros opcionais: `month` (`yyyy-MM`) e `categoryId`.
- Ordenacao: `Date DESC`, depois `Id DESC`.
- Resultado padronizado: `PagedResult<T>` com `Data` e `Total`.

### 4) Dashboard e historico

- Summary mensal: receita, despesa e saldo (`income - expense`).
- Chart mensal: consolidado diario para visualizacao grafica.
- History: trilha de auditoria por transacao.

## Configuracoes de Aplicacao

### Dependency Injection

- Casos de uso registrados como `Scoped`
- Repositorios e query service registrados por interface

### CORS

- Politica `AllowAll` permite origem: `http://localhost:4200`
- Metodos e headers liberados

### Swagger

- Swagger UI habilitado na aplicacao
- URL principal: `/swagger`

### Banco na inicializacao da API

Na subida da aplicacao:

- Executa `Database.Migrate()`
- Executa seed (`DbInitializer.Seed`)
- Tenta novamente em caso de indisponibilidade inicial do banco (loop com retries)

## Como Executar o Projeto

### Pre-requisitos

- .NET SDK 10
- Docker Desktop (para SQL Server em container)
- Ferramenta EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

### Opcao A: Execucao com Docker Compose (API + SQL Server)

Na raiz do repositorio:

```bash
docker-compose up --build -d
```

Acessos:

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Opcao B: Execucao local da API (SQL Server no Docker)

1. Suba apenas o SQL Server:

```bash
docker-compose up -d sqlserver
```

2. Execute a API localmente:

```bash
dotnet run --project src/Tce.Api/Tce.Api.csproj
```

Observacao:

- O `appsettings.json` ja aponta para `Server=sqlserver,1433`, que funciona com Docker Compose na mesma rede dos containers.
- Para execucao local fora do container da API, ajuste temporariamente a connection string para `localhost,1433` ou passe por variavel de ambiente.

## Banco de Dados

### Migrations

Criar migration:

```bash
dotnet ef migrations add NomeDaMigration \
	--project src/Tce.Infrastructure/Tce.Infrastructure.csproj \
	--startup-project src/Tce.Api/Tce.Api.csproj
```

Aplicar migrations:

```bash
dotnet ef database update \
	--project src/Tce.Infrastructure/Tce.Infrastructure.csproj \
	--startup-project src/Tce.Api/Tce.Api.csproj
```

### Seed

O seed inicial cria:

- 1 usuario padrao (`alberto@example.com`)
- Categorias padrao (receita/despesa)
- 15 transacoes de exemplo
- Historico inicial de criacao para as transacoes

## Endpoints Principais

Base: `/api`

### Transacoes

- `POST /transactions`
	- Cria transacao
- `GET /transactions?page=1&limit=10&month=2026-04&categoryId={guid}`
	- Lista paginada com filtros opcionais
- `GET /transactions/{id}`
	- Busca por id
- `PUT /transactions/{id}`
	- Atualiza integralmente
- `PATCH /transactions/{id}`
	- Atualiza (mesma logica do PUT no estado atual)
- `DELETE /transactions/{id}`
	- Remove transacao

### Dashboard

- `GET /transactions/summary?month=2026-04`
	- Retorna `balance`, `totalIncome`, `totalExpense`
- `GET /transactions/chart?month=2026-04`
	- Retorna pontos diarios `{ day, value }`

### Auditoria

- `GET /transactions/{id}/history`
	- Retorna trilha de alteracoes da transacao

### Categorias

- `GET /categories`
	- Lista categorias disponiveis

## Decisoes Tecnicas

### Paginacao e ordenacao

- Paginacao por `page` e `limit`
- `limit` limitado entre 1 e 100
- Ordenacao deterministica por data e id

### Filtro temporal

- Filtro por mes no formato `yyyy-MM`
- Janela de consulta calculada entre primeiro dia do mes e primeiro dia do mes seguinte

### Estrategia de auditoria

- Auditoria no repositorio em cada operacao de escrita (create/update/delete)
- Persistencia de metadados em `TransactionHistories`
- Consulta dedicada para historico via Dapper

### Tratamento de erros

- Erros esperados de negocio em criacao retornam `400` com mensagem
- Erros nao tratados retornam `ProblemDetails`
- Not found em consulta por id retorna `404`

### Separacao de responsabilidades

- API nao conhece detalhes de persistencia
- Application define contratos; Infrastructure implementa
- Domain concentra regras essenciais da entidade

## Qualidade e Testes

Executar testes:

```bash
dotnet test
```

Escopo atual:

- Testes unitarios de dominio
- Testes unitarios de casos de uso
- Cenarios positivos e negativos para regras principais

## Lacunas Identificadas e Melhorias Recomendadas

Para evolucao de nivel produtivo:

- Logging estruturado (Serilog) com correlation id
- Middleware global de excecoes com padronizacao de erros
- Integrar `FluentValidation` no pipeline HTTP (hoje ha validator criado, mas sem integracao automatica)
- Autenticacao/autorizacao (JWT/OAuth2)
- Testes de integracao com banco efemero (Testcontainers)
- Observabilidade com OpenTelemetry (traces/metrics/logs)
- Health checks e readiness/liveness probes
- CI/CD com quality gates e analise estatica

## Autor

Alberto Leao
