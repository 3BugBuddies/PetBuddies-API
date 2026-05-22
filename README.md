# PetBuddies API — Challenge FIAP 2026 | .NET Advanced

API REST de domínio clínico veterinário desenvolvida com ASP.NET Core e EF Core, como parte do Challenge da disciplina de **Advanced Business Development with .NET (2TDS)** — FIAP 2026.

O serviço gerencia clínica, veterinários, tutores, animais, janelas de atendimento e consultas. É consumido pelo bot WhatsApp (`petbuddies-ai`) e dispara automaticamente o motor de cuidado preventivo Java ao cadastrar um novo animal.

---

## Integrantes do Grupo

| Nome | RM |
|------|----|
| Felipe Yuiti Ishii | 565339 |
| Gabriel Nogueira Peixoto | 563925 |
| Giovanna Neri dos Santos | 566154 |
| Mariana Inoue | 565834 |

---

## Stack e Dependências

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| Microsoft.EntityFrameworkCore | 8.0.26 | ORM principal |
| Oracle.EntityFrameworkCore | 8.23.26200 | Driver Oracle para EF Core |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger / OpenAPI UI |
| Microsoft.AspNetCore.OpenApi | 8.0.26 | Suporte OpenAPI nativo |

---

## Estrutura do Projeto

```
PetBuddies-API/
├── Controllers/
│   ├── AnimalController.cs              # /api/animal
│   ├── ResponsavelController.cs         # /api/responsavel
│   ├── ConsultaController.cs            # /api/consulta
│   └── JanelaAtendimentoController.cs   # /api/janela-atendimento
├── Data/
│   └── ApplicationContext.cs            # DbContext — auto-timestamps via SaveChangesAsync
├── Dtos/
│   ├── Animal/          # AnimalDto, AnimalMotorDto, UltimaConsultaDto,
│   │                    # CadastrarAnimalRequest, AtualizarAnimalRequest
│   ├── Responsavel/     # ResponsavelDto, CadastrarResponsavelRequest
│   ├── Consulta/        # ConsultaDto, AgendarConsultaRequest,
│   │                    # AtualizarConsultaRequest, AtualizarConsultaStatusRequest
│   └── JanelaAtendimento/ # JanelaAtendimentoDto, SalvarJanelaAtendimentoRequest
├── Enums/               # 8 enums de domínio (SexoEnum, EspecieEnum, PorteEnum, ...)
├── Models/
│   ├── BaseEntity.cs    # CreatedAt / UpdatedAt automáticos
│   └── *Entity.cs       # 11 entidades EF Core
├── Services/
│   ├── AnimalCadastroService.cs
│   ├── AnimalMotorService.cs
│   ├── ConsultaService.cs
│   ├── JanelaAtendimentoService.cs
│   ├── MotorApiClient.cs          # Integração .NET → Java motor (best-effort)
│   └── ResponsavelService.cs
├── appsettings.json
└── Program.cs
```

---

## Modelo de Dados

### Entidades e Tabelas

| Entidade | Tabela | Relacionamentos |
|----------|--------|-----------------|
| `AnimalEntity` | `T_PB_ANIMAL` | → Responsavel, TipoAnimal |
| `ResponsavelEntity` | `T_PB_RESPONSAVEL` | → Clinica, Endereco (nullable) |
| `VeterinarioEntity` | `T_PB_VETERINARIO` | → Clinica |
| `ClinicaEntity` | `T_PB_CLINICA` | → Endereco |
| `ConsultaEntity` | `T_PB_CONSULTA` | → Animal, Veterinario, Clinica |
| `EnderecoEntity` | `T_PB_ENDERECO` | — |
| `TipoAnimalEntity` | `T_PB_TIPO_ANIMAL` | Especie + Porte |
| `ProntuarioEntity` | `T_PB_PRONTUARIO` | → Animal |
| `ProcedimentoEntity` | `T_PB_PROCEDIMENTO` | → RegistroAtendimento, Animal, Veterinario |
| `JanelaAtendimentoEntity` | `T_PB_JANELA_ATENDIMENTO` | → Veterinario |
| `RegistroAtendimentoEntity` | `T_PB_REGISTRO_ATENDIMENTO` | → Animal, Prontuario, Consulta |

