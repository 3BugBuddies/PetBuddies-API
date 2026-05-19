# PRD 01 (.NET): CRUD Clínico

**Status:** Rascunho
**Data:** 2026-05-19
**Serviço:** `PetBuddies-API` (.NET 8)
**Consumido por:** `petbuddies-ai` (Java) — contrato em `clyvo/.claude/docs/contracts/petbuddies-rest-contracts.md`

---

## Contexto

O `PetBuddies-API` já tem os 2 endpoints do motor (`GET /api/animal/{id}/motor` e `GET /api/animal/{id}/ultima-consulta`) e todas as 11 entidades modeladas. Falta expor os 7 endpoints de CRUD que o bot WhatsApp (Java) vai consumir para registrar tutores, animais, agendar e cancelar consultas.

Este PRD entrega esses endpoints seguindo o contrato compartilhado e respeitando as decisões de domínio já registradas (single-tenant, `EnderecoId` nullable, `TipoAnimal` lookup automático, etc.).

---

## O que precisa ser feito

### 1. Migration inicial

Antes de qualquer controller, garantir que o schema está no Oracle:

1. Preencher connection string em `appsettings.Development.json`
2. Resolver `[Table("")]` em `ProcedimentoEntity` e `RegistroAtendimentoEntity` (ou marcar como `Ignore` se não vão ser usados nesta sprint)
3. Confirmar que `ResponsavelEntity.EnderecoId` é `int?` (feito ✅)
4. `dotnet ef migrations add init`
5. `dotnet ef database update`

### 2. ResponsavelController

Controller em `Controllers/ResponsavelController.cs`:

| Método | Rota | Comportamento |
|---|---|---|
| GET | `/api/responsavel/buscar/{telefone}` | Busca por telefone, retorna 200/404 |
| POST | `/api/responsavel` | Cria com `Status=PRE_CADASTRO`, retorna 201 com `ResponsavelDto` |
| GET | `/api/responsavel/{id}/animal` | Lista animais via Include(TipoAnimal), retorna 200 com `List<AnimalDto>` |

DTOs:
- `ResponsavelDto` — `id, nome, telefone, status`
- `CadastrarResponsavelRequest` — `nome (required), telefone (required), clinicaId (required)`

Validações:
- `[Required]` em nome e telefone
- Verificar duplicata por telefone → retorna 409 `RESPONSAVEL_TELEFONE_DUPLICADO`

### 3. AnimalController — adicionar POST

Adicionar endpoint `POST /api/animal` no controller existente:

- Body `CadastrarAnimalRequest` — `responsavelId, nome, especie (string), porte (string), sexo (string), castrado, dataNascimento, preCadastro`
- Service `AnimalCadastroService`:
  1. Resolver `TipoAnimalId` via `TiposAnimal.FirstOrDefault(t => t.Especie == enumEspecie && t.Porte == enumPorte)`
  2. Se não achar, retornar 400 `TIPO_ANIMAL_NAO_CADASTRADO`
  3. Criar `AnimalEntity` com `CreatedAt = DateTime.Now` (timezone São Paulo)
  4. Persistir
  5. Retornar `AnimalDto` (mapeado com `TipoAnimal.Especie.ToString()` e `Porte.ToString()`)

Validações:
- `[Required]` em todos os campos
- Verificar se `ResponsavelId` existe

### 4. JanelaAtendimentoController

`GET /api/janela-atendimento`:
- Filtros automáticos: `Data >= DateOnly.FromDateTime(DateTime.Now)` e não existe `Consulta` com `Status != CANCELADA` na mesma janela
- Include(Veterinario) para preencher `veterinarioNome`
- Order by `Data ASC, HoraInicio ASC`
- Retorna `List<JanelaAtendimentoDto>` — `id, data, horaInicio, horaFim, veterinarioId, veterinarioNome`

### 5. ConsultaController

