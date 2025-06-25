# Arquitetura e Organização de Código

## Padrão: Hexagonal com DDD

### Camadas
- **Domínio (`/core/domain`)**: entidades, agregados, regras de negócio
- **Aplicação (`/core/application`)**: casos de uso e orquestração
- **Infraestrutura (`/infra`)**: banco de dados, APIs externas
- **Interface (`/ui`)**: frontend e controle de fluxo
- **Schemas (`/schemas`)**: definem estrutura por locatário

### Regras
- Use Repositórios para abstração de dados
- Prefira composição sobre herança
- Separar os módulos por contexto de negócio
- Cada locatário ativa apenas os módulos que utiliza
