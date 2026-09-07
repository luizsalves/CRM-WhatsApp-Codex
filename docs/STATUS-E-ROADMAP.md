# WhatsCRM — Status do Projeto e Roadmap

> Documento consolidado do que já foi criado, do que ainda falta e da sequência recomendada de desenvolvimento.

## 1. Identificação

| Item | Valor |
| --- | --- |
| Produto | WhatsCRM |
| Repositório | `luizsalves/CRM-WhatsApp-Codex` |
| Branch principal | `main` |
| Diretório local | `C:\Projetos\CRM-WhatsApp-Codex` |
| Data desta revisão | 7 de setembro de 2026 |
| Fase atual | Fundação do SaaS |
| Spec implementada | SPEC-001 — Autenticação e Multiempresa |

## 2. Legenda de status

| Símbolo | Significado |
| --- | --- |
| ✅ | Implementado e validado no escopo informado |
| 🟡 | Parcial, preparado ou ainda dependente de validação integrada |
| ⬜ | Planejado e ainda não implementado |
| ⛔ | Deliberadamente fora do escopo atual |

## 3. Resumo executivo

O repositório contém uma fundação executável para um SaaS multiempresa com backend .NET 10, frontend React, PostgreSQL, autenticação JWT, criação da primeira empresa, primeiro administrador, proteção de rotas e isolamento de tenant no EF Core.

O produto completo ainda não está pronto para operação comercial. Contatos, integração oficial com WhatsApp, conversas, CRM, agenda, tarefas, pós-venda, dashboard operacional, PWA e verticalização continuam no roadmap.

| Área | Estado | Observação |
| --- | --- | --- |
| Arquitetura e regras do repositório | ✅ | Documentadas em `ARCHITECTURE.md` e `AGENTS.md` |
| SPEC-001 | ✅ | Escrita e implementada |
| Backend base | ✅ | Compila sem erros ou avisos |
| Autenticação JWT | ✅ | Bootstrap, login e identidade atual |
| Isolamento multiempresa | ✅ | Filtro de leitura e guarda de escrita testados |
| Migration inicial | ✅ | `empresas` e `usuarios` |
| Frontend base | ✅ | Login, primeiro acesso, rota protegida e layout |
| Responsividade do login | ✅ | Inspeção visual em desktop e 375 px |
| Docker Compose | 🟡 | Configuração validada; execução integrada completa ainda pendente |
| PostgreSQL real | 🟡 | Estrutura pronta; migration ainda deve ser exercitada em uma instância real |
| WhatsApp e robô | ⬜ | Não implementados |
| CRM e módulos operacionais | ⬜ | Não implementados |
| CI/CD e produção | ⬜ | Não implementados |

## 4. O que já foi criado

### 4.1 Documentação e governança

- `README.md` com execução via Docker, desenvolvimento local e comandos de validação;
- `ARCHITECTURE.md` como fonte de verdade arquitetural;
- `AGENTS.md` com ordem de autoridade, regras de segurança e Definition of Done;
- `docs/specs/README.md` com o índice das specs;
- `docs/specs/SPEC-001-auth-multitenant.md` com escopo, regras, endpoints, UX, segurança, critérios de aceite e testes;
- `.editorconfig` para convenções básicas;
- `global.json` fixando a família do SDK .NET 10;
- `NuGet.Config` apontando para a fonte oficial do NuGet;
- manifesto local do `dotnet-ef` em `dotnet-tools.json`.

### 4.2 Backend .NET 10

Estrutura criada em `backend/WhatsCrm.Api`:

- ASP.NET Core Web API com Controllers;
- Entity Framework Core com Npgsql;
- PostgreSQL como banco de produção/desenvolvimento integrado;
- Swagger/OpenAPI com autenticação Bearer;
- autenticação e autorização JWT;
- CORS configurável por allowlist;
- rate limit para login e bootstrap;
- Problem Details e tratamento centralizado de exceções;
- endpoint simples de health check;
- aplicação opcional de migrations na inicialização;
- separação entre Controllers, Services, Interfaces, DTOs, Entities, Data, Configurations, Middlewares e Options.

#### Entidades implementadas

##### `Empresa`

