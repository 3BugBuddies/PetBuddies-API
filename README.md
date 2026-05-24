# PetBuddies API — Challenge FIAP 2026 | .NET

API REST de domínio clínico veterinário desenvolvida com ASP.NET Core e EF Core, como parte do Challenge da disciplina de **Advanced Business Development with .NET (2TDS)** — FIAP 2026.

O serviço gerencia clínica, endereços, veterinários, tutores, animais, tipos de animal, janelas de atendimento, consultas, prontuários, registros de atendimento e procedimentos. É consumido pelo bot WhatsApp `petbuddies-ai` (Java) e dispara automaticamente o motor de cuidado preventivo Java ao cadastrar um novo animal.


## Integrantes do Grupo

| Nome | RM |
|------|----|
| Felipe Yuiti Ishii | 565339 |
| Gabriel Nogueira Peixoto | 563925 |
| Giovanna Neri dos Santos | 566154 |
| Mariana Inoue | 565834 |


## Stack e Dependências

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| Microsoft.EntityFrameworkCore | 8.0.26 | ORM principal |
| Oracle.EntityFrameworkCore | 8.23.26200 | Driver Oracle para EF Core |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger / OpenAPI UI |
| Swashbuckle.AspNetCore.Annotations | 6.6.2 | Anotações do Swagger |

---

## Avaliação isolada — endpoints clínicos

> Devido à relação de algumas classes com o Java, o teste de ponta a ponta completo exige o outro serviço rodando. Para a avaliação isolada da API .NET, a coleção Postman cobre os recursos clínicos independentes listados abaixo.

### Setup rápido para o avaliador

**1. Preencher a connection string Oracle** em `PetBuddies-API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Oracle": "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=ORCL)));User Id=;Password=;Max Pool Size=3;Min Pool Size=1"
  }
}
```

> Preencha `User Id` com seu RM (ex: `rm123456`) e `Password` com a senha do Oracle FIAP.

**2. Executar** (as migrations rodam automaticamente no startup via `Database.Migrate()`):

```bash
cd PetBuddies-API
dotnet run --project PetBuddies-API
```

> Alternativa manual: `dotnet ef database update --project PetBuddies-API`

**3. Swagger:** `http://localhost:5297/swagger`

**4. Postman:** importar `docs/postman/petbuddies-api-net.postman_collection.json` (dentro deste repo)

Execute as pastas na ordem em que aparecem na coleção: **Endereco → Clinica → Veterinario → JanelaAtendimento**.

### Contexto dos recursos testados

O domínio clínico é a base operacional do PetBuddies. Ele guarda os dados que permitem cadastrar unidades de atendimento, vincular profissionais e disponibilizar horários reais para consultas.

- **Endereco** representa a localização física usada pela clínica. Ele é criado primeiro porque a clínica depende desse vínculo.

- **Clinica** é a unidade de atendimento do sistema. Nesta sprint, ela funciona como base single-tenant para os fluxos clínicos e conversacionais.

- **Veterinario** é o profissional vinculado à clínica. O cadastro valida a referência da clínica e impede CRMV duplicado dentro da mesma unidade.

- **JanelaAtendimento** é a agenda disponível do veterinário. Cada janela informa início, fim, duração do slot e o veterinário responsável pelo horário.

- Quando o bot WhatsApp agenda uma consulta, ele consome justamente as janelas disponíveis expostas por esta API. Por isso esta cadeia é a base clínica do fluxo de agendamento.

- Esta seção pode ser testada isoladamente: ela valida CRUD, FKs, conflitos, respostas e ordenação correta de dependências sem precisar do WhatsApp, da Evolution API ou do serviço Java.

- A coleção Postman captura `enderecoId`, `clinicaId`, `veterinarioId` e `janelaId` automaticamente nas variáveis de coleção.

### Rotas testadas na coleção Postman

Execute os recursos na ordem em que aparecem na coleção, pois cada recurso depende do ID gerado pelo anterior.

