# CLAUDE.md — PetBuddies-API (.NET)

> Serviço .NET do desafio FIAP 2026. Domínio clínico: clínica, veterinário, responsável, animal, consulta, procedimento.
> Integrado ao `petbuddies-ai` (Java) via REST — Java chama .NET para dados clínicos/tools conversacionais, e o .NET chama o motor Java para efeitos de cuidado quando eventos clínicos nascem no domínio clínico.

---

## Stack

- **.NET 8.0** / ASP.NET Core / EF Core 8.0.26
- **Oracle FIAP** via `Oracle.EntityFrameworkCore 8.23.26200`
- **Swagger** via Swashbuckle 6.6.2 + Annotations habilitado
- Nullable reference types habilitado (`<Nullable>enable</Nullable>`)

**Porta local:** HTTP `5297` · HTTPS `7049`

---

## Estado atual (2026-05-19)

| O que existe | Status |
|---|---|
| 11 entidades/models com campos e relacionamentos | ✅ |
| 10 enums de domínio | ✅ |
| `ApplicationContext` com 11 DbSets | ✅ |
| `GET /api/animal/{id}/motor` | ✅ controller + service + DTO |
| `GET /api/animal/{id}/ultima-consulta` | ✅ controller + service + DTO |
| `JsonStringEnumConverter` global | ✅ enums serializam como string |
| DTOs organizados por domínio (`Dtos/Animal`, `Responsavel`, `Consulta`, `JanelaAtendimento`) | ✅ |
| Controllers REST de Responsavel, Animal, Consulta e JanelaAtendimento | ✅ |
| CRUD básico de Responsavel, Animal, Consulta e JanelaAtendimento | ✅ |
| `AnimalMotorDto.PreCadastro` | ✅ |
| `ResponsavelEntity.EnderecoId` nullable | ✅ |
| Tabelas `pb_tb_*` mapeadas — exceto pendências históricas de Procedimento/Registro conforme migration final | ⚠️ |
| `BaseEntity` com `CreatedAt`/`UpdatedAt` (auto via `SaveChangesAsync`) | ✅ |
| Todos 11 models com `T_PB_` + prefixos de coluna | ✅ |
| `JsonNamingPolicy.CamelCase` global | ✅ |
| `MotorApiClient` (.NET → Java, best-effort) | ✅ |
| Migrations | ❌ nenhuma criada |
| Connection string com credenciais | ❌ placeholder vazio |

---

## Naming convention

| Camada | Convenção | Exemplo |
|---|---|---|
| Classes | PascalCase + sufixo `Entity` | `AnimalEntity`, `ConsultaEntity` |
| Propriedades | PascalCase | `Nome`, `DataNascimento`, `CondicaoCronica` |
| Tabelas | `T_PB_` + UPPER_SNAKE_CASE | `T_PB_ANIMAL`, `T_PB_RESPONSAVEL` |
| Colunas | prefixo semântico + UPPER_SNAKE_CASE | `NM_NOME_ANIMAL`, `DT_DATA_NASCIMENTO` |
| Timestamps (BaseEntity) | PascalCase EN | `CreatedAt` / `UpdatedAt` → `CA_CREATED_AT` / `AT_UPDATED_AT` |
| Enums | PascalCase nome, UPPER_SNAKE_CASE valores | `SexoEnum.MACHO` |
| Navigation props | `= null!` para obrigatórias, `?` para nullable | `public ResponsavelEntity Responsavel { get; set; } = null!;` |

**BaseEntity** — todos os models herdam. `CreatedAt` setado no `Added`, `UpdatedAt` no `Modified` via override de `SaveChangesAsync`. Nunca atribuir manualmente em services.

---

## Padrões de código

- **Data Annotations** exclusivamente (sem Fluent API)
- `[Key]`, `[Required]`, `[Column]`, `[Table]`, `[ForeignKey(nameof(...))]`
- `[JsonIgnore]` nas navegações para evitar circular reference na serialização
- `[EmailAddress]` para validação de email
- Controllers não expõem entity; usar DTOs por domínio.
- DTOs vivem em subpastas por domínio: `Dtos/Animal`, `Dtos/Responsavel`, `Dtos/Consulta`, `Dtos/JanelaAtendimento`.
- Services usam retornos simples (`Dto?`, `List<Dto>`, `bool`) e métodos explícitos de validação. Não usar `ServiceResult<T>` nem tuplas para fluxo de controller.
- Erros continuam simples no controller: `BadRequest("mensagem")`, `NotFound("mensagem")`, `Conflict("mensagem")`. Sem envelope `ErrorDto`, middleware ou exceções customizadas no .NET nesta sprint.
- Usar `AsNoTracking()` em consultas de leitura/validação; não usar quando carregar entity para alterar/remover.

---

## Entidades e tabelas

| Entidade | Tabela | Observação |
|---|---|---|
| `AnimalEntity` | `T_PB_ANIMAL` | FK → Responsavel, TipoAnimal |
| `ResponsavelEntity` | `T_PB_RESPONSAVEL` | FK → Clinica, Endereco (nullable) |
| `VeterinarioEntity` | `T_PB_VETERINARIO` | FK → Clinica |
| `ClinicaEntity` | `T_PB_CLINICA` | FK → Endereco |
| `ConsultaEntity` | `T_PB_CONSULTA` | FK → Animal, Veterinario, Clinica |
| `EnderecoEntity` | `T_PB_ENDERECO` | sem campo Pais (BR only) |
| `TipoAnimalEntity` | `T_PB_TIPO_ANIMAL` | Especie + Porte + Raca |
| `ProntuarioEntity` | `T_PB_PRONTUARIO` | FK → Animal |
| `ProcedimentoEntity` | `T_PB_PROCEDIMENTO` | FK → RegistroAtendimento, Animal, Veterinario |
| `JanelaAtendimentoEntity` | `T_PB_JANELA_ATENDIMENTO` | FK → Veterinario |
| `RegistroAtendimentoEntity` | `T_PB_REGISTRO_ATENDIMENTO` | FK → Animal, Prontuario, Consulta |