### Enums

| Enum | Valores |
|------|---------|
| `SexoEnum` | `MACHO`, `FEMEA` |
| `EspecieEnum` | `CACHORRO`, `GATO`, `PASSARO`, `COELHO`, `HAMSTER`, `OUTRO` |
| `PorteEnum` | `MINI`, `PEQUENO`, `MEDIO`, `GRANDE`, `GIGANTE` |
| `StatusTutorEnum` | `ATIVO`, `PRE_CADASTRO` |
| `StatusConsultaEnum` | `AGENDADA`, `CONFIRMADA`, `REALIZADA`, `CANCELADA`, `NAO_COMPARECEU` |
| `TipoConsultaEnum` | `TRIAGEM`, `ROTINA`, `VACINACAO`, `EXAME`, `RETORNO`, `EMERGENCIA` |
| `TipoProcedimentoEnum` | `VACINACAO`, `VERMIFUGACAO`, `EXAME_LABORATORIAL`, `EXAME_IMAGEM`, `CIRURGIA`, `INTERNACAO`, `OUTRO` |
| `StatusProcedimentoEnum` | `PENDENTE`, `REALIZADO`, `CANCELADO` |

---

## Configuração do Banco de Dados

Oracle disponibilizado pela FIAP. Crie `appsettings.Development.json` com suas credenciais:

```json
{
  "ConnectionStrings": {
    "Oracle": "Data Source=oracle.fiap.com.br:1521/ORCL;User Id=<SEU_RM>;Password=<SUA_SENHA>;"
  }
}
```

As tabelas são criadas automaticamente via `ddl-auto` configurado no `ApplicationContext`. Migrations EF Core também estão disponíveis:

```bash
dotnet ef migrations add initial_schema --project PetBuddies-API
dotnet ef database update --project PetBuddies-API
```

---

## Como Executar

### Pré-requisitos

- .NET 8 SDK
- Acesso ao Oracle FIAP (VPN ou rede local)

### Rodando localmente

```bash
# Clonar o repositório
git clone <URL_DO_REPOSITORIO>
cd PetBuddies-API/PetBuddies-API

# Criar appsettings.Development.json com suas credenciais Oracle
# (ver seção "Configuração do Banco de Dados")

# Executar
dotnet run
```

A aplicação sobe em:
- **HTTP:** `http://localhost:5297`
- **Swagger UI:** `http://localhost:5297/swagger`

---

## Endpoints da API

Base URL: `http://localhost:5297`

### Responsável — `/api/responsavel`

| Método | Rota | Status | Descrição |
|--------|------|--------|-----------|
| `GET` | `/api/responsavel` | 200 | Lista todos os responsáveis |
| `GET` | `/api/responsavel/{id}` | 200 / 404 | Busca por ID |
| `GET` | `/api/responsavel/buscar/{telefone}` | 200 / 404 | Busca por telefone (aceita com ou sem formatação) |
| `GET` | `/api/responsavel/{id}/animal` | 200 / 404 | Lista animais vinculados ao responsável |
| `POST` | `/api/responsavel` | 201 / 400 / 409 | Cadastra responsável em pré-cadastro |
| `PUT` | `/api/responsavel/{id}` | 204 / 400 / 404 / 409 | Atualiza responsável |
| `DELETE` | `/api/responsavel/{id}` | 204 / 404 / 409 | Remove responsável |

**POST `/api/responsavel` — Request:**
```json
{
  "nome": "João Silva",
  "telefone": "11999887766"
}
```

