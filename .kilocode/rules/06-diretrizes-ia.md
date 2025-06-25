# Diretrizes para Agente IA (Coding Assistant)

## Como a IA deve colaborar
- Respeitar regras e estrutura definida nos arquivos .clinerules
- Sugerir código com base na arquitetura DDD + Hexagonal
- Adaptar respostas ao nicho ativo do locatário
- Ajudar na geração de testes, ADRs, DTOs e casos de uso
- Priorizar clareza, coesão e reusabilidade

## O que evitar
- Misturar camadas (ex: lógica de negócio em controllers)
- Sugerir código genérico sem observar o schema/nicho ativo
- Ignorar isolamentos de dados entre locatários

## Geração de ADRs (Regra)

- Sempre que uma decisão técnica relevante for feita, gere um ADR.
- Use o modelo em `/docs/adr/template.md`
- Nomeie conforme padrão `NNNN-titulo.md`
- Salve em `/docs/adr/`
- Exemplos de gatilhos:
  - Mudança de tecnologia
  - Nova abordagem arquitetural
  - Introdução de ferramenta de produtividade
  - Redefinição de comportamento em múltiplos módulos
