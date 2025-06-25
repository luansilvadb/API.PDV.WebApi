# Modelagem do Banco – Sprint 08

## Visão Geral

O sistema utiliza PostgreSQL com multitenancy por schema. Cada locatário possui um schema próprio (`tenant_<slug>`), garantindo isolamento total de dados e operações.

---

## Estrutura de Schemas

- **Schema global:** armazena informações de tenants e configurações globais.
- **Schema por tenant:** armazena entidades de domínio (Produto, Estoque, Venda, etc).

---

## Principais Tabelas por Tenant

- `produto` (id, nome, preco, estoque, validade, criado_em)
- `estoque` (id, produto_id, quantidade, atualizado_em)
- `venda` (id, data, total, status)
- `item_venda` (id, venda_id, produto_id, quantidade, preco_unitario)
- `pagamento` (id, venda_id, tipo, valor, data)

---

## Script de Criação de Schema e Tabelas

```sql
-- Criação do schema do tenant
CREATE SCHEMA IF NOT EXISTS tenant_acme;

-- Tabela de produtos
CREATE TABLE tenant_acme.produto (
  id SERIAL PRIMARY KEY,
  nome VARCHAR(100) NOT NULL,
  preco NUMERIC(10,2) NOT NULL CHECK (preco >= 0),
  estoque INT NOT NULL DEFAULT 0,
  validade DATE,
  criado_em TIMESTAMP DEFAULT NOW()
);

-- Tabela de estoque
CREATE TABLE tenant_acme.estoque (
  id SERIAL PRIMARY KEY,
  produto_id INT REFERENCES tenant_acme.produto(id),
  quantidade INT NOT NULL,
  atualizado_em TIMESTAMP DEFAULT NOW()
);

-- Demais tabelas seguem padrão similar...
```

---

## Isolamento Multitenant

- Toda query é executada no contexto do schema ativo.
- O middleware identifica o tenant via header `X-Tenant` e ativa o schema correspondente.
- Migrations e seeds são aplicados por schema.

---

## Diagrama ER (simplificado)

```
[produto] 1---* [estoque]
[produto] 1---* [item_venda] *---1 [venda] 1---* [pagamento]
```

---

## Observações

- Scripts completos e diagramas detalhados devem ser mantidos em `/infra/scripts/` e `/docs/diagrams/`.
- Para novos tenants, execute o script de bootstrap do schema.