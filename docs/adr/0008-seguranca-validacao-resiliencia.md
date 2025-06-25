# ADR 0008 – Segurança, Validação e Resiliência (Sprint 08)

## Contexto

A Sprint 08 teve como foco hardening do backend PDV multitenant, cobrindo autenticação, autorização, validação de entrada, resiliência e logging.

---

## Decisão

- **Autenticação JWT** obrigatória para todas as rotas protegidas.
- **Autorização por roles** (`admin`, `user`) via claims no token.
- **CORS restritivo**: apenas domínios autorizados.
- **Proteções**: HSTS, antiforgery, headers seguros.
- **Validação de entrada**: DTOs com atributos, ModelState, respostas 400 detalhadas.
- **Resiliência**: Polly para retry/fallback em serviços críticos.
- **Logging estruturado**: Serilog/ILogger em toda a aplicação.
- **Multitenancy**: middleware ativa schema por header `X-Tenant`.

---

## Consequências

- APIs mais seguras e robustas.
- Facilidade de auditoria e troubleshooting.
- Isolamento total de dados por tenant.
- Onboarding facilitado por documentação e exemplos.

---

## Alternativas Consideradas

- Autenticação via API Key (rejeitada por menor segurança).
- Retry manual sem Polly (rejeitada por menor padronização).
- Validação apenas no domínio (rejeitada por UX ruim na API).

---

## Referências

- [`docs/README-Onboarding-Sprint08.md`](../README-Onboarding-Sprint08.md)
- [`docs/modelagem-banco-sprint08.md`](../modelagem-banco-sprint08.md)
- [`docs/swagger-sprint08.json`](../swagger-sprint08.json)