- `Id`;
- `Nome`;
- `NomeFantasia`;
- `Cnpj`;
- `TipoNegocio`;
- `Ativo`;
- `CreatedAt`;
- `UpdatedAt`.

Tipos de negócio iniciais:

- `Vendas`;
- `Clinica`;
- `Servicos`;
- `Estetica`;
- `Outro`.

##### `Usuario`

- `Id`;
- `EmpresaId`;
- `Nome`;
- `Email`;
- `EmailNormalizado`;
- `PasswordHash`;
- `Perfil`;
- `Ativo`;
- `CreatedAt`;
- `UpdatedAt`.

Perfis iniciais:

- `Administrador`;
- `Gestor`;
- `Profissional`;
- `Atendente`.

#### Autenticação implementada

- verificação de disponibilidade da configuração inicial;
- criação transacional da primeira empresa e do primeiro administrador;
- proteção contra bootstrap repetido;
- transação serializável no PostgreSQL para reduzir risco de bootstrap concorrente;
- política de senha com 12 ou mais caracteres, maiúscula, minúscula e número;
- hash por `PasswordHasher<Usuario>` do ASP.NET Core;
- login com mensagem genérica para credenciais inválidas;
- bloqueio de login para usuário ou empresa inativos;
- rehash automático quando o algoritmo indicar necessidade;
- emissão de JWT HMAC SHA-256;
- validação de emissor, audiência, assinatura e expiração;
- claims de usuário, empresa, nome, e-mail e role;
- segredo JWT obrigatório por configuração externa e com no mínimo 32 caracteres.

#### Multiempresa implementado

- `EmpresaId` de autorização obtido da claim validada `empresa_id`;
- `UsuarioId` obtido da claim padrão `sub`;
- filtro global de leitura em `Empresa` e `Usuario`;
- preenchimento seguro de `EmpresaId` em novas entidades quando aplicável;
- bloqueio de gravação para entidade pertencente a outro tenant;
- bloqueio de alteração do `EmpresaId` de entidade existente;
- consultas globais sem filtro limitadas aos fluxos internos e revisados de bootstrap/login.

#### Endpoints disponíveis

| Método | Endpoint | Autenticação | Estado |
| --- | --- | --- | --- |
| GET | `/health` | Não | ✅ |
| GET | `/api/auth/bootstrap-status` | Não | ✅ |
| POST | `/api/auth/bootstrap` | Não; apenas instalação vazia | ✅ |
| POST | `/api/auth/login` | Não | ✅ |
| GET | `/api/auth/me` | Bearer JWT | ✅ |

#### Banco e migration

Migration criada:

```text
InitialAuthMultitenant
```

Ela cria:

- tabela `empresas`;
- tabela `usuarios`;
- PKs UUID;
- FK restritiva de usuário para empresa;
- índice único de CNPJ quando informado;
- índice único de e-mail normalizado;
- índice composto de empresa e perfil.

### 4.3 Testes automatizados do backend

Projeto criado em `backend/WhatsCrm.Api.Tests`.

Cobertura atual:

- senha forte aceita;
- senha curta rejeitada;
- senha sem maiúscula rejeitada;
- senha sem minúscula rejeitada;
- senha sem número rejeitada;
- filtro EF retorna somente usuários da empresa autenticada;
- gravação de entidade de outro tenant é bloqueada;
- JWT contém `sub`, `empresa_id` e `role` esperados.

Resultado registrado:

```text
8 testes aprovados
0 testes com falha
```

### 4.4 Frontend React

Estrutura criada em `frontend` com:

- React e TypeScript;
- Vite;
- React Router;
- Axios;
- ESLint;
- manifest web básico;
- Nginx para entrega do build e proxy da API.

#### Fluxos implementados

- tela de login;
- consulta da disponibilidade do primeiro acesso;
- tela de cadastro da primeira empresa e administrador;
- validações HTML essenciais;
- exibição de loading durante autenticação;
- exibição segura de erros da API;
- armazenamento da sessão inicial;
- interceptor Axios para enviar Bearer token;
- validação da sessão em `/api/auth/me`;
- limpeza da sessão em token inválido;
- logout;
- rota pública e rota protegida;
- layout autenticado responsivo;
- dashboard informativo da fundação;
- módulos futuros exibidos como indisponíveis, sem simular funções inexistentes;
- menu inferior adaptado para celular.

