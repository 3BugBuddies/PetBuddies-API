# Spec 01 (.NET): CRUD Clinico

**Status:** Aprovado para implementacao  
**Data:** 2026-05-19  
**PRD:** `PetBuddies-API/.claude/docs/prds/01-crud-clinico.md`  
**Contrato:** `.claude/docs/contracts/petbuddies-rest-contracts.md`  
**Servico:** `PetBuddies-API` (.NET 8)

---

## Visao geral

Implementar os endpoints minimos do dominio clinico que o `petbuddies-ai` consome para cadastro e agendamento via WhatsApp:

- responsavel por telefone, criacao de responsavel e listagem de animais do responsavel
- criacao de animal em pre-cadastro
- listagem de janelas de atendimento disponiveis
- criacao, listagem e cancelamento de consultas

Nao executar migrations nesta spec. Gabriel vai criar e aplicar as migrations manualmente.

---

## Decisao de naming: singular em controller e rota

Usar classes de controller e rotas REST no singular, seguindo a convencao solicitada para o .NET e para o contrato consumido pelo Java:

- `AnimalController` -> `/api/animal`
- `ResponsavelController` -> `/api/responsavel`
- `JanelaAtendimentoController` -> `/api/janela-atendimento`
- `ConsultaController` -> `/api/consulta`

As entidades, services e DTOs tambem continuam no singular quando representam um item ou uma operacao especifica (`AnimalEntity`, `AnimalCadastroService`, `ConsultaDto`).

---

## T0 - Preparacao sem migration

Validar antes da implementacao:

- `ResponsavelEntity.EnderecoId` ja esta `int?`
- `AnimalMotorDto` ja possui `PreCadastro`
- `TipoAnimalEntity` esta mapeada para `pb_tb_tipo_animal`
- `JanelaAtendimentoEntity` esta mapeada para `pb_tb_janela_atendimento`
- `Program.cs` registra `JsonStringEnumConverter`

Nao fazer:

- `dotnet ef migrations add`
- `dotnet ef database update`
- alteracao de connection string com credenciais
- seed automatico via codigo

---

## T1 - DTOs

Criar DTOs em `PetBuddies-API/Dtos/Client` ou subpasta equivalente mantendo o padrao atual.

### ResponsavelDto

```csharp
public class ResponsavelDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
```

### CadastrarResponsavelRequest

```csharp
public class CadastrarResponsavelRequest
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    public int ClinicaId { get; set; }
}
```

### AnimalDto

```csharp
public class AnimalDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Porte { get; set; } = string.Empty;
    public string Sexo { get; set; } = string.Empty;
    public bool Castrado { get; set; }
    public bool PreCadastro { get; set; }
}
```

### CadastrarAnimalRequest

```csharp
public class CadastrarAnimalRequest
{
    [Required]
    public int ResponsavelId { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Especie { get; set; } = string.Empty;

    [Required]
    public string Porte { get; set; } = string.Empty;

    [Required]
    public string Sexo { get; set; } = string.Empty;

    public bool Castrado { get; set; }

    [Required]
    public DateOnly DataNascimento { get; set; }

    public bool PreCadastro { get; set; }
}
```

### JanelaAtendimentoDto

```csharp
public class JanelaAtendimentoDto
{
    public int Id { get; set; }
    public DateOnly Data { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public int VeterinarioId { get; set; }
    public string VeterinarioNome { get; set; } = string.Empty;
}
```

### ConsultaDto

```csharp
public class ConsultaDto
{
    public int Id { get; set; }
    public string TipoConsulta { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AnimalId { get; set; }
}
```

### AgendarConsultaRequest

```csharp
public class AgendarConsultaRequest
{
    [Required]
    public int AnimalId { get; set; }

    [Required]
    public int JanelaId { get; set; }

    [Required]
    public string TipoConsulta { get; set; } = string.Empty;
}
```

### AtualizarConsultaStatusRequest

```csharp
public class AtualizarConsultaStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;

    public string? Motivo { get; set; }
}
```

---

## T2 - Erros padronizados

Implementar contrato:

```json
{
  "error": {
    "code": "VALIDACAO_FALHOU",
    "message": "Mensagem legivel."
  }
}
```

Criar:

- `Dtos/Client/ErrorDto.cs`
- excecao de dominio simples com `Code` e `StatusCode`
- middleware ou filter global para converter excecoes conhecidas

Códigos obrigatórios:

| Code | HTTP |
|---|---|
| `RESPONSAVEL_NAO_ENCONTRADO` | 404 |
| `RESPONSAVEL_TELEFONE_DUPLICADO` | 409 |
| `TIPO_ANIMAL_NAO_CADASTRADO` | 400 |
| `JANELA_OCUPADA` | 409 |
| `CONSULTA_JA_REALIZADA` | 409 |
| `VALIDACAO_FALHOU` | 400 |

---

## T3 - ResponsavelController

Arquivo: `Controllers/ResponsavelController.cs`

Rota base: `[Route("api/responsavel")]`

Endpoints:

- `GET /api/responsavel/buscar/{telefone}`
- `POST /api/responsavel`
- `GET /api/responsavel/{id:int}/animal`

Regras:

- normalizar telefone para apenas digitos antes de buscar/salvar
- telefone duplicado retorna 409 `RESPONSAVEL_TELEFONE_DUPLICADO`
- POST cria `Status = PRE_CADASTRO`
- POST deixa `EnderecoId = null`
- `CreatedAt = DateTime.Now`
- listagem de animais usa `Include(animal => animal.TipoAnimal)`
- tutor sem animais retorna 200 com array vazio

---

## T4 - AnimalController: adicionar POST

Arquivo existente a renomear: `Controllers/AnimaisController.cs` -> `Controllers/AnimalController.cs`

Adicionar:

- `POST /api/animal`

Regras:

- validar se `ResponsavelId` existe
- converter strings `Especie`, `Porte`, `Sexo` para enums com parse case-insensitive
- resolver `TipoAnimalId` por `Especie` + `Porte`
- quando nao achar combinacao, retornar 400 `TIPO_ANIMAL_NAO_CADASTRADO`
- criar `AnimalEntity` com `PreCadastro` do request, `CreatedAt = DateTime.Now`
- campos sem origem no bot ficam com defaults seguros do modelo atual (`Peso = 0`, `CondicaoCronica = false`, `Foto = null`)
- retornar 201 com `AnimalDto`

---

## T5 - JanelaAtendimentoController

Arquivo: `Controllers/JanelaAtendimentoController.cs`

Rota base: `[Route("api/janela-atendimento")]`

Endpoint:

- `GET /api/janela-atendimento`

Regras:

- filtrar `Data >= DateOnly.FromDateTime(DateTime.Now)`
- considerar disponivel quando nao existe consulta na mesma janela com status diferente de `CANCELADA`
- como `ConsultaEntity` nao possui `JanelaAtendimentoId`, disponibilidade deve ser calculada por `DataHora == Data + HoraInicio` e mesmo `VeterinarioId`
- usar `Include(janela => janela.Veterinario)`
- ordenar por `Data`, depois `HoraInicio`
- retornar `JanelaAtendimentoDto`

---

## T6 - ConsultaController

Arquivo: `Controllers/ConsultaController.cs`

Rota base: `[Route("api/consulta")]`

Endpoints:

- `POST /api/consulta`
- `GET /api/consulta?animalId={id}`
- `PATCH /api/consulta/{id:int}`

### POST /api/consulta

Regras:

- validar se animal existe
- buscar janela por `JanelaId`
- verificar disponibilidade com mesma regra do T5
- converter `TipoConsulta` para enum
- criar consulta com:
  - `Status = AGENDADA`
  - `DataHora = janela.Data + janela.HoraInicio`
  - `VeterinarioId = janela.VeterinarioId`
  - `ClinicaId` vindo de configuracao
  - `Emergencia = false`
  - `Prioridade = false`
  - `CreatedAt = DateTime.Now`
  - `UpdatedAt = DateTime.Now`
- retornar 201 com `ConsultaDto`

### GET /api/consulta?animalId={id}

Regras:

- filtrar por `AnimalId`
- ordenar por `DataHora desc`
- retornar lista de `ConsultaDto`

### PATCH /api/consulta/{id}

Regras:

- aceitar apenas cancelamento nesta sprint (`Status = CANCELADA`)
- consulta inexistente retorna 404
- se `Status == REALIZADA`, retornar 409 `CONSULTA_JA_REALIZADA`
- atualizar `Status`, `Motivo` e `UpdatedAt`
- retornar 200 com `ConsultaDto`

---

## T7 - Configuracao

Adicionar em `appsettings.json` e `appsettings.Development.json` sem credenciais:

```json
{
  "Clyvo": {
    "ClinicaId": 1
  }
}
```

Implementacao pode ler por `IConfiguration["Clyvo:ClinicaId"]`.

Para Docker, usar `CLYVO__CLINICAID=1`.

---

## T8 - Swagger

Adicionar annotations nos endpoints novos:

- `[ProducesResponseType]` para 200/201/400/404/409 conforme contrato
- `[SwaggerOperation(Summary = "...")]`

Manter `c.EnableAnnotations()` em `Program.cs`.

---

## Checklist de implementacao

- [ ] DTOs criados
- [ ] contrato global de erro implementado
- [ ] `ResponsavelController` criado
- [ ] `AnimaisController` renomeado para `AnimalController`
- [ ] `POST /api/animal` adicionado ao `AnimalController`
- [ ] `JanelaAtendimentoController` criado
- [ ] `ConsultaController` criado
- [ ] `Clyvo:ClinicaId` configurado sem credenciais
- [ ] Swagger documenta os endpoints novos
- [ ] `dotnet build PetBuddies-API.slnx` sem erros
- [ ] migrations nao executadas pelo Codex

---

## Checklist manual do Gabriel

- [ ] preencher connection string local
- [ ] criar migration
- [ ] aplicar migration
- [ ] executar seed manual no Oracle
- [ ] validar no Swagger/Postman
