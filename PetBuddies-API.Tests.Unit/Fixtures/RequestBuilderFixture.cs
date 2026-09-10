using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Application.Dtos.RegraPontuacao;
using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Tests.Unit.Fixtures
{
    // Compartilhada via Collection Fixture pelos quatro testes de serviço: cada um parte
    // de uma requisição válida e usa `with` para chegar no cenário que quer provar.
    public class RequestBuilderFixture
    {
        public SalvarProtocoloRequest ProtocoloValido() => new()
        {
            Nome = "Protocolo de vacinação anual",
            Categoria = CategoriaProtocoloEnum.PREVENTIVO,
            Especie = EspecieEnum.CACHORRO,
            Ativo = true,
            Descricao = "Protocolo padrão da clínica"
        };

        public SalvarRegraProtocoloRequest RegraProtocoloValida(long protocoloId = 1) => new()
        {
            ProtocoloId = protocoloId,
            Tipo = TipoCuidadoEnum.VACINACAO,
            Nome = "Primeira dose",
            Offset = 0,
            UnidadeOffset = UnidadeTempoEnum.DIAS,
            DataBase = TipoDataBaseEnum.NASCIMENTO,
            Intervalo = null,
            UnidadeIntervalo = null,
            Repeticoes = 1,
            Descricao = null
        };

        public SalvarOfertaRequest OfertaDeProtocoloValida(long protocoloId = 1) => new()
        {
            ClinicaId = 1,
            Ato = TipoAtoOfertaEnum.PROTOCOLO,
            Subtipo = null,
            ProtocoloId = protocoloId,
            Descricao = "Pacote preventivo",
            Valor = 199.90m,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };

        public SalvarOfertaRequest OfertaDeConsultaValida() => new()
        {
            ClinicaId = 1,
            Ato = TipoAtoOfertaEnum.CONSULTA,
            Subtipo = "CLINICA_GERAL",
            ProtocoloId = null,
            Descricao = "Consulta de rotina",
            Valor = 120m,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };

        public SalvarRegraPontuacaoRequest RegraPontuacaoValida() => new()
        {
            ClinicaId = 1,
            Gesto = TipoGestoEnum.CONSULTA_REALIZADA,
            Pontos = 10,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };
    }

    [CollectionDefinition(NomeDaColecao)]
    public class ServicosDoBackOfficeCollection : ICollectionFixture<RequestBuilderFixture>
    {
        public const string NomeDaColecao = "Serviços do back-office";
    }
}