#### Telas disponíveis

| Rota | Tela | Estado |
| --- | --- | --- |
| `/login` | Login | ✅ |
| `/primeiro-acesso` | Primeira empresa e administrador | ✅ |
| `/` | Dashboard protegido da fundação | ✅ |

### 4.5 Infraestrutura

- `docker-compose.yml` com PostgreSQL, backend e frontend;
- volume nomeado `postgres_data`;
- health check do PostgreSQL;
- migration automática configurável no backend;
- Dockerfile multi-stage para a API;
- Dockerfile multi-stage para o frontend;
- Nginx servindo a SPA e encaminhando `/api` ao backend;
- `.env.example` sem credenciais reais;
- `.dockerignore` e `.gitignore`;
- portas previstas:
  - aplicação: `8080`;
  - API/Swagger: `8081`;
  - PostgreSQL apenas na rede interna do Compose.

### 4.6 Git e GitHub

- repositório Git inicializado na branch `main`;
- remoto `origin` configurado;
- fundação enviada ao GitHub;
- commit inicial da fundação: `3d2d0e4`;
- repositório: <https://github.com/luizsalves/CRM-WhatsApp-Codex>.

## 5. Validações já executadas

| Validação | Resultado |
| --- | --- |
| `dotnet restore` | ✅ Sucesso |
| `dotnet build` | ✅ Sucesso, 0 erros e 0 avisos |
| `dotnet test` | ✅ 8 aprovados, 0 falhas |
| `npm run lint` | ✅ Sucesso, sem avisos finais |
| `npm run build` | ✅ Sucesso |
| `docker compose config` | ✅ Configuração válida |
| Inspeção visual desktop | ✅ Login verificado |
| Inspeção visual em 375 px | ✅ Login verificado |
| Verificação de secrets evidentes | ✅ Nenhum secret real encontrado |
| Sincronização local/remoto | ✅ `main` acompanha `origin/main` |

## 6. O que ainda falta validar na fundação

Os itens abaixo não anulam os builds e testes já concluídos, mas precisam ser executados antes de considerar a fundação pronta para produção:

- [ ] executar `docker compose up -d --build` em ambiente limpo;
- [ ] aplicar a migration em PostgreSQL real;
- [ ] executar o bootstrap pela interface contra PostgreSQL real;
- [ ] testar logout e novo login ponta a ponta;
- [ ] verificar token expirado, adulterado e com emissor/audiência incorretos;
- [ ] testar duas empresas reais quando existir fluxo seguro de provisionamento adicional;
- [ ] testar corrida concorrente do bootstrap contra PostgreSQL;
- [ ] validar dashboard autenticado visualmente em 375, 768 e 1366 px;
- [ ] criar testes HTTP de integração para os endpoints de autenticação;
- [ ] revisar e atualizar a versão principal do ecossistema ESLint, pois o instalador atual informa fim de suporte da linha 9;
- [ ] configurar CI para repetir builds e testes em cada push/PR;
- [ ] definir ambiente de homologação, domínio, TLS e secret manager;
- [ ] definir backup e restauração do PostgreSQL;
- [ ] adicionar observabilidade de produção.

## 7. Limitações atuais importantes

### Primeira empresa

O sistema cadastra apenas a primeira empresa por meio do bootstrap. Depois que o primeiro usuário existe, o bootstrap é bloqueado. Ainda não há tela ou endpoint administrativo para provisionar uma segunda empresa.

### Usuários

Somente o primeiro administrador é criado. Convites, criação de equipe, alteração de roles, bloqueio e reativação pertencem à SPEC-002.

### Sessão

Ainda não existem refresh token, revogação centralizada, recuperação de senha ou MFA. O token inicial é armazenado no navegador; essa estratégia deve ser endurecida antes da produção.

### WhatsApp e robô

Não existe integração com a Meta, webhook, número conectado, envio, recebimento, conversa, caixa de entrada ou automação. A interface atual não fala com clientes.

### Operação comercial

