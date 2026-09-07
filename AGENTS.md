# AGENTS.md — WhatsCRM

## Ordem de autoridade

Ao trabalhar neste repositório, siga esta prioridade:

1. requisito atual do usuário;
2. spec correspondente em `docs/specs/`;
3. `ARCHITECTURE.md`;
4. este arquivo;
5. código existente.

Não contradiga silenciosamente a arquitetura. Registre a decisão quando uma mudança for necessária.

## Fluxo obrigatório

Para qualquer funcionalidade: requisito → spec → análise → implementação completa → testes → build → correções → revisão. Antes de editar, execute `git status` e preserve alterações que não pertençam ao escopo.

## Escopo incremental

Implemente apenas a spec solicitada. Não antecipe WhatsApp, CRM, agenda, SignalR, automações, IA, pagamentos ou infraestrutura complexa sem uma spec aprovada.

## Multiempresa e segurança

- Entidades operacionais devem possuir `EmpresaId` direta ou indiretamente.
- Nunca aceite `EmpresaId` do frontend como autorização.
- Obtenha empresa e usuário do contexto autenticado/JWT.
- Filtre consultas no banco e teste isolamento entre tenants.
- Não exponha entities diretamente; use DTOs.
- Nunca versione secrets, tokens, senhas ou connection strings de produção.
- Não envie stack traces ao cliente.
- Use integração oficial WhatsApp Cloud API quando a spec correspondente chegar.
- Minimize dados; o produto não é prontuário eletrônico.

## Backend

- .NET 10, ASP.NET Core Web API, EF Core, Npgsql e PostgreSQL.
- Controllers coordenam HTTP; regras ficam em services.
- Leituras usam `AsNoTracking` quando apropriado.
- Filtre e pagine no PostgreSQL; evite N+1 e materialização prematura.
- Mudanças de banco exigem migration nova, revisão de FKs, constraints e índices.
- Use códigos HTTP e Problem Details adequados.

## Frontend

- React, TypeScript, Vite, React Router e Axios.
- Fluxos precisam de loading, erro e validação.
- Toda operação deve funcionar sem hover e ser responsiva em 375, 768 e 1366 px.
- O backend é a fonte definitiva para regras e cálculos.

## Harness

Antes de concluir, execute:

```text
dotnet restore
dotnet build
dotnet test
npm ci
npm run lint
npm run build
docker compose config
```

Nunca declare uma tarefa concluída com erros conhecidos. Não remova volumes persistentes sem autorização explícita.
