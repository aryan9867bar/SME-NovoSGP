﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioSecaoEncaminhamentoAEE : RepositorioBase<SecaoEncaminhamentoAEE>, IRepositorioSecaoEncaminhamentoAEE
    {
        public RepositorioSecaoEncaminhamentoAEE(ISgpContext database) : base(database)
        {
        }

        public async Task<IEnumerable<SecaoQuestionarioDto>> ObterSecaoEncaminhamentoPorEtapa(long etapa, long encaminhamentoAeeId = 0)
        {
            var query = @"SELECT sea.id
	                            , sea.nome
	                            , sea.questionario_id
	                            , eas.concluido
                         FROM secao_encaminhamento_aee sea
                        left join encaminhamento_aee_secao eas on eas.encaminhamento_aee_id = @encaminhamentoAeeId and eas.secao_encaminhamento_id = sea.id
                         WHERE not sea.excluido 
                           AND sea.etapa = @etapa
                         ORDER BY sea.ordem ";

            return await database.Conexao.QueryAsync<SecaoQuestionarioDto>(query, new { etapa, encaminhamentoAeeId });
        }
    }
}