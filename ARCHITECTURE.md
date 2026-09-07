# WhatsCRM — Arquitetura do Sistema

## 1. Visão Geral

O **WhatsCRM** será uma plataforma SaaS multiempresa, multiusuário, Web e Mobile/PWA para centralizar:

- atendimento via WhatsApp;
- relacionamento com contatos;
- CRM;
- oportunidades;
- vendas;
- orçamentos;
- agenda e compromissos;
- follow-ups;
- pós-venda;
- tarefas;
- equipe;
- dashboards;
- automações futuras;
- IA futura;
- pagamentos futuros.

O sistema deverá possuir **um único núcleo tecnológico**, mas poderá ser comercializado em diferentes verticais.

Inicialmente:

```text
                    WHATSCRM
                        │
              NÚCLEO COMPARTILHADO
                        │
          ┌─────────────┴─────────────┐
          │                           │
   WhatsCRM Vendas             WhatsCRM Clínicas
          │                           │
   CRM Comercial                 CRM + Agenda
   Orçamentos                    Pacientes/Contatos
   Vendas                        Agendamentos
   Follow-up                     Orçamentos
   Pós-venda                     Follow-up
                                 Pós-venda
```

Não serão criados dois sistemas independentes.

Será:

> Um único SaaS com módulos, configurações, nomenclaturas e experiências adaptadas ao tipo de negócio.

---

# 2. Público-Alvo

## WhatsCRM Vendas

Voltado para:

- empresas de serviços;
- manutenção;
- ar-condicionado;
- energia solar;
- empresas de TI;
- consultorias;
- imobiliárias;
- escritórios;
- equipes comerciais;
- prestadores B2B;
- pequenas empresas em geral.

## WhatsCRM Clínicas

Voltado inicialmente para:

- clínicas odontológicas;
- clínicas médicas;
- estética;
- recepcionistas;
- secretárias;
- médicos;
- dentistas;
- profissionais;
- equipes de atendimento.

O WhatsCRM Clínicas inicialmente será um sistema de:

```text
Relacionamento
+
WhatsApp
+
CRM
+
Agenda
+
Orçamento
+
Pós-venda
```

Não será inicialmente um prontuário eletrônico.

---

# 3. Proposta do Produto

## Comercial

> Organize clientes do WhatsApp, acompanhe oportunidades e venda mais sem esquecer propostas e follow-ups.

## Clínicas

> Organize mensagens, pacientes, agenda, orçamentos, retornos e relacionamento em um único sistema.

---

# 4. Jornada Principal

A arquitetura deverá suportar todo este ciclo:

```text
WhatsApp
   ↓
Contato
   ↓
Relacionamento
   ↓
Agendamento / Reunião
   ↓
Negócio
   ↓
Orçamento
   ↓
Negociação
   ↓
Fechamento
   ↓
Pós-venda / Retorno
   ↓
Reativação
```

O ciclo não termina quando a venda é fechada.

O pós-venda faz parte explícita do produto.

---

# 5. Arquitetura Tecnológica

```text
                        INTERNET
                            │
                            ▼
                          NGINX
                            │
              ┌─────────────┴─────────────┐
              │                           │
              ▼                           ▼
      React + TypeScript          ASP.NET Core Web API
       Responsive / PWA                 .NET 10
              │                           │
              │                ┌──────────┼──────────┐
              │                │          │          │
              │               JWT      SignalR   Services
              │                                      │
              │                             ┌────────┴────────┐
              │                             │                 │
              ▼                             ▼                 ▼
          Browser/PWA                  PostgreSQL        Integrações
                                                            │
                                                     WhatsApp Cloud API
```

---

# 6. Stack

## Frontend

Utilizar:

```text
React
TypeScript
Vite
React Router
Axios
PWA
Responsive Design
```

## Backend

Utilizar:

```text
C#
.NET 10
ASP.NET Core Web API
Entity Framework Core
Npgsql
PostgreSQL
JWT
Roles
Claims
SignalR
Swagger / OpenAPI
```

