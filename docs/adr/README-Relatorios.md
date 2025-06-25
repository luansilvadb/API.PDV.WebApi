# Relatórios Operacionais – Sprint 06

## Arquitetura

- **Repositório Dapper:** [`RelatorioRepository`](../../infra/RelatorioRepository.cs) implementa consultas otimizadas multitenant.
- **Interface de domínio:** [`IRelatorioRepository`](../domain/IRelatorioRepository.cs) define contratos e DTOs.
- **Serviço de aplicação:** [`RelatorioService`](RelatorioService.cs) orquestra o acesso aos relatórios.
- **Testes:** [`RelatorioServiceTests`](../tests/RelatorioServiceTests.cs) cobre todos os fluxos.

## Relatórios Disponíveis

- **Vendas por período:** `ObterVendasPorPeriodoAsync(DateTime inicio, DateTime fim)`
- **Estoque atual:** `ObterEstoqueAtualAsync()`
- **Produtos mais vendidos:** `ObterProdutosMaisVendidosAsync(DateTime inicio, DateTime fim, int top)`

## Multitenancy

Todas as queries usam o schema ativo via `ISchemaContextAccessor.CurrentSchema`, garantindo isolamento por locatário.

## Como rodar os testes

1. Instale dependências de teste (xUnit, Moq).
2. Execute:
   ```
   dotnet test core/tests
   ```
3. Todos os métodos do serviço de relatórios são cobertos por testes unitários.

## Observações

- Para integração, injete `IRelatorioRepository` e `ISchemaContextAccessor` no container de DI.
- As queries são otimizadas para PostgreSQL, ajuste se necessário para outro SGBD.
- Para rastreabilidade, consulte este README e os comentários XML nos serviços.

---