Não existem contatos, tags, negócios, funil, agenda, tarefas, pós-venda ou métricas reais. O dashboard atual mostra somente o estado da fundação.

## 8. Tudo o que ainda falta criar

### Segurança e administração

- gestão de usuários;
- convites e ativação;
- permissões e policies por role;
- provisionamento de empresas adicionais;
- recuperação e troca de senha;
- refresh token e revogação;
- MFA opcional;
- trilha de auditoria;
- política de retenção e privacidade;
- testes de autorização por endpoint.

### Relacionamento

- contatos;
- tags e relacionamento contato-tag;
- busca, filtros, paginação e deduplicação;
- responsável pelo contato;
- histórico de relacionamento;
- importação futura, somente após spec.

### WhatsApp

- cadastro seguro de contas WhatsApp por empresa;
- integração oficial WhatsApp Business Platform Cloud API;
- secrets somente no backend/secret manager;
- verificação de webhook;
- validação de assinatura `X-Hub-Signature-256`;
- persistência idempotente de eventos;
- mapeamento de conta WhatsApp para empresa;
- envio e recebimento de mensagens;
- status de envio, entrega e leitura;
- tratamento de mídia;
- templates aprovados;
- janela de atendimento;
- opt-in e conformidade;
- observabilidade, retry e dead-letter/reprocessamento controlado;
- teste com número de sandbox antes de qualquer cliente real.

### Conversas e atendimento

- caixa de entrada;
- lista de conversas;
- histórico de mensagens;
- não lidas;
- responsável;
- status aberta, pendente e resolvida;
- atualização em tempo real via SignalR;
- envio de resposta pela interface;
- transferência para atendimento humano;
- regras do robô/automação em spec própria.

### CRM

- funis configuráveis;
- etapas;
- Kanban;
- negócios;
- itens do negócio;
- cálculo definitivo no backend;
- status `Aberto`, `Ganho` e `Perdido` separado da etapa visual;
- fechamento e motivo de perda;
- valor do pipeline;
- vínculo entre contato, responsável e negócio.

### Agenda

- compromissos;
- tipos e status;
- visualização de calendário;
- filtros por responsável/profissional;
- vínculo com contato e negócio;
- verificação de conflitos;
- reagendamento com histórico e novo registro;
- experiência mobile.

### Pós-venda e tarefas

- ações pós-venda;
- retornos;
- pesquisa de satisfação;
- reativação;
- acompanhamento;
- conclusão manual no MVP;
- tarefas gerais;
- follow-up;
- atrasos e lembretes;
- automação futura somente após nova spec.

### Dashboard, PWA e verticais

- métricas reais;
- filtros de período e responsável;
- indicadores comerciais;
- indicadores específicos de clínica sem prontuário;
- service worker e estratégia offline;
- instalação PWA;
- notificações, somente após definição de permissão e privacidade;
- nomenclatura por vertical;
- feature flags/configuração de módulos;
- validação completa em desktop, tablet e celular.

### Plataforma e produção

- testes de integração com PostgreSQL;
- testes end-to-end;
- pipeline de CI/CD;
- análise de dependências e segurança;
- ambientes de desenvolvimento, homologação e produção;
- TLS, domínio e proxy de produção;
- secret manager;
- logs estruturados e correlação;
- métricas, tracing e alertas;
- backup e restauração;
- política de migrations em produção;
- runbooks operacionais;
- política de versionamento e releases.

## 9. Roadmap das specs

Este roadmap orienta a ordem, mas não substitui a escrita completa de cada spec. Toda spec futura deve definir objetivo, problema, escopo, fora de escopo, atores, entidades, campos, relacionamentos, regras, permissões, endpoints, backend, frontend, UX, segurança, performance, erros, critérios de aceite e testes antes da implementação.

