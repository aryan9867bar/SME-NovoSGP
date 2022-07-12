﻿using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.SGP.Aplicacao;
using SME.SGP.Dominio;
using SME.SGP.Infra;
using SME.SGP.TesteIntegracao.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SME.SGP.TesteIntegracao.Frequencia
{
    public class Ao_apresentar_os_alunos_na_tela : FrequenciaTesteBase
    {
        public Ao_apresentar_os_alunos_na_tela(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected readonly DateTime DATA_02_09 = new(DateTimeExtension.HorarioBrasilia().Year, 09, 02);
        private const string SITUACAO_15 = "15";

        [Fact]
        public async Task Alunos_novos_devem_aparecer_com_tooltip_durante_15_dias()
        {
            await CriarDadosBasicos(ObterPerfilProfessor(), Modalidade.Fundamental, ModalidadeTipoCalendario.FundamentalMedio, DATA_02_05, DATA_07_08, BIMESTRE_2, DATA_02_05, COMPONENTE_CURRICULAR_PORTUGUES_ID_138.ToString(), false, NUMERO_AULAS_1);
            await InserirParametroSistema();

            var useCase = ServiceProvider.GetService<IObterFrequenciaPorAulaUseCase>();

            var filtroFrequencia = new FiltroFrequenciaDto()
            {
                AulaId = AULA_ID_1,
                ComponenteCurricularId = COMPONENTE_CURRICULAR_PORTUGUES_ID_138
            };

            var retorno = await useCase.Executar(filtroFrequencia);
            var periodosEscolares = ObterTodos<PeriodoEscolar>();
            var periodoEscolar = periodosEscolares.FirstOrDefault(x => x.Bimestre == BIMESTRE_2);

            var retornoAluno = retorno.ListaFrequencia.FirstOrDefault(x => x.SituacaoMatricula == ((int)SituacaoMatriculaAluno.Ativo).ToString());
            retornoAluno.DataSituacao.ShouldBeGreaterThanOrEqualTo(periodoEscolar.PeriodoInicio);
            retornoAluno.DataSituacao.ShouldBeLessThanOrEqualTo(periodoEscolar.PeriodoFim);

            retornoAluno.Marcador.ShouldNotBeNull();
        }

        [Fact]
        public async Task Alunos_novos_nao_devem_aparecer_com_tooltip_apos_15_dias()
        {
            await CriarDadosBasicos(ObterPerfilProfessor(), Modalidade.Fundamental, ModalidadeTipoCalendario.FundamentalMedio, DATA_02_05, DATA_07_08, BIMESTRE_2, DATA_02_05, COMPONENTE_CURRICULAR_PORTUGUES_ID_138.ToString(), false, NUMERO_AULAS_1);
            await InserirParametroSistema();

            var useCase = ServiceProvider.GetService<IObterFrequenciaPorAulaUseCase>();

            var filtroFrequencia = new FiltroFrequenciaDto()
            {
                AulaId = AULA_ID_1,
                ComponenteCurricularId = COMPONENTE_CURRICULAR_PORTUGUES_ID_138
            };

            var periodosEscolares = ObterTodos<PeriodoEscolar>();
            var periodoEscolar = periodosEscolares.FirstOrDefault(x => x.Bimestre == BIMESTRE_2);

            var retorno = await useCase.Executar(filtroFrequencia);

            var retornoAluno = retorno.ListaFrequencia.FirstOrDefault(x => x.CodigoAluno == CODIGO_ALUNO_4);

            retornoAluno.DataSituacao.ShouldBeGreaterThanOrEqualTo(periodoEscolar.PeriodoInicio);
            retornoAluno.DataSituacao.ShouldBeLessThanOrEqualTo(periodoEscolar.PeriodoFim);

            var marcador = retorno.ListaFrequencia.FirstOrDefault(x => x.CodigoAluno == CODIGO_ALUNO_4).Marcador;

            retornoAluno.Marcador.ShouldBeNull();
        }

        [Fact]
        public async Task Alunos_inativos_devem_aparecer_com_tooltip_ate_data_de_inativacao()
        {
            await CriarDadosBasicos(ObterPerfilProfessor(), Modalidade.Fundamental, ModalidadeTipoCalendario.FundamentalMedio, DATA_02_05, DATA_07_08, BIMESTRE_2, DATA_02_09, COMPONENTE_CURRICULAR_PORTUGUES_ID_138.ToString(), false, NUMERO_AULAS_1);
            await InserirParametroSistema();

            var useCase = ServiceProvider.GetService<IObterFrequenciaPorAulaUseCase>();

            var filtroFrequencia = new FiltroFrequenciaDto()
            {
                AulaId = AULA_ID_1,
                ComponenteCurricularId = COMPONENTE_CURRICULAR_PORTUGUES_ID_138
            };

            var retorno = await useCase.Executar(filtroFrequencia);

            var retornoAluno = retorno.ListaFrequencia.FirstOrDefault(x => x.CodigoAluno == CODIGO_ALUNO_2);

            retornoAluno.SituacaoMatricula.ShouldBe(SITUACAO_15);
            retornoAluno.Marcador.ShouldNotBeNull();
        }

        [Fact]
        public async Task Alunos_inativos_antes_do_inicio_do_ano_letivo_nao_devem_aparecer_na_tela()
        {
            await CriarDadosBasicos(ObterPerfilProfessor(), Modalidade.Fundamental, ModalidadeTipoCalendario.FundamentalMedio, DATA_02_05, DATA_07_08, BIMESTRE_2, DATA_02_09, COMPONENTE_CURRICULAR_PORTUGUES_ID_138.ToString(), false, NUMERO_AULAS_1);
            await InserirParametroSistema();

            var useCase = ServiceProvider.GetService<IObterFrequenciaPorAulaUseCase>();

            var filtroFrequencia = new FiltroFrequenciaDto()
            {
                AulaId = AULA_ID_1,
                ComponenteCurricularId = COMPONENTE_CURRICULAR_PORTUGUES_ID_138
            };

            var periodosEscolares = ObterTodos<PeriodoEscolar>();
            var periodoEscolar = periodosEscolares.FirstOrDefault(x => x.Bimestre == BIMESTRE_1);

            var retorno = await useCase.Executar(filtroFrequencia);

            var retornoAluno = retorno.ListaFrequencia.FirstOrDefault(x => x.CodigoAluno == CODIGO_ALUNO_3);

            retornoAluno.DataSituacao.ShouldBeLessThan(periodoEscolar.PeriodoInicio);
            retornoAluno.Desabilitado.ShouldBeTrue();
        }
    }
}