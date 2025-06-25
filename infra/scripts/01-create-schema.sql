-- Script de criação do banco PDV Multitenant por schema
-- Cada tenant terá seu próprio schema: tenant_<slug>

-- Exemplo de criação de schema para um tenant
CREATE SCHEMA IF NOT EXISTS tenant_demo;

-- Tabela de controle de tenants (no schema público)
CREATE TABLE IF NOT EXISTS public.tenants (
    id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    slug VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Tabelas do domínio (dentro do schema do tenant)
-- Substitua 'tenant_demo' pelo schema do tenant no provisionamento

CREATE TABLE IF NOT EXISTS tenant_demo.empresa (
    id UUID PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cnpj VARCHAR(18) NOT NULL,
    endereco VARCHAR(200),
    criado_em TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS tenant_demo.produto (
    id UUID PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    preco NUMERIC(18,2) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS tenant_demo.estoque (
    id UUID PRIMARY KEY,
    produto_id UUID NOT NULL REFERENCES tenant_demo.produto(id),
    quantidade NUMERIC(18,3) NOT NULL,
    lote VARCHAR(50),
    atualizado_em TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS tenant_demo.venda (
    id UUID PRIMARY KEY,
    empresa_id UUID NOT NULL REFERENCES tenant_demo.empresa(id),
    data_hora TIMESTAMP NOT NULL,
    total NUMERIC(18,2) NOT NULL DEFAULT 0,
    troco NUMERIC(18,2) NOT NULL DEFAULT 0,
    finalizada BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS tenant_demo.item_venda (
    id UUID PRIMARY KEY,
    venda_id UUID NOT NULL REFERENCES tenant_demo.venda(id),
    produto_id UUID NOT NULL REFERENCES tenant_demo.produto(id),
    descricao_produto VARCHAR(100) NOT NULL,
    quantidade NUMERIC(18,3) NOT NULL,
    preco_unitario NUMERIC(18,2) NOT NULL,
    total NUMERIC(18,2) GENERATED ALWAYS AS (quantidade * preco_unitario) STORED
);

CREATE TABLE IF NOT EXISTS tenant_demo.pagamento (
    id UUID PRIMARY KEY,
    venda_id UUID NOT NULL REFERENCES tenant_demo.venda(id),
    valor NUMERIC(18,2) NOT NULL,
    forma_pagamento VARCHAR(50) NOT NULL,
    data_hora TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Índices e constraints adicionais podem ser adicionados conforme necessidade