**Response 201:**
```json
{
  "id": 1,
  "nome": "João Silva",
  "telefone": "11999887766",
  "status": "PRE_CADASTRO"
}
```

---

### Animal — `/api/animal`

| Método | Rota | Status | Descrição |
|--------|------|--------|-----------|
| `GET` | `/api/animal` | 200 | Lista todos os animais |
| `GET` | `/api/animal/{id}` | 200 / 404 | Busca por ID |
| `GET` | `/api/animal/{id}/motor` | 200 / 404 | Dados completos para o motor Java (especie, porte, sexo, etc.) |
| `GET` | `/api/animal/{id}/ultima-consulta` | 200 / 404 | Última consulta com status `REALIZADA` |
| `POST` | `/api/animal` | 201 / 400 / 404 | Cadastra animal — dispara plano preventivo no motor em best-effort |
| `PUT` | `/api/animal/{id}` | 204 / 400 / 404 | Atualiza animal |
| `DELETE` | `/api/animal/{id}` | 204 / 404 / 409 | Remove animal |

**POST `/api/animal` — Request:**
```json
{
  "responsavelId": 1,
  "nome": "Rex",
  "especie": "CACHORRO",
  "porte": "GRANDE",
  "sexo": "MACHO",
  "castrado": true,
  "dataNascimento": "2020-03-15"
}
```

**Response 201:**
```json
{
  "id": 1,
  "nome": "Rex",
  "especie": "CACHORRO",
  "porte": "GRANDE",
  "sexo": "MACHO",
  "castrado": true,
  "preCadastro": true
}
```

> Após salvar o animal, o serviço chama `POST /api/motor/planos/instanciar` no `petbuddies-ai` de forma síncrona best-effort. Falha no motor Java não desfaz o cadastro clínico.

---

### Janela de Atendimento — `/api/janela-atendimento`

| Método | Rota | Status | Descrição |
|--------|------|--------|-----------|
| `GET` | `/api/janela-atendimento` | 200 | Lista janelas **disponíveis** (sem consulta, a partir de hoje) |
| `GET` | `/api/janela-atendimento/todas` | 200 | Lista **todas** as janelas cadastradas |
| `GET` | `/api/janela-atendimento/{id}` | 200 / 404 | Busca por ID |
| `POST` | `/api/janela-atendimento` | 201 / 400 / 404 / 409 | Cadastra janela para veterinário |
| `PUT` | `/api/janela-atendimento/{id}` | 204 / 400 / 404 / 409 | Atualiza janela |
| `DELETE` | `/api/janela-atendimento/{id}` | 204 / 404 / 409 | Remove janela |

**POST `/api/janela-atendimento` — Request:**
```json
{
  "data": "2026-06-15",
  "horaInicio": "09:00:00",
  "horaFim": "09:30:00",
  "duracaoSlot": 30,
  "veterinarioId": 1
}
```

---

### Consulta — `/api/consulta`

| Método | Rota | Status | Descrição |
|--------|------|--------|-----------|
| `GET` | `/api/consulta` | 200 | Lista todas as consultas |
| `GET` | `/api/consulta?animalId={id}` | 200 | Filtra consultas por animal |
| `GET` | `/api/consulta/{id}` | 200 / 404 | Busca por ID |
| `POST` | `/api/consulta` | 201 / 400 / 404 / 409 | Agenda consulta em janela disponível |
| `PUT` | `/api/consulta/{id}` | 204 / 400 / 404 / 409 | Atualiza dados da consulta |
| `PATCH` | `/api/consulta/{id}` | 200 / 400 / 404 / 409 | Cancela consulta |
| `DELETE` | `/api/consulta/{id}` | 204 / 404 / 409 | Remove consulta |

**POST `/api/consulta` — Request:**
```json
{
  "animalId": 1,
  "janelaId": 1,
  "tipoConsulta": "ROTINA"
}
```

