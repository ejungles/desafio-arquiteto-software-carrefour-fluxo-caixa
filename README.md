# Fluxo de Caixa - Sistema de Lançamentos e Consolidação Financeira

## 📌 Visão Geral do Projeto

O sistema Fluxo de Caixa é uma solução distribuída para gestão financeira que permite:
- Registro de lançamentos financeiros (débitos e créditos)
- Consolidação diária de saldos
- Geração de relatórios periódicos
- Processamento assíncrono de transações

**Arquitetura**: Microsserviços com comunicação via RabbitMQ  
**Banco de Dados**: SQL Server (transacional) + MongoDB (consolidado)  
**Cache**: Redis  
**Filas**: RabbitMQ com DLQ (Dead Letter Queue)

## 🏗️ Diagrama de Arquitetura

```mermaid
graph TD
    A[Lancamentos API] -->|Eventos| B[RabbitMQ]
    B --> C[Dados Consolidados API]
    A --> D[SQL Server]
    C --> E[MongoDB]
    C --> F[Redis]
    A --> F
    G[Clientes] --> A
    G --> C
```

## 📂 Estrutura do Projeto

```
FluxoCaixa/
├── API/
│   ├── Lancamentos.API/          # Serviço de lançamentos
│   └── DadosConsolidados.API/    # Serviço de consolidação
├── Application/                  # Lógica de negócio
├── Domain/                       # Entidades e interfaces
├── Infra/                        # Implementações de infra
├── Shared/                       # DTOs e componentes compartilhados
└── Tests/                        # Testes unitários e integração
```

## 🛠️ Tecnologias Principais

| Tecnologia | Uso |
|------------|-----|
| .NET 6 | Framework principal |
| SQL Server | Dados transacionais |
| MongoDB | Dados consolidados |
| RabbitMQ | Mensageria entre serviços |
| Redis | Cache de consultas |
| Docker | Containerização |
| MediatR | Padrão CQRS |
| FluentValidation | Validações |
| AutoMapper | Mapeamento de objetos |
| Polly | Resiliência |

## 🗃️ Modelagem de Dados

### SQL Server (Lançamentos)

```mermaid
erDiagram
    Lancamentos {
        bigint Id PK
        varchar255 Descricao
        int TipoLancamentoId FK
        decimal18_2 Valor
        date DataLancamento
        bit ProcessadoConsolidacao
        datetime2 DataCriacao
    }

    TiposLancamento {
        int Id PK
        varchar50 Descricao
        char1 Natureza
        bit Ativo
        datetime2 DataCriacao
    }

    TiposLancamento ||--o{ Lancamentos : "TipoLancamentoId"

```

### MongoDB (Consolidados)

```json
{
  "_id": ObjectId("507f1f77bcf86cd799439011"),
  "Data": ISODate("2025-05-02T00:00:00Z"),
  "TotalCreditos": NumberDecimal("1500.00"),
  "TotalDebitos": NumberDecimal("750.50"),
  "Saldo": NumberDecimal("749.50")
}
```

## 🚀 Como Executar o Projeto

### Pré-requisitos
- Docker e Docker Compose
- .NET 8 SDK

### Passo a passo

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/fluxo-caixa.git
cd fluxo-caixa
```

2. Execute os containers:
```bash
docker-compose up -d
```

3. Acesse os serviços:
- Lancamentos: http://localhost:5000/swagger
- Dados Consolidados: http://localhost:6000/swagger
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- MongoDB: mongodb://localhost:27017
- SQL Server: localhost,1433 (sa/YourStrong!Passw0rd)

## 🔍 Endpoints Principais

### Serviço de Lançamentos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/lancamentos` | Cria novo lançamento |
| GET | `/api/lancamentos/{id}` | Obtém lançamento por ID |

### Serviço de Dados Consolidados

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/consolidados/diario/{data}` | Obtém saldo diário |
| POST | `/api/consolidados/relatorio` | Gera relatório periódico |
| POST | `/api/consolidados/reprocessar` | Reprocessa saldos |

## 📊 Exemplos de Requisições

### Criar Lançamento
```bash
curl -X POST "http://localhost:5000/api/lancamentos" \
-H "Content-Type: application/json" \
-d '{
    "descricao": "Venda produto X",
    "tipoLancamentoId": 1,
    "valor": 150.50,
    "data": "2024-04-30"
}'
```

### Obter Saldo Diário
```bash
curl -X GET "http://localhost:6000/api/consolidados/diario/2024-04-30" \
-H "Authorization: Bearer {token}"
```

### Gerar Relatório
```bash
curl -X POST "http://localhost:6000/api/consolidados/relatorio" \
-H "Content-Type: application/json" \
-d '{
    "dataInicio": "2024-04-01",
    "dataFim": "2024-04-30",
    "formato": "json"
}'
```

## ⚙️ Configurações Avançadas

### Variáveis de Ambiente

**Lancamentos API**:
```ini
ConnectionStrings__SqlServerConnection=Server=sqlserver;Database=FluxoCaixaDB;User=sa;Password=YourStrong!Passw0rd;
RabbitMQ__Host=rabbitmq
RabbitMQ__Username=guest
RabbitMQ__Password=guest
Redis__ConnectionString=redis:6379
```

**Dados Consolidados API**:
```ini
MongoDB__ConnectionString=mongodb://root:example@mongodb:27017
MongoDB__DatabaseName=FluxoCaixaConsolidado
```

## 🧪 Testes

### Testes Unitários
```bash
dotnet test
```

### Testes de Integração
```bash
docker-compose -f docker-compose.test.yml up --build --abort-on-container-exit
```

## 🔄 Fluxo de Processamento

```mermaid
sequenceDiagram
    participant Cliente
    participant LancamentosAPI
    participant RabbitMQ
    participant DadosConsolidadosAPI
    
    Cliente->>LancamentosAPI: POST /api/lancamentos
    LancamentosAPI->>SQLServer: Persiste lançamento
    LancamentosAPI->>RabbitMQ: Publica evento
    RabbitMQ->>DadosConsolidadosAPI: Consome evento
    DadosConsolidadosAPI->>MongoDB: Atualiza consolidado
    DadosConsolidadosAPI->>Redis: Atualiza cache
```

## 🛡️ Estratégias de Resiliência

1. **Retry Pattern**: Tentativas de reconexão com Polly
2. **Circuit Breaker**: Proteção contra falhas em cascata
3. **Dead Letter Queue**: Tratamento de mensagens problemáticas
4. **Health Checks**: Monitoramento contínuo
5. **Cache**: Redis para consultas frequentes

## 👤 Autor

Desenvolvido por **Eliel Jungles**  
📧 Email: [eliel.jungles@icloud.com](mailto:eliel.jungles@icloud.com)  
💼 LinkedIn: [linkedin.com/in/eliel-jungles](https://www.linkedin.com/in/eliel-jungles/)  
💻 GitHub: [github.com/ejungles](https://github.com/ejungles)

## 📄 Licença

Distribuído sob a licença MIT. Veja `LICENSE` para mais informações.
