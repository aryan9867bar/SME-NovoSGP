﻿using MediatR;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Enumerados;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ObterParecerConclusivoAlunoQueryHandler : IRequestHandler<ObterParecerConclusivoAlunoQuery, ConselhoClasseParecerConclusivo>
    {
        protected IEnumerable<ConselhoClasseParecerConclusivo> pareceresDoServico;

        private readonly IRepositorioConceito repositorioConceito;
        private readonly IConsultasFrequencia consultasFrequencia;
        private readonly IMediator mediator;

        public ObterParecerConclusivoAlunoQueryHandler(IRepositorioConceito repositorioConceito,
                                                            IConsultasFrequencia consultasFrequencia,
                                                            IMediator mediator)
        {
            this.repositorioConceito = repositorioConceito ?? throw new System.ArgumentNullException(nameof(repositorioConceito));
            this.consultasFrequencia = consultasFrequencia ?? throw new System.ArgumentNullException(nameof(consultasFrequencia));
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public bool Filtrar(IEnumerable<ConselhoClasseParecerConclusivo> pareceresDoServico, string nomeClasseCalculo)
        {
            this.pareceresDoServico = pareceresDoServico;

            // Verifica se retornou 1 verdadeiro e 1 falso
            if (pareceresDoServico == null || !pareceresDoServico.Any())
                return false;

            if (!pareceresDoServico.Where(c => c.Aprovado).Any())
                throw new NegocioException($"Não localizado parecer conclusivo aprovado para o calculo por {nomeClasseCalculo}");
            if (pareceresDoServico.Where(c => c.Aprovado).Count() > 1)
                throw new NegocioException($"Encontrado mais de 1 parecer conclusivo aprovado para o calculo por {nomeClasseCalculo}");

            if (!pareceresDoServico.Where(c => !c.Aprovado).Any())
                throw new NegocioException($"Não localizado parecer conclusivo reprovado para o calculo por {nomeClasseCalculo}");
            if (pareceresDoServico.Where(c => !c.Aprovado).Count() > 1)
                throw new NegocioException($"Encontrado mais de 1 parecer conclusivo reprovado para o calculo por {nomeClasseCalculo}");

            return true;
        }

        private ConselhoClasseParecerConclusivo ObterParecerValidacao(bool retornoValidacao)
            => pareceresDoServico.FirstOrDefault(c => c.Aprovado == retornoValidacao);

        public async Task<ConselhoClasseParecerConclusivo> Handle(ObterParecerConclusivoAlunoQuery request, CancellationToken cancellationToken)
        {
            var turma = await mediator.Send(new ObterTurmaPorCodigoQuery(request.TurmaCodigo));

            string[] turmasCodigos;

            if (turma.DeveVerificarRegraRegulares())
            {
                List<TipoTurma> turmasCodigosParaConsulta = new List<TipoTurma>() { turma.TipoTurma };
                turmasCodigosParaConsulta.AddRange(turma.ObterTiposRegularesDiferentes());
                turmasCodigos = await mediator.Send(new ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery(turma.AnoLetivo, request.AlunoCodigo, turmasCodigosParaConsulta));
            }
            else
            {
                turmasCodigos = new string[1] { turma.CodigoTurma };
            }
            // Frequencia
            Filtrar(request.PareceresDaTurma.Where(c => c.Frequencia), "Frequência");
            if (!await ValidarParecerPorFrequencia(request.AlunoCodigo, turma, turmasCodigos))
                return ObterParecerValidacao(false);

            var parecerFrequencia = ObterParecerValidacao(true);

            // Nota
            if (!Filtrar(request.PareceresDaTurma.Where(c => c.Nota), "Nota"))
                return parecerFrequencia;

            var parecerNota = ObterParecerValidacao(await ValidarParecerPorNota(request.AlunoCodigo, turmasCodigos));

            // Conselho
            if (!Filtrar(request.PareceresDaTurma.Where(c => c.Conselho), "Conselho"))
                return parecerNota;

            var validacaoConselho = await ValidarParecerPorConselho(request.AlunoCodigo, turmasCodigos);
            if (!validacaoConselho.ExisteNotaConselho)
                return parecerNota;

            var parecerValidacao = ObterParecerValidacao(validacaoConselho.ValidacaoNotaConselho);

            if (parecerValidacao.Aprovado && parecerNota.Aprovado)
                return parecerNota;

            return parecerValidacao;
        }

        #region Frequência
        private async Task<bool> ValidarParecerPorFrequencia(string alunoCodigo, Turma turma, string[] turmasCodigos)
        {
            if (!await ValidarFrequenciaGeralAluno(alunoCodigo, turma.CodigoTurma))
                return false;

            var parametroFrequenciaBaseNacional = double.Parse(await mediator.Send(new ObterValorParametroSistemaTipoEAnoQuery(TipoParametroSistema.PercentualFrequenciaCriticoBaseNacional, DateTime.Today.Year)));

            Usuario usuarioAtual = await mediator.Send(new ObterUsuarioLogadoQuery());

            var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesPorTurmasCodigoQuery(turmasCodigos, usuarioAtual.PerfilAtual, usuarioAtual.Login, turma.EnsinoEspecial, turma.TurnoParaComponentesCurriculares));
            // Filtra componentes que lançam frequência
            var componentesCurriculareslancaFrequencia = componentesCurriculares.Where(c => c.RegistraFrequencia);
            var componentesCurricularesCodigos = componentesCurriculareslancaFrequencia.Select(c => c.CodigoComponenteCurricular.ToString()).ToArray();

            var frequencias = await mediator.Send(new ObterFrequenciasAlunosPorCodigoAlunoCodigoComponentesTurmaQuery(alunoCodigo, turmasCodigos, componentesCurricularesCodigos));
            if (frequencias.Any(f => f.PercentualFrequencia < parametroFrequenciaBaseNacional)) return false;

            return true;
        }

        private async Task<bool> ValidarFrequenciaGeralAluno(string alunoCodigo, string turmaCodigo)
        {
            var frequenciaAluno = await consultasFrequencia.ObterFrequenciaGeralAluno(alunoCodigo, turmaCodigo);
            double valorFrequenciaAluno = 0;
            if (frequenciaAluno != "")
                valorFrequenciaAluno = Convert.ToDouble(frequenciaAluno);


            var parametroFrequenciaGeral = double.Parse(await mediator.Send(new ObterValorParametroSistemaTipoEAnoQuery(TipoParametroSistema.PercentualFrequenciaCritico, DateTime.Today.Year)));
            return !(valorFrequenciaAluno < parametroFrequenciaGeral);
        }
        #endregion

        #region Nota
        private async Task<bool> ValidarParecerPorNota(string alunoCodigo, string[] turmasCodigos)
        {
            var notasFechamentoAluno = await mediator.Send(new ObterNotasFinaisPorAlunoTurmasQuery(alunoCodigo, turmasCodigos));
            if (notasFechamentoAluno == null || !notasFechamentoAluno.Any())
                return true;

            var tipoNota = notasFechamentoAluno.First().ConceitoId.HasValue ? TipoNota.Conceito : TipoNota.Nota;
            return tipoNota == TipoNota.Nota ?
                await ValidarParecerPorNota(notasFechamentoAluno) :
                await ValidarParecerPorConceito(notasFechamentoAluno);
        }

        private async Task<bool> ValidarParecerPorNota(IEnumerable<NotaConceitoBimestreComponenteDto> notasFechamentoAluno)
        {
            var notaMedia = double.Parse(await mediator.Send(new ObterValorParametroSistemaTipoEAnoQuery(TipoParametroSistema.MediaBimestre, DateTime.Today.Year)));
            foreach (var notaFechamentoAluno in notasFechamentoAluno)
                if (notaFechamentoAluno.Nota < notaMedia)
                    return false;

            return true;
        }

        private async Task<bool> ValidarParecerPorConceito(IEnumerable<NotaConceitoBimestreComponenteDto> conceitosFechamentoAluno)
        {
            var conceitosVigentes = await repositorioConceito.ObterPorData(DateTime.Today);
            foreach (var conceitoFechamentoAluno in conceitosFechamentoAluno)
            {
                var conceitoAluno = conceitosVigentes.FirstOrDefault(c => c.Id == conceitoFechamentoAluno.ConceitoId);
                if (conceitoAluno != null && !conceitoAluno.Aprovado)
                    return false;
            }

            return true;
        }
        #endregion

        #region Conselho
        private async Task<(bool ExisteNotaConselho, bool ValidacaoNotaConselho)> ValidarParecerPorConselho(string alunoCodigo, string[] turmasCodigos)
        {
            var notasConselhoClasse = await mediator.Send(new ObterNotasFinaisConselhoFechamentoPorAlunoTurmasQuery(turmasCodigos, alunoCodigo));
            if (notasConselhoClasse == null || !notasConselhoClasse.Any())
                return (false, false);

            var tipoNota = notasConselhoClasse.First().ConceitoId.HasValue ? TipoNota.Conceito : TipoNota.Nota;
            return (true, tipoNota == TipoNota.Nota ?
               await ValidarParecerConselhoPorNota(notasConselhoClasse) :
               await ValidarParecerConselhoPorConceito(notasConselhoClasse));
        }

        private async Task<bool> ValidarParecerConselhoPorNota(IEnumerable<NotaConceitoFechamentoConselhoFinalDto> notasConselhoClasse)
        {
            var notaMedia = double.Parse(await mediator.Send(new ObterValorParametroSistemaTipoEAnoQuery(TipoParametroSistema.MediaBimestre, DateTime.Today.Year)));
            foreach (var notaConcelhoClasse in notasConselhoClasse)
            {
                var nota = notaConcelhoClasse.Nota;
                var notaPosConselho = notasConselhoClasse.FirstOrDefault(n => n.ComponenteCurricularCodigo == notaConcelhoClasse.ComponenteCurricularCodigo && n.ConselhoClasseAlunoId > 0);
                if (notaPosConselho != null)
                {
                    nota = notaPosConselho.Nota;
                }
                if (nota < notaMedia)
                    return false;
            }

            return true;
        }

        private async Task<bool> ValidarParecerConselhoPorConceito(IEnumerable<NotaConceitoFechamentoConselhoFinalDto> notasConselhoClasse)
        {
            var conceitosVigentes = await repositorioConceito.ObterPorData(DateTime.Today);
            foreach (var conceitoConselhoClasseAluno in notasConselhoClasse)
            {
                var conceitoId = conceitoConselhoClasseAluno.ConceitoId;

                var conceitoPosConselho = notasConselhoClasse.FirstOrDefault(n => n.ComponenteCurricularCodigo == conceitoConselhoClasseAluno.ComponenteCurricularCodigo && n.ConselhoClasseAlunoId > 0);
                if (conceitoPosConselho != null)
                {
                    conceitoId = conceitoPosConselho.ConceitoId;
                }

                var conceitoAluno = conceitosVigentes.FirstOrDefault(c => c.Id == conceitoId);
                if (conceitoAluno != null && !conceitoAluno.Aprovado)
                    return false;
            }

            return true;
        }
        #endregion
    }
}