## Infraestrutura

```text
Docker
Docker Compose
Nginx
Linux
Git
GitHub
```

---

# 7. SaaS Multiempresa

Uma única instalação atenderá várias organizações.

```text
WhatsCRM
│
├── Empresa A
│   ├── usuários
│   ├── contatos
│   ├── mensagens
│   ├── negócios
│   └── agenda
│
├── Clínica B
│   ├── usuários
│   ├── pacientes/contatos
│   ├── mensagens
│   ├── negócios
│   └── agenda
│
└── Empresa C
```

Regra crítica:

> Nenhuma empresa pode consultar ou alterar dados de outra empresa.

Toda entidade operacional relevante deverá possuir `EmpresaId` direta ou indiretamente de forma segura.

O `EmpresaId` utilizado para autorização deve vir do contexto autenticado.

Nunca confiar no `EmpresaId` enviado pelo frontend.

---

# 8. Verticalização

A entidade `empresas` deverá indicar o tipo ou edição utilizada.

Exemplo conceitual:

```text
empresas

id
nome
nome_fantasia
cnpj
tipo_negocio
ativo
created_at
updated_at
```

Tipos iniciais:

```text
VENDAS
CLINICA
SERVICOS
ESTETICA
OUTRO
```

A aplicação poderá apresentar nomenclaturas diferentes.

## Clínica

```text
Contato       → Paciente
Compromisso   → Agendamento
Negócio       → Orçamento / Tratamento Comercial
Responsável   → Profissional
```

## Comercial

```text
Contato       → Cliente / Lead
Compromisso   → Reunião / Visita
Negócio       → Oportunidade / Venda
Responsável   → Vendedor
```

O modelo de dados central permanecerá consistente.

---

# 9. Módulos do Núcleo

O núcleo compartilhado deverá possuir:

```text
Autenticação
Empresas
Usuários
Permissões
WhatsApp
Contatos
Tags
Conversas
Mensagens
CRM
Funil
Negócios
Itens de negócio
Compromissos
Tarefas
Follow-up
Ações pós-venda
Dashboard
Notificações
```

---

# 10. Relacionamento

A entidade central será:

```text
contatos
```

Campos iniciais:

```text
id
empresa_id
nome
telefone
email
data_nascimento
empresa_nome
observacoes
responsavel_usuario_id
created_at
updated_at
```

`data_nascimento` será usada para relacionamento comercial, como:

- aniversário;
- campanhas;
- ações de relacionamento.

Não deve ser interpretada automaticamente como informação clínica.

---

# 11. Tags

Contatos poderão possuir várias tags.

Não armazenar tags em uma única string.

Estrutura:

```text
tags

id
empresa_id
nome
cor
created_at
```

Relacionamento:

```text
contato_tags

contato_id
tag_id
```

Exemplos:

```text
[ Convênio ]
[ Paciente de aparelho ]
[ Retorno ]
[ VIP ]
[ Lead quente ]
[ Indicação ]
[ Cliente recorrente ]
```

As tags deverão poder ser utilizadas posteriormente para:

- filtros;
- segmentação;
- campanhas;
- automações;
- relatórios.

---

# 12. WhatsApp

Utilizar integração oficial:

```text
WhatsApp Business Platform
Cloud API
```

Recebimento:

```text
Cliente
   ↓
WhatsApp
   ↓
Meta
   ↓
Webhook
   ↓
ASP.NET Core
   ↓
PostgreSQL
   ↓
SignalR
   ↓
React
```

Envio:

```text
React
   ↓
ASP.NET Core
   ↓
WhatsApp Cloud API
   ↓
Cliente
```

Secrets nunca poderão ir para o frontend.

---

# 13. Conversas

Entidade:

```text
conversas
```

Campos sugeridos:

```text
id
empresa_id
contato_id
whatsapp_conta_id
responsavel_usuario_id
status
ultima_mensagem_em
ultima_mensagem_texto
quantidade_nao_lidas
created_at
updated_at
```