| Método | Rota | Comportamento |
|---|---|---|
| POST | `/api/consulta` | Body `{animalId, janelaId, tipoConsulta}` → service busca janela, deriva `VeterinarioId` e `ClinicaId`, cria com `Status=AGENDADA`, `DataHora=Data+HoraInicio` |
| GET | `/api/consulta?animalId={id}` | Lista consultas do animal, order desc por DataHora |
| PATCH | `/api/consulta/{id}` | Body `{status, motivo}` → atualiza status. Se já `REALIZADA`, retorna 409 |

Service:
- `AgendarConsultaService` valida janela disponível, deriva FKs, cria consulta
- `CancelarConsultaService` busca, valida status, atualiza

### 6. Configuração

`appsettings.json` + `appsettings.Development.json`:
```json
{
  "Clyvo": { "ClinicaId": 1 }
}
```

Lido via `IOptions<ClyvoOptions>` ou `IConfiguration["Clyvo:ClinicaId"]` nos services.

Variável de ambiente `CLYVO__CLINICAID=1` para Docker.

### 7. Tratamento global de erros

Middleware ou `ExceptionHandlerMiddleware` que converte exceções de domínio em contrato `{ error: { code, message } }`. Códigos listados em `petbuddies-rest-contracts.md` §4.

### 8. Swagger

Garantir que todos os controllers aparecem no Swagger com:
- `[ProducesResponseType]` para todos os status codes documentados no contracts doc
- Resumos via `[SwaggerOperation(Summary = ...)]`

### 9. Seed manual no Oracle

Após migration, executar SQL manual:
- 1 `Clinica` com `Id=1`
- 1 `Endereco` para a clínica
- 1-2 `Veterinario` vinculados à clínica
- 3-5 `JanelaAtendimento` futuras (próximos 7 dias)
- 5-8 `TipoAnimal` cobrindo combinações comuns (CACHORRO+PEQUENO, CACHORRO+GRANDE, GATO+PEQUENO, etc.)

---

## O que está fora do escopo

- `Procedimento` e `RegistroAtendimento` (ficam para sprint 2)
- Endpoints de update completo de tutor/animal (só os mínimos do bot)
- Autenticação/autorização (decisão #12)
- Upload de fotos
- Webhooks de mudança de status

---

## Critérios de aceitação

- [ ] Migration aplicada, todas as tabelas `pb_tb_*` no Oracle
- [ ] Seed manual executado (clínica, veterinários, janelas, tipos de animal)
- [ ] Swagger lista todos os 7 endpoints novos com schemas corretos
- [ ] `GET /api/responsavel/buscar/{telefone}` retorna 404 para telefone inexistente
- [ ] `POST /api/responsavel` retorna 201 com `ResponsavelDto` e seta `Status=PRE_CADASTRO`
- [ ] `POST /api/animal` resolve `TipoAnimal` automaticamente, retorna 400 quando não acha combinação
- [ ] `GET /api/janela-atendimento` retorna apenas janelas futuras e disponíveis
- [ ] `POST /api/consulta` deriva `VeterinarioId` e `ClinicaId` da janela
- [ ] `PATCH /api/consulta/{id}` cancela e retorna 409 se já REALIZADA
- [ ] Erros seguem contrato `{ error: { code, message } }`
- [ ] Postman .NET atualizado (`PetBuddies-API.postman_collection.json`)

---

## Dependências

- `AnimalMotorDto` com `PreCadastro` ✅
- `ResponsavelEntity.EnderecoId` nullable ✅
- Connection string Oracle configurada (responsabilidade do Gabriel)
- `petbuddies-rest-contracts.md` é a fonte da verdade dos contratos

---

## Perguntas em aberto

- `JanelaAtendimento` "disponível" significa **sem nenhuma `Consulta` associada** ou inclui consultas `CANCELADA`? (Recomendação: ignorar canceladas — janela volta a estar disponível após cancelamento)
- Telefone do tutor: tamanho fixo (apenas dígitos) ou aceitar máscara? (Recomendação: apenas dígitos, normalizar no service antes de salvar/buscar)
