# TCE Finance API

Projeto desenvolvido como solução para o teste técnico de Desenvolvedor do Tribunal de Contas do Estado de Minas Gerais (TCE-MG).

---

## Visão Geral

Aplicação para gerenciamento de transações financeiras, com funcionalidades de controle de entradas e saídas, dashboard mensal, histórico de alterações e análise por categoria.

O projeto foi construído com foco em qualidade de código, organização arquitetural e boas práticas de desenvolvimento.

---

## Arquitetura

A solução foi estruturada utilizando uma abordagem baseada em Clean Architecture, com separação clara de responsabilidades entre camadas.

Estrutura do projeto:

```
src/
 ├── Tce.Api              # Camada de apresentação (Minimal API)
 ├── Tce.Application      # Casos de uso e orquestração
 ├── Tce.Domain           # Regras de negócio
 ├── Tce.Infrastructure   # Persistência e acesso a dados
```

### Princípios aplicados

* SOLID
* Separation of Concerns
* Baixo acoplamento e alta coesão
* Arquitetura em camadas
* CQRS (Command Query Responsibility Segregation) de forma simplificada

---

## Tecnologias Utilizadas

### Backend

* .NET 10
* Minimal API
* Entity Framework Core (operações de escrita)
* Dapper (operações de leitura)

### Banco de Dados

* SQL Server

### Testes

* xUnit
* Moq
* FluentAssertions
* AutoFixture com AutoMoq
* Bogus (geração de dados)

### Infraestrutura

* Docker
* Docker Compose

---

## Decisões Técnicas

### Separação entre leitura e escrita (CQRS leve)

* Escrita realizada via Entity Framework Core
* Leitura otimizada utilizando Dapper

Essa abordagem permite melhor performance nas consultas e mantém consistência nas operações transacionais.

---

### Auditoria de transações

Foi implementado um mecanismo de auditoria automática no nível de repositório, registrando eventos de:

* Criação
* Atualização
* Exclusão

O histórico é persistido na tabela `TransactionHistories` e pode ser consultado via endpoint específico.

---

### Isolamento do domínio

A camada de domínio não possui dependência de frameworks externos, garantindo maior testabilidade e independência.

---

## Execução do Projeto

### Pré-requisitos

* Docker instalado

---

### Execução com Docker

Na raiz do projeto, execute:

```
docker-compose up --build
```

Após a inicialização:

```
http://localhost:5000/swagger
```

---

## Banco de Dados

* SQL Server executado em container
* Migrations aplicadas automaticamente na inicialização
* Seed de dados inicial incluído

O seed cria:

* Usuário padrão
* Categorias de entrada e saída
* Transações de exemplo
* Histórico inicial de transações

---

## Endpoints

### Transações

* GET /api/transactions
* POST /api/transactions
* GET /api/transactions/{id}
* PATCH /api/transactions/{id}
* DELETE /api/transactions/{id}

---

### Dashboard

* GET /api/transactions/summary
* GET /api/transactions/chart

---

### Histórico

* GET /api/transactions/{id}/history

Retorna o histórico de alterações da transação.

---

### Categorias

* GET /api/categories

---

## Testes

Para executar os testes:

```
dotnet test
```

Cobertura implementada:

* Regras de negócio (Domain)
* Casos de uso (Application)
* Testes positivos e negativos
* Uso de mocks e geração automática de dados

---

## Diferenciais Implementados

* Arquitetura limpa e organizada
* Separação clara entre camadas
* Implementação de CQRS leve
* Auditoria automática de transações
* Uso de Dapper para performance em consultas
* Dockerização completa da aplicação
* Seed de dados automatizado
* Testes unitários com padrão AAA
* Baixo acoplamento entre componentes

---

## Melhorias Possíveis

* Implementação de autenticação e autorização (JWT)
* Testes de integração
* Pipeline CI/CD
* Observabilidade (logs estruturados e métricas)
* Cache para consultas frequentes

---

## Autor

Alberto Leão

---

## Considerações Finais

O projeto foi desenvolvido com foco em qualidade, organização e aderência a boas práticas modernas de desenvolvimento, simulando um ambiente real de produção.