Status:

```text
ABERTA
PENDENTE
RESOLVIDA
```

---

# 14. Mensagens

Entidade:

```text
mensagens
```

Campos:

```text
id
empresa_id
conversa_id
provider_message_id
direcao
tipo
texto
status
enviada_em
recebida_em
lida_em
created_at
```

Direção:

```text
ENTRADA
SAIDA
```

Tipos previstos:

```text
TEXTO
IMAGEM
DOCUMENTO
AUDIO
VIDEO
LOCALIZACAO
TEMPLATE
```

---

# 15. Webhooks

Criar:

```text
webhook_eventos
```

Campos:

```text
id
empresa_id
whatsapp_conta_id
provider_event_id
tipo
payload
processado
created_at
processed_at
```

`provider_event_id` deverá possuir proteção contra duplicidade.

Fluxo:

```text
Webhook recebido
      ↓
Validar
      ↓
Identificar conta
      ↓
Identificar empresa
      ↓
Já processado?
      ↓
SIM ─────────────→ retornar sucesso
      ↓
NÃO
      ↓
Persistir
      ↓
Processar
      ↓
Contato
      ↓
Conversa
      ↓
Mensagem
      ↓
SignalR
```

---

# 16. CRM

O CRM será baseado em:

```text
funis
funil_etapas
negocios
itens_negocio
```

O termo técnico principal será `negocio`.

Na interface poderá aparecer como:

```text
Oportunidade
Venda
Orçamento
Tratamento
```

dependendo da vertical.

---

# 17. Funil Kanban

Modelo visual padrão:

```text
NOVO
   ↓
CONTATO
   ↓
AVALIAÇÃO
   ↓
ORÇAMENTO
   ↓
PROPOSTA
   ↓
NEGOCIAÇÃO
   ↓
FECHADO / GANHO
   ↓
PÓS-VENDA / RETORNO
```

Nem toda empresa será obrigada a usar todas essas etapas.

As etapas deverão ser configuráveis.

---

# 18. Regra Importante do Kanban

O status comercial do negócio será separado da etapa visual.

Campos conceituais:

```text
negocio.status

ABERTO
GANHO
PERDIDO
```

e:

```text
negocio.funil_etapa_id
```

Quando uma oportunidade chegar em:

```text
FECHADO / GANHO
```

o sistema deverá registrar:

```text
status = GANHO
```

Se depois o cartão for apresentado na etapa:

```text
PÓS-VENDA / RETORNO
```

o status continuará:

```text
GANHO
```

Isso evita quebrar indicadores comerciais.

Exemplo:

```text
Negócio R$ 8.500

Etapa visual:
PÓS-VENDA / RETORNO

Status comercial:
GANHO
```

Portanto:

> Pós-venda não desfaz uma venda ganha.

---

# 19. Negócios

Entidade:

```text
negocios
```

Campos iniciais:

```text
id
empresa_id
contato_id
funil_id
funil_etapa_id
responsavel_usuario_id

titulo
descricao
valor_total

status

data_proximo_contato

ganho_em
perdido_em
motivo_perda

created_at
updated_at
```

---

# 20. Itens do Negócio

Entidade:

```text
itens_negocio
```

Campos:

```text
id
negocio_id
descricao
quantidade
valor_unitario
valor_total
created_at
```

Exemplo:

```text
Negócio #154

Clareamento             R$ 1.200
Limpeza                 R$   300
--------------------------------
Total                   R$ 1.500
```

Ou em empresa comercial:

```text
Instalação              R$ 2.000
Equipamento             R$ 4.500
--------------------------------
Total                   R$ 6.500
```

---

# 21. Agendamento / Compromissos

Utilizar entidade genérica:

```text
compromissos
```

Ela servirá para:

- consultas;
- retornos;
- avaliações;
- manutenções;
- reuniões;
- visitas;
- demonstrações.

Campos:

