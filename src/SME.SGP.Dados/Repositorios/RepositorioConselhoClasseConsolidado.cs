﻿using Dapper;
using SME.SGP.Dados.Repositorios;
using SME.SGP.Dominio;
using SME.SGP.Infra;
using SME.SGP.Infra.Interface;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados
{
    public class RepositorioConselhoClasseConsolidado : RepositorioBase<ConselhoClasseConsolidadoTurmaAluno>, IRepositorioConselhoClasseConsolidado
    {
        public RepositorioConselhoClasseConsolidado(ISgpContext database, IServicoAuditoria servicoAuditoria) : base(database, servicoAuditoria)
        {
        }

        public async Task<bool> ExcluirLogicamenteConsolidacaoConselhoClasseAlunoTurmaPorIdConsolidacao(long[] idsConsolidacoes)
        {
            var sqlQuery = @"update consolidado_conselho_classe_aluno_turma
                             set excluido = true
                             where id = any(@idsConsolidacoes);";

            await database.Conexao
                .ExecuteAsync(sqlQuery, new { idsConsolidacoes });

            return true;
        }

        public Task<IEnumerable<ConsolidacaoConselhoClasseAlunoMigracaoDto>> ObterFechamentoNotaAlunoOuConselhoClasseAsync(long turmaId)
        {
           
            var query = @";with FechamentoConselhoBase as 
                            (
  	                        SELECT fa.aluno_codigo AlunoCodigo,
                                    ft.turma_id TurmaId,
                                    comp.id DisciplinaId,
                                    coalesce(ccn.nota,fn.nota) Nota,
                                    coalesce(ccn.conceito_id,fn.conceito_id) as ConceitoId,               
                                    pe.bimestre,
                                    cca.conselho_classe_parecer_id ParecerConclusivoId,
                                    coalesce(ccn.criado_em,fn.criado_em,cca.criado_em) criadoEm, 
                                    coalesce(ccn.criado_por,fn.criado_por,cca.criado_por) CriadoPor,  
                                    coalesce(ccn.criado_rf,fn.criado_rf,cca.criado_rf) CriadoRf,  
                                    cca.id ConselhoClasseAlunoId        
                                    FROM   fechamento_turma ft
                                    LEFT JOIN periodo_escolar pe
                                            ON pe.id = ft.periodo_escolar_id
                                    INNER JOIN turma t
                                            ON t.id = ft.turma_id
                                    INNER JOIN ue
                                            ON ue.id = t.ue_id
                                    INNER JOIN fechamento_turma_disciplina ftd
                                            ON ftd.fechamento_turma_id = ft.id
                                    INNER JOIN fechamento_aluno fa
                                            ON fa.fechamento_turma_disciplina_id = ftd.id
                                    INNER JOIN fechamento_nota fn
                                            ON fn.fechamento_aluno_id = fa.id
                                    INNER JOIN componente_curricular comp
                                            ON comp.id = fn.disciplina_id
                                    INNER JOIN conselho_classe cc
                                            ON cc.fechamento_turma_id = ft.id
                                    INNER JOIN conselho_classe_aluno cca
                                            ON cca.conselho_classe_id = cc.id
                                                AND cca.aluno_codigo = fa.aluno_codigo
                                    LEFT JOIN conselho_classe_nota ccn
                                            ON ccn.conselho_classe_aluno_id = cca.id
                                                AND ccn.componente_curricular_codigo = fn.disciplina_id
                                WHERE  ft.turma_id = @turmaId
                            ),FechamentoAlunoNotaBase as
                            (
  	                        select cca.aluno_codigo AlunoCodigo,
  	                                ft.turma_id TurmaId,
                                    comp.id DisciplinaId,
                                    ccn.nota Nota,
                                    ccn.conceito_id ConceitoId,
                                    pe.bimestre,
                                    cca.conselho_classe_parecer_id ParecerConclusivoId,
                                    ccn.criado_em criadoEmCCN,
		                            cca.criado_em criadoEmCCA,
                                    ccn.criado_por CriadoPorCCN,
                                    cca.criado_por CriadoPorCCA,
                                    ccn.criado_rf CriadoRfCCN,
                                    cca.criado_rf CriadoRfCCA,
                                    cca.id ConselhoClasseAlunoId,
                                    ft.id FechamentoTurmaId,
           
                                    ccn.componente_curricular_codigo ComponenteCurricularCodigo           
  	                        FROM   fechamento_turma ft
                                        LEFT JOIN periodo_escolar pe
                                                ON pe.id = ft.periodo_escolar_id
                                        INNER JOIN turma t
                                                ON t.id = ft.turma_id
                                        INNER JOIN ue
                                                ON ue.id = t.ue_id
                                        INNER JOIN conselho_classe cc
                                                ON cc.fechamento_turma_id = ft.id
                                        INNER JOIN conselho_classe_aluno cca
                                                ON cca.conselho_classe_id = cc.id
                                        INNER JOIN conselho_classe_nota ccn
                                                ON ccn.conselho_classe_aluno_id = cca.id
                                        INNER JOIN componente_curricular comp
                                                ON comp.id = ccn.componente_curricular_codigo
                            WHERE  ft.turma_id = @turmaId
                            ), fechamentoAlunoNota as 
                            (
  	                        select fb.AlunoCodigo,
  		                            fb.TurmaId, 
  		                            fb.DisciplinaId, 
  		                            coalesce(fb.nota,fn.nota) Nota,
  		                            coalesce(fb.ConceitoId,fn.conceito_id) as ConceitoId, 
  		                            fb.bimestre, 
  		                            fb.ParecerConclusivoId, 
  		                            coalesce(fb.criadoEmCCN,fn.criado_em,fb.criadoEmCCA) criadoEm,               
                                    coalesce(fb.CriadoPorCCN,fn.criado_por,fb.CriadoPorCCA) CriadoPor,  
                                    coalesce(fb.CriadoRfCCN,fn.criado_rf,fb.CriadoRfCCA) CriadoRf,      
  		                            fb.ConselhoClasseAlunoId
  	                        from FechamentoAlunoNotaBase fb 
	                        LEFT JOIN fechamento_turma_disciplina ftd ON ftd.fechamento_turma_id = fb.FechamentoTurmaId
                            LEFT JOIN fechamento_aluno fa ON fa.fechamento_turma_disciplina_id = ftd.id AND fb.AlunoCodigo = fa.aluno_codigo
                            LEFT JOIN fechamento_nota fn ON fn.fechamento_aluno_id = fa.id AND fb.ComponenteCurricularCodigo = fn.disciplina_id
                            WHERE  fb.TurmaId = @turmaId
                            ),
                            UnionQueries as 
                            (
	                            select * from fechamentoAlunoNota
	                            union all
	                            select * from FechamentoConselhoBase
	  
                            )
                            select DISTINCT * from UnionQueries ORDER BY AlunoCodigo asc ,bimestre asc ,criadoEm desc";

            return (database.Conexao.QueryAsync<ConsolidacaoConselhoClasseAlunoMigracaoDto>(query, new { turmaId }));
            
        }

        public async Task<long> ObterConselhoClasseConsolidadoPorTurmaAlunoAsync(long turmaId, string alunoCodigo)
        {
            var query = @"select id as ConsolidacaoId, 
                        turma_id as TurmaId,
                        aluno_codigo as AlunoCodigo
                        from consolidado_conselho_classe_aluno_turma 
                        where not excluido 
                        and turma_id = @turmaId 
                        and aluno_codigo = @alunoCodigo";

            return await database.Conexao.QueryFirstOrDefaultAsync<long>(query, new { turmaId, alunoCodigo });
        }
    }
}