| Ordem | Spec | Status | Dependências principais | Resultado esperado |
| --- | --- | --- | --- | --- |
| 001 | Auth e multiempresa | ✅ Implementada | Fundação | Empresa inicial, admin, JWT e isolamento |
| 002 | Usuários e permissões | ⬜ Próxima | SPEC-001 | Equipe e autorização por role |
| 003 | Contatos e relacionamento | ⬜ Planejada | SPEC-001/002 | Contatos, tags e segmentação |
| 004 | Integração WhatsApp | ⬜ Planejada | SPEC-001/003 | Conta Cloud API e webhook seguro |
| 005 | Conversas e mensagens | ⬜ Planejada | SPEC-003/004 | Caixa de entrada e envio/recebimento |
| 006 | CRM, negócios e vendas | ⬜ Planejada | SPEC-002/003 | Funil, Kanban, negócios e itens |
| 007 | Agenda e compromissos | ⬜ Planejada | SPEC-002/003/006 | Calendário, conflitos e reagendamento |
| 008 | Pós-venda | ⬜ Planejada | SPEC-003/006 | Retorno, reativação e acompanhamento |
| 009 | Tarefas e follow-up | ⬜ Planejada | SPEC-002/003/006 | Atividades, responsáveis e prazos |
| 010 | Dashboard | ⬜ Planejada | SPEC-005 a 009 | Métricas operacionais e comerciais |
| 011 | Mobile/PWA | ⬜ Planejada | Fluxos anteriores estáveis | Instalação e experiência móvel completa |
| 012 | Verticais | ⬜ Planejada | Modelo comum estabilizado | Vendas, clínica e demais nomenclaturas |

## 10. Detalhamento do roadmap

### SPEC-001 — Autenticação e Multiempresa

**Status:** implementada na fundação.

**Entregue:** empresa inicial, administrador, login, JWT, roles base, contexto de tenant, filtros, migration, frontend protegido e Docker base.

**Gate restante:** validação ponta a ponta com PostgreSQL real e Docker Compose em execução.

### SPEC-002 — Usuários e Permissões

**Objetivo:** permitir que o administrador monte a equipe da empresa sem quebrar o isolamento multiempresa.

**Escopo mínimo sugerido:**

- listar, criar, editar, desativar e reativar usuários do próprio tenant;
- roles `Administrador`, `Gestor`, `Profissional` e `Atendente`;
- policies por operação;
- convite ou definição inicial de senha;
- proteção contra remoção do último administrador;
- paginação e busca;
- auditoria básica de mudanças sensíveis;
- testes de acesso permitido, negado e tenant divergente.

**Resultado:** equipe administrável e base de responsáveis para contatos, CRM e agenda.

### SPEC-003 — Contatos e Relacionamento

**Objetivo:** criar o cadastro central compartilhado pelas verticais.

**Escopo mínimo sugerido:**

- CRUD de contatos;
- telefone normalizado e deduplicação dentro da empresa;
- nome, telefone, e-mail, nascimento comercial, empresa, observações e responsável;
- CRUD de tags;
- relacionamento muitos-para-muitos contato-tag;
- filtros, busca e paginação no PostgreSQL;
- DTOs, validação e índices;
- telas desktop/mobile;
- testes de tenant e duplicidade.

**Resultado:** base de clientes/leads/pacientes comerciais pronta para receber mensagens e negócios.

### SPEC-004 — Integração WhatsApp

**Objetivo:** conectar cada empresa à API oficial do WhatsApp com segurança e idempotência.

**Escopo mínimo sugerido:**

- `whatsapp_contas` vinculada à empresa;
- WABA ID e Phone Number ID;
- armazenamento externo/seguro do access token;
- webhook GET para challenge;
- webhook POST com validação de assinatura;
- descoberta segura do tenant pela conta recebedora;
- `webhook_eventos` com ID único do provedor;
- persistência rápida e processamento idempotente;
- logs, correlação, retry e reprocessamento controlado;
- teste no sandbox da Meta usando apenas números autorizados.

**Resultado:** canal oficial conectado, ainda sem uma caixa de entrada completa.

### SPEC-005 — Conversas e Mensagens

**Objetivo:** permitir que atendentes recebam, consultem e respondam mensagens.

**Escopo mínimo sugerido:**

- `conversas` e `mensagens`;
- associação ou criação de contato pelo telefone;
- mensagens de entrada e saída;
- texto no MVP e política explícita para mídia;
- status enviada, entregue, lida e falha;
- lista de conversas, histórico e não lidas;
- responsável e status da conversa;
- envio manual pelo backend;
- SignalR para atualização em tempo real;
- loading, erros, retry seguro e mobile;
- janela de 24 horas, templates e opt-in conforme regras vigentes da Meta.