```text
id
empresa_id
contato_id
profissional_usuario_id
negocio_id

tipo
status

inicio
fim

compromisso_origem_id

observacao_operacional

created_at
updated_at
```

---

# 22. Tipos de Compromisso

Exemplos iniciais:

```text
CONSULTA
RETORNO
MANUTENCAO
AVALIACAO
REUNIAO
VISITA
DEMONSTRACAO
OUTRO
```

---

# 23. Status do Compromisso

```text
AGENDADO
CONFIRMADO
REALIZADO
CANCELADO
NAO_COMPARECEU
REAGENDADO
```

---

# 24. Reagendamento

Nunca simplesmente sobrescrever o horário antigo e perder o histórico.

Fluxo:

```text
Compromisso A
10/09 14:00
      ↓
REAGENDADO
      ↓
Compromisso B
12/09 16:00
```

O novo compromisso poderá armazenar:

```text
compromisso_origem_id
```

Isso permitirá histórico de reagendamentos.

---

# 25. Pós-venda

Criar entidade própria:

```text
acoes_pos_venda
```

Campos:

```text
id
empresa_id
contato_id
negocio_id
responsavel_usuario_id

tipo
titulo
descricao

data_prevista
data_realizada

status

created_at
updated_at
```

---

# 26. Tipos de Pós-venda

Inicialmente:

```text
RETORNO
PESQUISA_SATISFACAO
REATIVACAO
ACOMPANHAMENTO
LEMBRETE
OUTRO
```

---

# 27. Status de Pós-venda

```text
PENDENTE
CONCLUIDA
CANCELADA
```

No MVP, a criação e execução serão manuais.

Fluxo:

```text
Venda ganha
    ↓
Criar ação pós-venda
    ↓
Data prevista
    ↓
Dashboard
    ↓
Atendente executa
    ↓
Concluir
```

Futuramente:

```text
Ação pós-venda
    ↓
Data chegou
    ↓
Automação
    ↓
WhatsApp
    ↓
Mensagem enviada
```

---

# 28. Exemplo Clínica

```text
WhatsApp
   ↓
Maria entra em contato
   ↓
Contato/Paciente
   ↓
Tag: [ Aparelho ]
   ↓
Avaliação
   ↓
Compromisso
   ↓
Negócio
   ↓
Orçamento R$ 6.500
   ↓
Negociação
   ↓
GANHO
   ↓
Pós-venda / Retorno
   ↓
Ação: lembrar manutenção
```

---

# 29. Exemplo Comercial

```text
WhatsApp
   ↓
Empresa ABC
   ↓
Lead
   ↓
Reunião
   ↓
Negócio
   ↓
Proposta R$ 15.000
   ↓
Negociação
   ↓
GANHO
   ↓
Pós-venda
   ↓
Pesquisa de satisfação
```

O mesmo núcleo atende os dois casos.

---

# 30. Tarefas

Entidade:

```text
tarefas
```

Campos:

```text
id
empresa_id
contato_id
negocio_id
responsavel_usuario_id

titulo
descricao
tipo
data_hora
status

created_at
updated_at
```

Tipos:

```text
LIGACAO
WHATSAPP
ENVIAR_PROPOSTA
FOLLOW_UP
REUNIAO
OUTRO
```

---

# 31. Diferença entre Tarefa e Pós-venda

`tarefas`:

> Atividades operacionais gerais.

Exemplos:

```text
Ligar amanhã
Enviar orçamento
Responder cliente
```

`acoes_pos_venda`:

> Atividades relacionadas especificamente a um relacionamento após fechamento.

Exemplos:

```text
Retorno em 30 dias
Pesquisa de satisfação
Reativar cliente em 6 meses
```

As duas entidades possuem finalidades diferentes.

---

# 32. Dashboard

Métricas iniciais:

```text
Conversas aguardando resposta
Novos contatos
Negócios abertos
Valor do pipeline
Vendas ganhas
Taxa de conversão
Follow-ups de hoje
Tarefas atrasadas
Compromissos de hoje
Ações pós-venda pendentes
```