| Recurso | Rotas cobertas | Principais cenários | Status codes |
|---|---|---|---|
| `Endereco` | `POST /api/endereco`<br>`GET /api/endereco`<br>`GET /api/endereco/{id}`<br>`PUT /api/endereco/{id}`<br>`DELETE /api/endereco/{id}` | Criação<br>Listagem<br>Busca por ID<br>Atualização<br>Remoção<br>Payload inválido<br>ID inexistente | `201 Created`<br>`200 OK`<br>`204 No Content`<br>`400 Bad Request`<br>`404 Not Found` |
| `Clinica` | `POST /api/clinica`<br>`GET /api/clinica`<br>`GET /api/clinica/{id}`<br>`GET /api/clinica/buscar?nome=`<br>`PUT /api/clinica/{id}`<br>`DELETE /api/clinica/{id}` | Vínculo com endereço<br>Busca por nome<br>CNPJ inválido<br>CNPJ duplicado<br>FK inexistente<br>Operações CRUD | `201 Created`<br>`200 OK`<br>`204 No Content`<br>`400 Bad Request`<br>`404 Not Found`<br>`409 Conflict` |
| `Veterinario` | `POST /api/veterinario`<br>`GET /api/veterinario`<br>`GET /api/veterinario/{id}`<br>`GET /api/veterinario/por-clinica/{clinicaId}`<br>`PUT /api/veterinario/{id}`<br>`DELETE /api/veterinario/{id}` | Vínculo com clínica<br>Filtro por clínica<br>CRMV duplicado na clínica<br>FK inexistente<br>Operações CRUD | `201 Created`<br>`200 OK`<br>`204 No Content`<br>`400 Bad Request`<br>`404 Not Found`<br>`409 Conflict` |
| `JanelaAtendimento` | `POST /api/janela-atendimento`<br>`GET /api/janela-atendimento`<br>`GET /api/janela-atendimento/todas`<br>`GET /api/janela-atendimento/{id}`<br>`PUT /api/janela-atendimento/{id}`<br>`DELETE /api/janela-atendimento/{id}` | Vínculo com veterinário<br>Horários disponíveis<br>Listagem completa<br>Conflito de horário<br>Intervalo inválido<br>Operações CRUD | `201 Created`<br>`200 OK`<br>`204 No Content`<br>`400 Bad Request`<br>`404 Not Found`<br>`409 Conflict` |


### Validações e respostas esperadas

| Situação | Exemplo na coleção | Retorno esperado |
|---|---|---|
| Lista sem registros | `GET /api/endereco` antes de haver dados | `204 No Content` |
| Busca por ID inexistente | `GET /api/clinica/999999` | `404 Not Found` |
| Payload inválido | `POST /api/endereco` sem `logradouro` | `400 Bad Request` |
| FK inexistente | `POST /api/clinica` com `enderecoId` inexistente | `404 Not Found` |
| Duplicidade de negócio | CNPJ, CRMV ou horário já cadastrado | `409 Conflict` |
| Atualização com sucesso | `PUT /api/veterinario/{id}` | `204 No Content` |
| Remoção com sucesso | `DELETE /api/janela-atendimento/{id}` | `204 No Content` |


## Estrutura do Projeto

```
PetBuddies-API/
├── docs/
│   └── postman/
│       └── petbuddies-api-net.postman_collection.json
├── PetBuddies-API/
│   ├── Controllers/     # 11 controllers REST por domínio
│   ├── Data/
│   │   ├── ApplicationContext.cs
│   │   └── Migrations/  # Migrations EF Core
│   ├── Dtos/            # DTOs e requests organizados por domínio
│   ├── Enums/           # 10 enums de domínio
│   ├── Models/          # BaseEntity + 11 entidades EF Core
│   ├── Services/        # Services de domínio + MotorApiClient
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
├── Dockerfile
└── README.md
```


## Modelo de Dados

### Entidades e Tabelas

#### Tabelas independentes — Sem ligação com Java