**Resultado:** primeiro atendimento humano funcional pelo WhatsCRM.

### SPEC-006 — CRM, Negócios e Vendas

**Objetivo:** acompanhar oportunidades do contato até o fechamento.

**Escopo mínimo sugerido:**

- funil e etapas configuráveis;
- Kanban;
- negócio, responsável e próximo contato;
- itens e valor total calculado no backend;
- status comercial separado da etapa visual;
- ganho, perda e motivo;
- histórico de movimentações;
- filtros e indicadores básicos;
- mobile e testes de concorrência/movimentação.

**Regra obrigatória:** mover uma venda ganha para Pós-venda/Retorno não altera o status comercial `Ganho`.

### SPEC-007 — Agenda e Compromissos

**Objetivo:** gerenciar reuniões, avaliações, consultas e retornos em uma entidade genérica.

**Escopo mínimo sugerido:**

- compromisso, tipo, status, início e fim;
- contato, profissional e negócio opcionais conforme regra;
- calendário e filtros;
- conflitos e timezone;
- reagendamento criando novo compromisso;
- vínculo com compromisso de origem;
- histórico;
- desktop/mobile e testes de datas.

### SPEC-008 — Pós-venda

**Objetivo:** manter o relacionamento depois do fechamento.

**Escopo mínimo sugerido:**

- ação de pós-venda;
- retorno, satisfação, reativação, acompanhamento, lembrete e outro;
- contato, negócio, responsável e data prevista;
- pendente, concluída e cancelada;
- conclusão manual no MVP;
- filtros de vencidas e próximas;
- vínculo visual com negócio ganho;
- nenhuma mensagem automática nesta spec.

### SPEC-009 — Tarefas e Follow-up

**Objetivo:** organizar atividades operacionais gerais sem misturá-las ao pós-venda.

**Escopo mínimo sugerido:**

- tarefa, título, descrição, tipo, prazo e status;
- contato, negócio e responsável;
- ligação, WhatsApp, proposta, follow-up, reunião e outro;
- atrasadas, hoje e futuras;
- conclusão e cancelamento;
- filtros, dashboard futuro e mobile.

### SPEC-010 — Dashboard

**Objetivo:** transformar dados dos módulos em visão operacional e comercial.

**Escopo mínimo sugerido:**

- conversas aguardando resposta;
- novos contatos;
- negócios abertos e valor do pipeline;
- vendas ganhas e conversão;
- follow-ups, tarefas e compromissos;
- pós-venda pendente;
- filtros por período, responsável e funil;
- regras consistentes de timezone e status;
- consultas agregadas eficientes e testes de indicadores.

### SPEC-011 — Mobile/PWA

**Objetivo:** consolidar todos os fluxos operacionais para celular e instalação PWA.

**Escopo mínimo sugerido:**

- auditoria de todas as telas em 375 e 768 px;
- navegação inferior funcional;
- manifest completo e ícones;
- service worker e estratégia de atualização;
- política de cache sem expor dados sensíveis;
- estados offline controlados;
- instalação;
- acessibilidade e áreas de toque;
- testes em navegadores/dispositivos definidos.

### SPEC-012 — Verticais

**Objetivo:** adaptar linguagem e experiência sem duplicar entidades ou código de domínio.

**Escopo mínimo sugerido:**

- configuração por `TipoNegocio`;
- nomenclatura de contato, compromisso, negócio e responsável;
- módulos habilitados por configuração;
- defaults de funil e tipos de compromisso;
- vendas, clínica, serviços, estética e outro;
- nenhum prontuário ou dado clínico sem arquitetura e spec próprias.

## 11. Caminho recomendado até o primeiro teste de WhatsApp