Para clínicas:

```text
Agendamentos de hoje
Confirmações pendentes
Não comparecimentos
Retornos pendentes
```

---

# 33. Tela Web

Desktop:

```text
┌─────────────────┬──────────────────────────────────────┐
│ WhatsCRM        │                                      │
│                 │                                      │
│ Dashboard       │            CONTEÚDO                  │
│ Conversas       │                                      │
│ Contatos        │                                      │
│ CRM             │                                      │
│ Agenda          │                                      │
│ Pós-venda       │                                      │
│ Tarefas         │                                      │
│ Equipe          │                                      │
│ Relatórios      │                                      │
│ Configurações   │                                      │
└─────────────────┴──────────────────────────────────────┘
```

---

# 34. Mobile/PWA

Menu inferior:

```text
Conversas
Contatos
CRM
Agenda
Mais
```

O usuário deverá conseguir pelo celular:

```text
Ver mensagem
↓
Responder
↓
Consultar contato
↓
Criar negócio
↓
Mover Kanban
↓
Agendar compromisso
↓
Criar follow-up
↓
Registrar pós-venda
```

---

# 35. Usuários

Roles iniciais:

```text
ADMINISTRADOR
GESTOR
PROFISSIONAL
ATENDENTE
```

Na vertical comercial:

```text
PROFISSIONAL → VENDEDOR
```

Na clínica:

```text
PROFISSIONAL → MÉDICO / DENTISTA
```

A Role interna poderá permanecer genérica.

---

# 36. Segurança em Clínicas

O sistema inicialmente não deve armazenar silenciosamente:

- diagnóstico;
- exame;
- prescrição;
- prontuário;
- doença;
- condição médica;
- histórico clínico.

Informações comerciais e de relacionamento devem permanecer separadas de informações clínicas.

Exemplo aceitável:

```text
Tag:
Paciente aparelho
```

O sistema não deve transformar isso automaticamente em prontuário.

Um futuro módulo clínico terá arquitetura e especificação próprias.

---

# 37. Backend

Estrutura:

```text
backend/
└── WhatsCrm.Api/
    ├── Controllers/
    ├── Services/
    ├── Interfaces/
    ├── Entities/
    ├── DTOs/
    ├── Data/
    ├── Configurations/
    ├── Integrations/
    │   └── WhatsApp/
    ├── Hubs/
    ├── Middlewares/
    ├── Extensions/
    ├── Migrations/
    └── Program.cs
```

---

# 38. Frontend

```text
frontend/
└── src/
    ├── components/
    ├── pages/
    │   ├── Login/
    │   ├── Dashboard/
    │   ├── Conversations/
    │   ├── Contacts/
    │   ├── Crm/
    │   ├── Calendar/
    │   ├── AfterSales/
    │   ├── Tasks/
    │   ├── Team/
    │   └── Settings/
    ├── services/
    ├── hooks/
    ├── contexts/
    ├── layouts/
    ├── routes/
    ├── types/
    └── utils/
```

---

# 39. Banco Inicial

Principais tabelas:

```text
empresas
usuarios

whatsapp_contas
webhook_eventos

contatos
tags
contato_tags

conversas
mensagens

funis
funil_etapas
negocios
itens_negocio

compromissos

tarefas

acoes_pos_venda
```

---

# 40. API Inicial

## Auth

```text
POST /api/auth/login
GET  /api/auth/me
```

## Contatos

```text
GET    /api/contatos
GET    /api/contatos/{id}
POST   /api/contatos
PUT    /api/contatos/{id}
DELETE /api/contatos/{id}
```

## Tags

```text
GET    /api/tags
POST   /api/tags
PUT    /api/tags/{id}
DELETE /api/tags/{id}
```

## Conversas

```text
GET   /api/conversas
GET   /api/conversas/{id}
GET   /api/conversas/{id}/mensagens
POST  /api/conversas/{id}/mensagens
PATCH /api/conversas/{id}/responsavel
```

## Negócios