| Entidade | Tabela | Relacionamentos |
|----------|--------|-----------------|
| `VeterinarioEntity` | `T_PB_VETERINARIO` | → Clinica |
| `ClinicaEntity` | `T_PB_CLINICA` | → Endereco |
| `EnderecoEntity` | `T_PB_ENDERECO` | — |
| `TipoAnimalEntity` | `T_PB_TIPO_ANIMAL` | Especie + Porte |
| `JanelaAtendimentoEntity` | `T_PB_JANELA_ATENDIMENTO` | → Veterinario |

#### Tabelas dependentes — Com ligação ao Java

| Entidade | Tabela | Relacionamentos |
|----------|--------|-----------------|
| `AnimalEntity` | `T_PB_ANIMAL` | → Responsavel, TipoAnimal |
| `ResponsavelEntity` | `T_PB_RESPONSAVEL` | → Clinica, Endereco (nullable) |
| `ConsultaEntity` | `T_PB_CONSULTA` | → Animal, Veterinario, Clinica |

#### Tabelas dependentes de tabelas relacionadas ao Java

| Entidade | Tabela | Relacionamentos |
|----------|--------|-----------------|
| `ProntuarioEntity` | `T_PB_PRONTUARIO` | → Animal |
| `ProcedimentoEntity` | `T_PB_PROCEDIMENTO` | → RegistroAtendimento, Animal, Veterinario |
| `RegistroAtendimentoEntity` | `T_PB_REGISTRO_ATENDIMENTO` | → Animal, Prontuario, Consulta |

### Enums

| Enum | Valores |
|------|---------|
| `SexoEnum` | `MACHO`, `FEMEA` |
| `EspecieEnum` | `CACHORRO`, `GATO`, `PASSARO`, `COELHO`, `HAMSTER`, `OUTRO` |
| `PorteEnum` | `MINI`, `PEQUENO`, `MEDIO`, `GRANDE`, `GIGANTE` |
| `StatusTutorEnum` | `ATIVO`, `PRE_CADASTRO` |
| `StatusConsultaEnum` | `AGENDADA`, `CONFIRMADA`, `REALIZADA`, `CANCELADA`, `NAO_COMPARECEU` |
| `CategoriaProtocoloEnum` | `PREVENTIVO`, `POS_CIRURGICO` |
| `StatusPlanoEnum` | `ATIVO`, `CONCLUIDO`, `CANCELADO` |
| `TipoConsultaEnum` | `TRIAGEM`, `ROTINA`, `VACINACAO`, `EXAME`, `RETORNO`, `EMERGENCIA` |
| `TipoProcedimentoEnum` | `VACINACAO`, `VERMIFUGACAO`, `EXAME_LABORATORIAL`, `EXAME_IMAGEM`, `CIRURGIA`, `INTERNACAO`, `OUTRO` |
| `StatusProcedimentoEnum` | `PENDENTE`, `REALIZADO`, `CANCELADO` |

---

## Configuração do Banco de Dados

Oracle disponibilizado pela FIAP. O arquivo `PetBuddies-API/appsettings.Development.json` fica versionado com `User Id` e `Password` vazios; preencha esses campos localmente antes de executar.

No startup, o `Program.cs` executa `Database.Migrate()` e aplica as migrations EF Core pendentes. Para rodar manualmente:

```bash
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
git clone https://github.com/3BugBuddies/PetBuddies-API
cd PetBuddies-API

# Preencher User Id e Password em PetBuddies-API/appsettings.Development.json
# (ver seção "Configuração do Banco de Dados")

# Executar
dotnet run --project PetBuddies-API
```

A aplicação sobe em:
- **HTTP:** `http://localhost:5297`
- **Swagger UI:** `http://localhost:5297/swagger`

---

## Tecnologias Utilizadas

- **.NET 8.0** / ASP.NET Core
- **Entity Framework Core 8.0.26** + **Oracle.EntityFrameworkCore 8.23.26200**
- **Oracle Database** (FIAP)
- **Swashbuckle 6.6.2** (Swagger UI + Annotations)
- **Data Annotations** para validação de requests
- **System.Text.Json** com `CamelCase` + `JsonStringEnumConverter`