**Response 201:**
```json
{
  "id": 1,
  "tipoConsulta": "ROTINA",
  "dataHora": "2026-06-15T09:00:00",
  "status": "AGENDADA",
  "animalId": 1
}
```

**PATCH `/api/consulta/{id}` — Request:**
```json
{
  "status": "CANCELADA",
  "motivo": "Tutor não pode comparecer"
}
```

---

## Seed — Dados Iniciais Necessários

Antes de usar os endpoints, executar no Oracle FIAP (ordem importa por FKs):

```sql
-- 1. Endereço
INSERT INTO T_PB_ENDERECO (ID_ENDERECO, LG_LOGRADOURO, NR_NUMERO, BR_BAIRRO, CD_CIDADE, ES_ESTADO, NR_CEP, CA_CREATED_AT)
VALUES (1, 'Av. Paulista', '1000', 'Bela Vista', 'São Paulo', 'SP', '01310100', SYSDATE);

-- 2. Clínica (id=1 — single-tenant)
INSERT INTO T_PB_CLINICA (ID_CLINICA, NM_NOME_CLINICA, NR_CNPJ, TL_TELEFONE, ID_ENDERECO, CA_CREATED_AT)
VALUES (1, 'PetBuddies Clínica Veterinária', '00000000000000', '11999999999', 1, SYSDATE);

-- 3. TipoAnimal (pelo menos as combinações que serão usadas)
INSERT INTO T_PB_TIPO_ANIMAL (ID_TIPO_ANIMAL, ES_ESPECIE, PT_PORTE, RC_RACA) VALUES (1, 'CACHORRO', 'GRANDE', '');
INSERT INTO T_PB_TIPO_ANIMAL (ID_TIPO_ANIMAL, ES_ESPECIE, PT_PORTE, RC_RACA) VALUES (2, 'CACHORRO', 'MEDIO', '');
INSERT INTO T_PB_TIPO_ANIMAL (ID_TIPO_ANIMAL, ES_ESPECIE, PT_PORTE, RC_RACA) VALUES (3, 'GATO', 'PEQUENO', '');

-- 4. Veterinário
INSERT INTO T_PB_VETERINARIO (ID_VETERINARIO, NM_NOME_VETERINARIO, NR_CRMV, TL_TELEFONE, AT_ATIVO, ID_CLINICA, CA_CREATED_AT)
VALUES (1, 'Dra. Ana Lima', '12345-SP', '11988887777', 1, 1, SYSDATE);

COMMIT;
```

---

## Testes — Postman

A coleção Postman com todos os endpoints está disponível em:

```
postman/PetBuddies-API.postman_collection.json
```

Importar no Postman e configurar a variável `base_url` para `http://localhost:5297`.

---

## Integração com petbuddies-ai (Java)

Este serviço é chamado pelo bot WhatsApp para cadastro, consulta e agendamento. Também expõe endpoints específicos para o motor de personalização Java:

| Endpoint | Consumidor | Finalidade |
|----------|-----------|------------|
| `GET /api/animal/{id}/motor` | `petbuddies-ai` MotorScoreService | Dados do animal para cálculo de risco |
| `GET /api/animal/{id}/ultima-consulta` | `petbuddies-ai` MotorPlanoService | Data/tipo da última consulta realizada |
| `POST /api/responsavel` | `petbuddies-ai` CadastroTools | Criar tutor via WhatsApp |
| `POST /api/animal` | `petbuddies-ai` CadastroTools | Criar animal via WhatsApp |

---

## Tecnologias Utilizadas

- **.NET 8.0** / ASP.NET Core
- **Entity Framework Core 8.0.26** + **Oracle.EntityFrameworkCore 8.23.26200**
- **Oracle Database** (FIAP)
- **Swashbuckle 6.6.2** (Swagger UI + Annotations)
- **Data Annotations** para Bean Validation
- **System.Text.Json** com `CamelCase` + `JsonStringEnumConverter`
