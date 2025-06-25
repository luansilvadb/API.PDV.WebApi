# Onboarding – Sprint 08 (Hardening e Finalização)

## Visão Geral
Esta sprint consolidou segurança, resiliência, validação e documentação do backend PDV multitenant. Todas as APIs, regras de negócio e integrações agora seguem padrões de robustez e rastreabilidade.

---

## 1. Autenticação e Segurança

- **JWT obrigatório** para todas as rotas protegidas.
- **Roles**: `admin`, `user` (exemplo de uso no payload do token).
- **CORS restritivo**: apenas domínios autorizados.
- **Proteções**: HSTS, antiforgery, headers seguros.

### Exemplo de login e obtenção de token

```http
POST /api/auth/login
Content-Type: application/json

{
  "usuario": "admin@empresa.com",
  "senha": "senha123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

## 2. Validações de Entrada

- Todos os endpoints usam DTOs com validação automática.
- Erros de validação retornam HTTP 400 com detalhes.

### Exemplo de erro de validação

```http
POST /api/produtos
Content-Type: application/json

{
  "nome": "",
  "preco": -10
}
```

**Resposta:**
```json
{
  "errors": {
    "Nome": ["O campo Nome é obrigatório."],
    "Preco": ["O valor deve ser positivo."]
  }
}
```

---

## 3. Exemplos de Uso dos Endpoints

### Criar Produto

```http
POST /api/produtos
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Cerveja",
  "preco": 5.99,
  "estoque": 100
}
```

### Consultar Estoque

```http
GET /api/estoque
Authorization: Bearer {token}
```

---

## 4. Resiliência e Logging

- Serviços críticos usam retry/fallback (Polly).
- Falhas são logadas via Serilog/ILogger.
- Respostas de erro seguem padrão HTTP.

---

## 5. Multitenancy

- Cada tenant opera em schema isolado.
- Header `X-Tenant` obrigatório em todas as requisições.

---

## 6. Execução de Testes

- Testes automatizados cobrem validação, segurança, resiliência e integração.
- Cobertura mínima: 80%.

---

## 7. Documentação Swagger

- Disponível em `/swagger` no ambiente de desenvolvimento.
- Recomenda-se exportar o arquivo para versionamento.

---

## 8. Contato e Suporte

- Consulte os ADRs em `/docs/adr/` para decisões técnicas.
- Dúvidas: equipe-dev@empresa.com