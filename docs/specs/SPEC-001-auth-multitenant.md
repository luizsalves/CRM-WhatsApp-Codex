# SPEC-001 — Autenticação e Multiempresa

## 1. Título

Fundação de autenticação JWT, empresa, usuário administrador, roles e isolamento multiempresa.

## 2. Objetivo

Criar a primeira fundação executável do WhatsCRM. Ao final desta spec, uma instalação vazia permite cadastrar uma empresa com seu primeiro administrador, autenticar o usuário, identificar empresa e usuário por claims seguras e acessar uma rota protegida sem exposição de dados de outro tenant.

## 3. Problema

O WhatsCRM atenderá várias empresas na mesma instalação e no mesmo banco. Toda funcionalidade futura depende de uma identidade autenticada e de um limite de autorização por empresa que não possa ser escolhido pelo frontend.

Sem essa fundação, qualquer módulo operacional poderia consultar ou gravar dados no tenant errado.

## 4. Escopo

- entidade `Empresa`;
- entidade `Usuario`;
- tipos iniciais de negócio;
- perfis iniciais de usuário;
- configuração do primeiro administrador em instalação vazia;
- hash de senha usando `PasswordHasher<TUser>`;
- login por e-mail e senha;
- emissão e validação de JWT;
- claims `sub`, `empresa_id`, nome, e-mail e role;
- endpoint autenticado de identidade atual;
- contexto de tenant derivado apenas do usuário autenticado;
- filtro global e guarda de escrita no EF Core;
- migration PostgreSQL;
- tela de primeiro acesso e login;
- layout e rota frontend protegida;
- tratamento de erros, rate limit e Swagger;
- testes automatizados essenciais;
- Docker Compose da fundação.

## 5. Fora do escopo

- recuperação e alteração de senha;
- refresh token e revogação centralizada;
- MFA e login social;
- convite, cadastro e administração de outros usuários (SPEC-002);
- permissões granulares além de roles;
- contatos, tags, WhatsApp, conversas e mensagens;
- CRM, agenda, tarefas e pós-venda;
- SignalR, notificações, automações e IA;
- faturamento, assinatura e pagamentos;
- prontuário ou qualquer dado clínico.

## 6. Atores

### Instalador

Acessa uma instalação sem usuários e cria a primeira empresa e a primeira conta administradora.

### Usuário não autenticado

Consulta a disponibilidade do primeiro acesso e realiza login.

### Administrador

Primeiro usuário da empresa. Nesta spec, acessa apenas sua identidade e o layout protegido.

### Sistema

Emite e valida JWT, deriva o tenant e impede leitura ou gravação fora da empresa autenticada.

## 7. Entidades e campos

### Empresa (`empresas`)

| Campo | Tipo | Regra |
| --- | --- | --- |
| `id` | UUID | Chave primária gerada pela aplicação |
| `nome` | varchar(160) | Obrigatório |
| `nome_fantasia` | varchar(160) | Opcional |
| `cnpj` | varchar(14) | Opcional; único quando informado |
| `tipo_negocio` | varchar(24) | Obrigatório |
| `ativo` | boolean | Obrigatório; padrão lógico verdadeiro |
| `created_at` | timestamptz | Preenchido pelo backend |
| `updated_at` | timestamptz | Preenchido pelo backend |

Tipos iniciais: `Vendas`, `Clinica`, `Servicos`, `Estetica`, `Outro`.

### Usuário (`usuarios`)

| Campo | Tipo | Regra |
| --- | --- | --- |
| `id` | UUID | Chave primária gerada pela aplicação |
| `empresa_id` | UUID | Obrigatório; FK para `empresas` |
| `nome` | varchar(160) | Obrigatório |
| `email` | varchar(254) | Obrigatório |
| `email_normalizado` | varchar(254) | Obrigatório; único globalmente nesta versão |
| `password_hash` | varchar(512) | Obrigatório; nunca retornar pela API |
| `perfil` | varchar(24) | Obrigatório |
| `ativo` | boolean | Obrigatório |
| `created_at` | timestamptz | Preenchido pelo backend |
| `updated_at` | timestamptz | Preenchido pelo backend |

Perfis iniciais: `Administrador`, `Gestor`, `Profissional`, `Atendente`.

O e-mail normalizado é único globalmente porque o login desta versão não solicita identificador de tenant. Uma futura mudança dessa regra exige fluxo de login não ambíguo e migration própria.

## 8. Relacionamentos

- uma empresa possui muitos usuários;
- um usuário pertence exatamente a uma empresa;
- a FK `usuarios.empresa_id` usa exclusão restrita;
- empresas com usuários não são excluídas em cascata.

