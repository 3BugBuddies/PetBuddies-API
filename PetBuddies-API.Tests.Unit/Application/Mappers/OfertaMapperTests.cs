using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Tests.Unit.Application.Mappers
{
    public class OfertaMapperTests
    {
        [Fact]
        [Trait("Mapper", "Oferta")]
        public void Aplicar_AtoProtocolo_ZeraSubtipoEMantemProtocoloId()
        {
            // Arrange
            var oferta = new OfertaEntity();
            var request = new SalvarOfertaRequest
            {
                ClinicaId = 1,
                Ato = TipoAtoOfertaEnum.PROTOCOLO,
                Subtipo = "ignorado-pelo-mapper",
                ProtocoloId = 42,
                Descricao = "  Pacote preventivo  ",
                Valor = 199.9m,
                InicioVigencia = new DateOnly(2026, 1, 1)
            };

            // Act
            oferta.Aplicar(request);

            // Assert
            Assert.Null(oferta.Subtipo);
            Assert.Equal(42, oferta.ProtocoloId);
            Assert.Equal("Pacote preventivo", oferta.Descricao);
        }

        [Fact]
        [Trait("Mapper", "Oferta")]
        public void Aplicar_AtoConsulta_TrimaSubtipoEZeraProtocoloId()
        {
            // Arrange
            var oferta = new OfertaEntity();
            var request = new SalvarOfertaRequest
            {
                ClinicaId = 1,
                Ato = TipoAtoOfertaEnum.CONSULTA,
                Subtipo = "  CLINICA_GERAL  ",
                ProtocoloId = null,
                Descricao = "Consulta de rotina",
                Valor = 120m,
                InicioVigencia = new DateOnly(2026, 1, 1)
            };

            // Act
            oferta.Aplicar(request);

            // Assert
            Assert.Equal("CLINICA_GERAL", oferta.Subtipo);
            Assert.Null(oferta.ProtocoloId);
        }
    }
}
