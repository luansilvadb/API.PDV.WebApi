# Sprint 02 – Multitenancy

## Objetivo
Implementar a infraestrutura de multitenancy por schema.

## Tarefas
- Definir estratégia de troca de schema por request
- Criar middleware/serviço para identificar tenant
- Criar migrations por schema
- Implementar camada de abstração para conexão tenant-aware
- Criar repositório de tenants e schema bootstrap

## Critérios de Aceite
- Conexão ativa com schema correto por tenant
- Capacidade de registrar novo locatário
- Cada locatário opera com dados isolados