## 9. Regras de negócio

### Configuração inicial

1. A configuração só está disponível quando não existe nenhum usuário, considerando todos os tenants.
2. A operação cria empresa e administrador na mesma transação em banco relacional.
3. O primeiro usuário sempre recebe o perfil `Administrador`.
4. Depois da criação, novas tentativas recebem conflito HTTP 409.
5. A senha precisa ter ao menos 12 caracteres, letra maiúscula, letra minúscula e número.
6. A resposta já inclui um JWT válido para iniciar a sessão.

### Login

1. E-mail é aparado e normalizado em maiúsculas para busca.
2. A resposta de falha não revela se o e-mail existe.
3. Usuário ou empresa inativa não pode autenticar.
4. Hash que precise de atualização é recalculado após login válido.

### Tenant

1. `EmpresaId` usado para autorização vem exclusivamente da claim `empresa_id` de um token validado.
2. `UsuarioId` vem da claim de identificador do token.
3. Nenhum DTO desta spec aceita `EmpresaId` como autorização.
4. Consultas autenticadas aplicam filtro global por empresa.
5. Uma escrita com `EmpresaId` diferente do contexto autenticado é bloqueada.
6. Alterar `EmpresaId` de uma entidade existente é proibido.
7. Serviços internos de bootstrap e login podem ignorar filtros apenas nos pontos explicitamente revisados.

### JWT

1. Assinatura HMAC SHA-256.
2. Secret com no mínimo 32 caracteres e fornecido por variável de ambiente.
3. Emissor e audiência são validados.
4. Expiração padrão de oito horas, configurável entre 5 e 1440 minutos.
5. Tolerância de relógio de 30 segundos.
6. Claims mínimas: identificador do token, usuário, empresa, nome, e-mail e role.

## 10. Permissões

| Operação | Anônimo | Usuário autenticado |
| --- | --- | --- |
| Consultar status do primeiro acesso | Sim | Sim |
| Criar primeira empresa/admin | Apenas se instalação vazia | Apenas se instalação vazia |
| Login | Sim | Sim |
| Consultar identidade atual | Não | Sim, somente o próprio usuário no tenant |

Permissões de gestão de equipe serão definidas na SPEC-002.

## 11. Endpoints

### `GET /api/auth/bootstrap-status`

Resposta 200:

```json
{ "disponivel": true }
```

### `POST /api/auth/bootstrap`

Requisição:

```json
{
  "empresaNome": "Empresa Exemplo",
  "nome": "Administrador",
  "email": "admin@exemplo.com",
  "senha": "SenhaForte123",
  "tipoNegocio": "Vendas"
}
```

Respostas: 201; 400 para validação; 409 quando já configurado; 429 por excesso de tentativas.

### `POST /api/auth/login`

Requisição:

```json
{
  "email": "admin@exemplo.com",
  "senha": "SenhaForte123"
}
```

Resposta 200 contém `token`, `expiraEm` e `usuario`. Erros: 400, 401 ou 429.

### `GET /api/auth/me`

Requer `Authorization: Bearer <token>`. Retorna somente dados não sensíveis do usuário e da empresa. Erros: 401 ou 403.

### `GET /health`

Retorna 200 para indicar que o processo HTTP está operacional. Uma verificação aprofundada do banco pode ser adicionada em spec de observabilidade.

## 12. Fluxo backend

### Bootstrap

```text
Validar DTO e senha
→ abrir transação
→ verificar ausência global de usuários
→ criar Empresa
→ criar Administrador com hash
→ persistir
→ confirmar transação
→ emitir JWT
```

### Login

```text
Normalizar e-mail
→ buscar usuário e empresa ignorando filtro de tenant neste ponto controlado
→ validar estados ativos
→ verificar hash
→ rehash quando necessário
→ emitir JWT com empresa_id
```

### Requisição autenticada

```text
Validar assinatura, emissor, audiência e expiração
→ construir ClaimsPrincipal
→ TenantContext extrai usuário e empresa
→ EF Core filtra por EmpresaId
→ serviço retorna DTO
```

## 13. Fluxo frontend

1. Ao abrir o login, consultar `bootstrap-status`.
2. Se disponível, mostrar acesso à configuração inicial.
3. Enviar formulário e exibir loading durante a operação.
4. Exibir mensagem segura em caso de erro.
5. Persistir o JWT localmente nesta primeira versão e anexá-lo via interceptor Axios.
6. Na inicialização, validar a sessão em `/api/auth/me`.
7. Token inválido remove a sessão e redireciona ao login.
8. Logout remove o token local.