1. Concluir o teste integrado da SPEC-001 com Docker e PostgreSQL.
2. Escrever e implementar a SPEC-002.
3. Escrever e implementar a SPEC-003.
4. Criar conta/app de teste na Meta e registrar ativos de sandbox.
5. Escrever e implementar a SPEC-004 com webhook público HTTPS.
6. Validar assinatura, idempotência e isolamento da conta por empresa.
7. Escrever e implementar a SPEC-005.
8. Adicionar somente números próprios/autorizados ao ambiente de teste.
9. Testar mensagem recebida, resposta manual, status e duplicidade.
10. Definir em spec separada as regras do robô e a transferência para humano.
11. Executar piloto interno.
12. Somente depois conectar número e clientes reais, com opt-in, templates e políticas vigentes.

Referências operacionais que devem ser revisitadas na implementação, pois as regras podem mudar:

- coleção oficial da Meta para WhatsApp Cloud API: <https://www.postman.com/meta/whatsapp-business-platform/documentation/wlk6lh4/whatsapp-cloud-api>;
- política oficial do WhatsApp Business: <https://whatsappbusiness.com/policy/>.

## 12. Roadmap por fases

```text
FASE 1 — FUNDAÇÃO
SPEC-001 Auth + Multiempresa                    ✅
SPEC-002 Usuários + Permissões                 ⬜
        ↓
FASE 2 — RELACIONAMENTO
SPEC-003 Contatos + Tags                       ⬜
        ↓
FASE 3 — ATENDIMENTO WHATSAPP
SPEC-004 Integração + Webhook                  ⬜
SPEC-005 Conversas + Mensagens + SignalR       ⬜
        ↓
FASE 4 — COMERCIAL
SPEC-006 CRM + Funil + Negócios                ⬜
        ↓
FASE 5 — AGENDA
SPEC-007 Compromissos + Reagendamento          ⬜
        ↓
FASE 6 — EXECUÇÃO E RELACIONAMENTO CONTÍNUO
SPEC-008 Pós-venda                             ⬜
SPEC-009 Tarefas + Follow-up                   ⬜
        ↓
FASE 7 — VISÃO E EXPERIÊNCIA
SPEC-010 Dashboard                             ⬜
SPEC-011 Mobile/PWA                            ⬜
SPEC-012 Verticais                             ⬜
        ↓
FASES POSTERIORES — SOMENTE COM NOVAS SPECS
Orçamentos formais, automações, IA,
pagamentos, financeiro e módulos clínicos      ⛔
```

## 13. Prioridade imediata recomendada

### Marco A — fechar a fundação em ambiente real

- subir os três containers;
- executar bootstrap;
- validar login e `/api/auth/me`;
- registrar evidências e erros;
- criar testes HTTP de integração.

### Marco B — SPEC-002

- escrever a spec completa;
- implementar usuários e autorização;
- validar tenant e permissões.

### Marco C — SPEC-003

- escrever a spec completa;
- implementar contatos e tags;
- preparar telefone normalizado para a futura integração WhatsApp.

Depois desses marcos, iniciar SPEC-004/005 e o sandbox oficial do WhatsApp.

## 14. Definition of Done para cada próxima spec

- [ ] spec aprovada e atendida;
- [ ] banco, FKs, constraints e índices revisados;
- [ ] migration nova criada quando necessária;
- [ ] backend completo com DTO, service, controller e endpoint;
- [ ] frontend completo com validação, loading e erro;
- [ ] responsividade verificada;
- [ ] isolamento multiempresa testado;
- [ ] permissões testadas;
- [ ] Swagger atualizado;
- [ ] testes unitários e de integração relevantes;
- [ ] `dotnet build` aprovado;
- [ ] `dotnet test` aprovado;
- [ ] `npm run lint` aprovado;
- [ ] `npm run build` aprovado;
- [ ] `docker compose config` aprovado;
- [ ] nenhum secret ou dado sensível versionado;
- [ ] nenhuma alteração fora do escopo;
- [ ] documentação e este status atualizados;
- [ ] commit revisado e enviado ao GitHub.

## 15. Documentos relacionados

- [README do projeto](../README.md)
- [Arquitetura](../ARCHITECTURE.md)
- [Regras para agentes](../AGENTS.md)
- [Índice de specs](specs/README.md)
- [SPEC-001 — Autenticação e Multiempresa](specs/SPEC-001-auth-multitenant.md)

---

Este documento deve ser atualizado ao final de cada spec para permanecer como visão consolidada do produto.
