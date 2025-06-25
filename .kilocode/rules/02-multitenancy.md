# Estratégia de Multitenancy

## Tipo: Por Schema

Cada locatário (empresa/negócio) possui seu próprio schema no banco de dados, garantindo:

- **Isolamento completo dos dados**
- **Autonomia em configurações e módulos**
- **Backup e restauração independentes**
- **Facilidade de manutenção e auditoria**
- **Segurança na troca de schema por sessão/autenticação**

## Convenções
- Nome do schema: `tenant_<slug>`
- Configurações específicas também são armazenadas no schema
- Toda operação executada dentro do contexto de schema ativo
- Migrations e seeds devem rodar por schema