O armazenamento de token será reavaliado junto à estratégia de refresh/revogação futura. Todo conteúdo frontend deve manter proteção contra XSS e não usar HTML arbitrário.

## 14. UX desktop

- login em painel central, campos com labels e mensagens acessíveis;
- tela separada para primeiro acesso;
- layout autenticado com sidebar;
- dashboard de fundação informa o que está implementado e o que pertence ao roadmap;
- módulos futuros aparecem como indisponíveis e não simulam funcionalidades inexistentes.

## 15. UX mobile

- funcionar a partir de 375 px;
- formulários em uma coluna;
- inputs e botões com área confortável para toque;
- navegação inferior sem depender de hover;
- header compacto e ação de logout acessível;
- nenhum conteúdo essencial deve exigir rolagem horizontal.

## 16. Segurança

- segredo JWT e senha do PostgreSQL somente por ambiente/secret manager;
- senha nunca é registrada ou retornada;
- hash via implementação oficial do ASP.NET Core Identity;
- mensagens de login não permitem enumeração direta de usuário;
- rate limit de dez tentativas por minuto por IP em bootstrap e login;
- Problem Details sem stack trace;
- logs de falhas inesperadas usam `traceId`;
- CORS por allowlist configurável;
- headers básicos de segurança no Nginx;
- nenhuma claim é aceita sem validação criptográfica do JWT;
- testes cobrem isolamento de leitura e escrita entre tenants.

## 17. Performance

- índices em `usuarios.email_normalizado`, `usuarios(empresa_id, perfil)` e `empresas.cnpj`;
- consultas de status, identidade e testes de existência executadas no banco;
- leituras sem rastreamento quando não há atualização;
- payloads pequenos e sem navegações desnecessárias;
- não introduzir cache distribuído nesta fase.

## 18. Erros

| Situação | HTTP | Comportamento |
| --- | --- | --- |
| DTO inválido | 400 | Validation Problem Details |
| Senha fora da política | 400 | Mensagem de regra de negócio |
| Bootstrap já executado | 409 | Sem criar novos dados |
| Credenciais inválidas | 401 | Mensagem genérica |
| Token ausente/inválido | 401 | Sem conteúdo sensível |
| Tenant divergente | 403 | Escrita ou acesso bloqueado |
| Limite de autenticação excedido | 429 | Requisição rejeitada |
| Erro inesperado | 500 | Mensagem genérica e `traceId` |

## 19. Critérios de aceite

- [x] Instalação vazia informa que bootstrap está disponível.
- [x] Bootstrap cria exatamente uma empresa e um administrador.
- [x] Segunda tentativa de bootstrap é bloqueada.
- [x] Senha fraca é rejeitada.
- [x] Login válido retorna JWT com `empresa_id` e usuário.
- [x] Login inválido retorna resposta genérica.
- [x] `/api/auth/me` exige autenticação.
- [x] Consultas de um tenant não retornam usuários de outro tenant.
- [x] Escrita com empresa divergente é bloqueada.
- [x] Frontend possui login, primeiro acesso, loading, erro e rota protegida.
- [x] Layout funciona em desktop e mobile.
- [x] Migration contém PK, FK, índices e constraints de nulabilidade.
- [x] Docker Compose não contém secret real e preserva volume PostgreSQL.

## 20. Testes

### Automatizados nesta entrega

- política aceita senha forte;
- política rejeita senha curta, sem maiúscula, sem minúscula e sem número;
- filtro EF retorna apenas usuários da empresa autenticada;
- `SaveChanges` bloqueia entidade de outra empresa;
- build do backend;
- build e lint do frontend;
- validação sintática do Docker Compose.

### Integração/manual recomendada

- subir ambiente limpo e executar bootstrap pela interface;
- conferir as tabelas e a migration no PostgreSQL;
- efetuar logout e login;
- adulterar/expirar token e confirmar HTTP 401;
- usar token de uma empresa contra registros de outra quando os endpoints da SPEC-002 existirem;
- validar visualmente em 375, 768 e 1366 px.

## 21. Decisões e riscos conhecidos

- O JWT é autocontido e não possui revogação imediata nesta fase; desativação é verificada em `/me`, mas endpoints futuros devem avaliar a política adequada.
- O token fica no armazenamento local do navegador nesta fundação. Refresh token em cookie seguro e revogação serão tratados em spec própria.
- E-mail é globalmente único para impedir login ambíguo sem solicitar tenant.
- O endpoint de bootstrap existe em todas as instalações, mas torna-se inoperante após o primeiro usuário e possui rate limit. Operações concorrentes também são protegidas pelas constraints do banco.