### Atenção: TipoAnimal é JOIN obrigatório para o motor Java

`especie` e `porte` do animal vivem em `TipoAnimalEntity`, não em `AnimalEntity` diretamente.
O endpoint `GET /api/animal/{id}/motor` precisa fazer JOIN com `TipoAnimal` para montar a resposta.

---

## Enums

| Enum | Valores |
|---|---|
| `SexoEnum` | MACHO, FEMEA |
| `EspecieEnum` | CACHORRO, GATO, PASSARO, COELHO, HAMSTER, OUTRO |
| `PorteEnum` | MINI, PEQUENO, MEDIO, GRANDE, GIGANTE |
| `StatusTutorEnum` | ATIVO, PRE_CADASTRO |
| `StatusConsultaEnum` | AGENDADA, CONFIRMADA, REALIZADA, CANCELADA, NAO_COMPARECEU |
| `TipoConsultaEnum` | TRIAGEM, ROTINA, VACINACAO, EXAME, RETORNO, EMERGENCIA |
| `TipoProcedimentoEnum` | VACINACAO, VERMIFUGACAO, EXAME_LABORATORIAL, EXAME_IMAGEM, CIRURGIA, INTERNACAO, OUTRO |
| `StatusProcedimentoEnum` | PENDENTE, REALIZADO, CANCELADO |

---

## O que o Java precisa deste serviço (PRD 04)

Endpoints que o `petbuddies-ai` vai chamar. Implementar com prioridade:

### Motor (chamados pelo MotorScoreService/MotorPlanoService)

| Método | Rota | Response campos |
|---|---|---|
| GET | `/api/animal/{id}/motor` | `id, nome, dataNascimento, condicaoCronica, castrado, preCadastro, sexo, especie, porte` |
| GET | `/api/animal/{id}/ultima-consulta` | `dataHora, tipo` (última consulta com status REALIZADA) |

> `especie` e `porte` vêm de `TipoAnimalEntity` via JOIN. `sexo` vem de `AnimalEntity`.

### Tools conversacionais (chamados pelo bot WhatsApp)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/responsavel/buscar/{telefone}` | Busca tutor por telefone |
| POST | `/api/responsavel` | Cria tutor (Status = PRE_CADASTRO) |
| POST | `/api/animal` | Cria animal (preCadastro = true) |
| GET | `/api/responsavel/{id}/animal` | Lista animais do tutor |
| GET | `/api/janela-atendimento` | Lista janelas disponíveis |
| POST | `/api/consulta` | Agenda consulta |
| GET | `/api/consulta?animalId=` | Lista consultas do animal |
| PATCH | `/api/consulta/{id}` | Cancela consulta |

---

## O que o .NET precisa do Java (PRD 05+)

O `.NET POST /api/animal` é o ponto canônico de criação de animal para WhatsApp, mobile futuro e uso manual. Após salvar o animal, o .NET deve chamar o Java:

| Método | Rota Java | Quando |
|---|---|---|
| POST | `/api/motor/planos/instanciar` | Após criar animal no .NET |

Regras:
- chamada síncrona best-effort;
- falha do Java não desfaz cadastro clínico;
- log simples com `ILogger`;
- sem endpoint manual de fallback nesta sprint;
- Java garante idempotência retornando plano existente quando já houver plano preventivo ATIVO.

PRD posterior (`09-eventos-clinicos-motor.md`) expande isso para score, consulta realizada e pós-cirúrgico.

---

## Decisões de domínio confirmadas

| # | Decisão |
|---|---|
| D1 | **Single-tenant.** `private const int ClinicaId = 1` nos services (`ConsultaService`, `ResponsavelService`). Sem env var. |
| D2 | **`ResponsavelEntity.EnderecoId` é `int?`.** Tutor PRE_CADASTRADO via WhatsApp não tem endereço residencial ainda. |
| D3 | **`ConsultaEntity.VeterinarioId`** ao agendar via bot: service busca `Janela.VeterinarioId` e usa diretamente. |
| D4 | **`TipoAnimalId` ao criar animal via bot:** service faz `FirstOrDefault` por `(Especie, Porte)` nos registros de `TipoAnimal` existentes. `Raca` fica vazia (`string.Empty`) quando cadastrado via bot. |
| D5 | **`preCadastro`** deve ser incluído em `AnimalMotorDto` — Java usa para decidir fluxo G1. |
| D6 | **Criação de animal é canônica no .NET.** Todo canal cria animal via `.NET POST /api/animal`; o .NET dispara plano preventivo no Java em best-effort. |

## Pendências antes da migration

1. **Connection string vazia** em `appsettings.Development.json` — preencher com credenciais Oracle FIAP
2. **`MotorApi:BaseUrl` no docker** — alinhar para `http://petbuddies-ai:8080` no `appsettings.Docker.json` quando o compose for criado

---

## Workflow de migration

```bash
# Criar migration (sem aplicar)
dotnet ef migrations add NomeDaMigration --project PetBuddies-API

# Aplicar
dotnet ef database update --project PetBuddies-API

# Verificar status
dotnet ef migrations list --project PetBuddies-API
```

Naming de migrations: `snake_case` com verbo (ex: `add_animal_controller`, `add_consulta_endpoints`).