```text
GET   /api/negocios
GET   /api/negocios/{id}
POST  /api/negocios
PUT   /api/negocios/{id}
PATCH /api/negocios/{id}/etapa
PATCH /api/negocios/{id}/status
```

## Compromissos

```text
GET    /api/compromissos
POST   /api/compromissos
PUT    /api/compromissos/{id}
POST   /api/compromissos/{id}/reagendar
PATCH  /api/compromissos/{id}/status
```

## Pós-venda

```text
GET   /api/acoes-pos-venda
POST  /api/acoes-pos-venda
PUT   /api/acoes-pos-venda/{id}
PATCH /api/acoes-pos-venda/{id}/concluir
```

---

# 41. EF Core

Obrigatório:

- migrations;
- índices;
- FKs;
- constraints;
- paginação;
- `AsNoTracking` para leituras;
- evitar N+1;
- filtros no banco;
- isolamento por empresa.

---

# 42. Índices Importantes

Considerar índices compostos em:

```text
contatos:
empresa_id + telefone

tags:
empresa_id + nome

conversas:
empresa_id + ultima_mensagem_em

mensagens:
conversa_id + created_at

negocios:
empresa_id + status
empresa_id + funil_etapa_id
empresa_id + responsavel_usuario_id

compromissos:
empresa_id + inicio
empresa_id + profissional_usuario_id + inicio

acoes_pos_venda:
empresa_id + status + data_prevista
```

---

# 43. Desenvolvimento por Specs

Estrutura:

```text
docs/specs/

SPEC-001-auth-multitenant.md
SPEC-002-usuarios-permissoes.md
SPEC-003-contatos-relacionamento.md
SPEC-004-whatsapp-integracao.md
SPEC-005-conversas-mensagens.md
SPEC-006-crm-negocios-vendas.md
SPEC-007-agenda-compromissos.md
SPEC-008-pos-venda.md
SPEC-009-tarefas-followup.md
SPEC-010-dashboard.md
SPEC-011-mobile-pwa.md
SPEC-012-verticais.md
```

---

# 44. Roadmap

## Fase 1

```text
Fundação
Autenticação
Multiempresa
Usuários
Roles
Docker
```

## Fase 2

```text
Contatos
Tags
Relacionamento
```

## Fase 3

```text
WhatsApp
Conversas
Mensagens
SignalR
```

## Fase 4

```text
CRM
Funil
Negócios
Itens do negócio
Kanban
```

## Fase 5

```text
Agenda
Compromissos
Reagendamento
```

## Fase 6

```text
Tarefas
Follow-up
Pós-venda
Retorno
Reativação
```

## Fase 7

```text
Dashboard
Relatórios
Mobile/PWA
```

## Fase 8

```text
Orçamentos formais
Produtos/serviços
Documentos
```

## Fase 9

```text
Automações WhatsApp
Templates
Distribuição de leads
```

## Fase 10

```text
IA
Resumo
Classificação
Sugestão de resposta
```

## Fase 11

```text
Pix
Cobrança
Contas a receber
Financeiro
```

---

# 45. Princípio Fundamental

Construir:

```text
UM NÚCLEO
+
MÓDULOS
+
VERTICAIS
```

e não:

```text
Sistema Clínicas separado
+
Sistema Vendas separado
```

O sistema deve crescer sem duplicar código.

---

# 46. Resultado Esperado

## Clínica

```text
WhatsApp
   ↓
Paciente
   ↓
Agenda
   ↓
Avaliação
   ↓
Orçamento
   ↓
Negociação
   ↓
Fechamento
   ↓
Retorno
   ↓
Pós-venda
```

## Vendas

```text
WhatsApp
   ↓
Lead
   ↓
Reunião
   ↓
Oportunidade
   ↓
Proposta
   ↓
Negociação
   ↓
Venda
   ↓
Pós-venda
```

O mesmo sistema deverá suportar os dois fluxos.

Este documento é a **fonte de verdade arquitetural do WhatsCRM**.