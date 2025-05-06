using AutoMapper;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Shared.DTOs.Consolidados.Responses;
using FluxoCaixa.Shared.DTOs.Lancamentos.Requests;
using FluxoCaixa.Shared.DTOs.Lancamentos.Responses;

namespace FluxoCaixa.Application.Mapping
{
    /// <summary>
    /// Perfil de mapeamento global para conversão entre entidades e DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamento de Lançamentos
            CreateMap<CriarLancamentoRequest, Lancamento>()
                .ForMember(dest => dest.DataLancamento, opt => opt.Ignore())
                .ForMember(dest => dest.ProcessadoConsolidacao, opt => opt.Ignore());

            CreateMap<Lancamento, LancamentoResponse>()
                .ForMember(dest => dest.TipoLancamento, opt => opt.MapFrom(src => src.TipoLancamento));

            CreateMap<TipoLancamento, TipoLancamentoResponse>();

            // Mapeamento de Consolidados
            CreateMap<SaldoConsolidado, ConsolidadoDiarioResponse>();
            CreateMap<RelatorioConsolidado, RelatorioConsolidadoResponse>();
        }
    }
}
