# Qualidade, Testes e Cobertura – Sprint 07

## Testes Unitários e de Integração
- Todos os domínios e serviços possuem testes unitários cobrindo cenários positivos e negativos.
- Testes de integração garantem operações entre entidades (ex: venda e baixa de estoque).

## Cobertura de Código
- Utiliza Coverlet com configuração em [`core/tests/coverlet.runsettings`](core/tests/coverlet.runsettings).
- Cobertura mínima recomendada: 80% no domínio.

## Benchmarks de Queries
- Benchmark de queries críticas com Dapper em [`core/tests/DapperBenchmarkTests.cs`](core/tests/DapperBenchmarkTests.cs).

## Logging e Métricas
- Serviço de logging implementado em [`core/application/LoggingService.cs`](core/application/LoggingService.cs) usando Microsoft.Extensions.Logging.

## Pipeline CI/CD
- Pipeline automatizado com GitHub Actions em [`github/workflows/ci.yml`](../.github/workflows/ci.yml) para build, testes e cobertura.

## Como Executar os Testes
```sh
dotnet test ./core/tests/API.PDV.Domain.Tests.csproj --settings ./core/tests/coverlet.runsettings --collect:"XPlat Code Coverage"
```

## Observações
- Benchmarks exigem banco configurado.
- Ajuste connection strings conforme ambiente.