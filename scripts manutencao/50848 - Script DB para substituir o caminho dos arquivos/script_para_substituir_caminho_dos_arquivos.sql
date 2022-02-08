-- plano ciclo

drop table IF EXISTS tmp_plano_ciclo;

select tabela.id,
       regexp_replace(tabela.descricao, '/Arquivos/Editor/',
                      concat('/Arquivos/plano/ciclo/',
                             EXTRACT(YEAR FROM tabela.CRIADO_EM),
                             '/',
                             EXTRACT(MONTH FROM tabela.CRIADO_EM),
                             '/'
                          ), 'gi') as nova_descricao
into tmp_plano_ciclo
from plano_ciclo tabela
where descricao like any (array ['%<img%','%<video%']);

update plano_ciclo pc
set descricao = tpr.nova_descricao
from tmp_plano_ciclo tpr
where pc.id = tpr.id;

-- Frequência/Anotações do estudante

drop table IF EXISTS tmp_anotacao_frequencia_aluno_replace_img;

select tabela.id,
       regexp_replace(tabela.anotacao, '/Arquivos/Editor/', concat('/Arquivos/aluno/frequencia/',
                                                                   EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                   '/',
                                                                   EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                   '/'
           ), 'gi') as nova_descricao
into tmp_anotacao_frequencia_aluno_replace_img
from anotacao_frequencia_aluno tabela
where tabela.anotacao like any (array ['%<img%','%<video%']);

update anotacao_frequencia_aluno pc
set anotacao = tmp.nova_descricao
from tmp_anotacao_frequencia_aluno_replace_img tmp
where pc.id = tmp.id;


-- Plano anual

drop table IF EXISTS tmp_planejamento_anual_componente_replace_img;

select tabela.id,
       regexp_replace(descricao, '/Arquivos/Editor/', concat('/Arquivos/planejamento/anual/',
                                                             EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                             '/',
                                                             EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                             '/'
           ), 'gi') as nova_descricao
into tmp_planejamento_anual_componente_replace_img
from planejamento_anual_componente tabela
where descricao like any (array ['%<img%','%<video%']);

update planejamento_anual_componente pc
set descricao = tmp.nova_descricao
from tmp_planejamento_anual_componente_replace_img tmp
where pc.id = tmp.id;


-- Diario de bordo

drop table IF EXISTS tmp_diario_bordo_replace_img;

select tabela.id,
       regexp_replace(tabela.planejamento, '/Arquivos/Editor/', concat('/Arquivos/diario/bordo/',
                                                                       EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                       '/',
                                                                       EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                       '/'
           ), 'gi') as nova_descricao
into tmp_diario_bordo_replace_img
from diario_bordo tabela
where tabela.planejamento like any (array ['%<img%','%<video%'])
  and tabela.criado_em > '2021-01-01';

update diario_bordo tabela
set planejamento = tmp.nova_descricao
from tmp_diario_bordo_replace_img tmp
where tabela.id = tmp.id;


-- Devolutivas

drop table IF EXISTS tmp_devolutiva_replace_img;

select tabela.id,
       regexp_replace(tabela.descricao, '/Arquivos/Editor/', concat('/Arquivos/devolutiva/',
                                                                    EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                    '/',
                                                                    EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                    '/'
           ), 'gi') as nova_descricao
into tmp_devolutiva_replace_img
from devolutiva tabela
where descricao like any (array ['%<img%','%<video%']);

update devolutiva pc
set descricao = tmp.nova_descricao
from tmp_devolutiva_replace_img tmp
where pc.id = tmp.id;

-- Compensacao ausencia

drop table IF EXISTS tmp_compensacao_ausencia_replace_img;

select tabela.id,
       regexp_replace(descricao, '/Arquivos/Editor/', concat('/Arquivos/compensacao/ausencia/',
                                                             EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                             '/',
                                                             EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                             '/'
           ), 'gi') as nova_descricao
into tmp_compensacao_ausencia_replace_img
from compensacao_ausencia tabela
where descricao like any (array ['%<img%','%<video%']);

update compensacao_ausencia pc
set descricao = tmp.nova_descricao
from tmp_compensacao_ausencia_replace_img tmp
where pc.id = tmp.id;


-- Registro individual

drop table IF EXISTS tmp_registro_individual_replace_img;

select tabela.id,
       regexp_replace(registro, '/Arquivos/Editor/', concat('/Arquivos/registro/individual/',
                                                            EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                            '/',
                                                            EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                            '/'
           ), 'gi') as nova_descricao
into tmp_registro_individual_replace_img
from registro_individual tabela
where registro like any (array ['%<img%','%<video%']);

update registro_individual pc
set registro = tmp.nova_descricao
from tmp_registro_individual_replace_img tmp
where pc.id = tmp.id;


-- Territorio do saber

-- atualizar path desenvolvimento

drop table IF EXISTS tmp_plano_anual_territorio_saber_desenvolvimento_replace_img;

select tabela.id,
       regexp_replace(desenvolvimento, '/Arquivos/Editor/', concat('/Arquivos/planejamento/anual/territorio_saber/',
                                                                   EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                   '/',
                                                                   EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                   '/'
           ), 'gi') as nova_descricao
into tmp_plano_anual_territorio_saber_desenvolvimento_replace_img
from plano_anual_territorio_saber tabela
where desenvolvimento like any (array ['%<img%','%<video%']);

update plano_anual_territorio_saber pc
set desenvolvimento = tmp.nova_descricao
from tmp_plano_anual_territorio_saber_desenvolvimento_replace_img tmp
where pc.id = tmp.id;

-- atualizar path reflexao

drop table IF EXISTS tmp_plano_anual_territorio_saber_reflexao_replace_img;

select tabela.id,
       regexp_replace(reflexao, '/Arquivos/Editor/', concat('/Arquivos/planejamento/anual/territorio_saber/',
                                                            EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                            '/',
                                                            EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                            '/'
           ), 'gi') as nova_descricao
into tmp_plano_anual_territorio_saber_reflexao_replace_img
from plano_anual_territorio_saber tabela
where reflexao like any (array ['%<img%','%<video%']);

update plano_anual_territorio_saber pc
set reflexao = tmp.nova_descricao
from tmp_plano_anual_territorio_saber_reflexao_replace_img tmp
where pc.id = tmp.id;

-- Carta de intenções

drop table IF EXISTS tmp_carta_intencoes_replace_img;

select tabela.id,
       regexp_replace(planejamento, '/Arquivos/Editor/', concat('/Arquivos/carta/intencoes/',
                                                                EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                '/',
                                                                EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                '/'
           ), 'gi') as nova_descricao
into tmp_carta_intencoes_replace_img
from carta_intencoes tabela
where planejamento like any (array ['%<img%','%<video%']);

update carta_intencoes pc
set planejamento = tmp.nova_descricao
from tmp_carta_intencoes_replace_img tmp
where pc.id = tmp.id;


-- Registro POA

drop table IF EXISTS tmp_registro_poa_replace_img;

select tabela.id,
       regexp_replace(descricao, '/Arquivos/Editor/', concat('/Arquivos/regsitro/poa/',
                                                             EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                             '/',
                                                             EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                             '/'
           ), 'gi') as nova_descricao
into tmp_registro_poa_replace_img
from registro_poa tabela
where descricao like any (array ['%<img%','%<video%']);

update registro_poa pc
set descricao = tmp.nova_descricao
from tmp_registro_poa_replace_img tmp
where pc.id = tmp.id;


-- Conselho de classe

-- atualizar path recomendacoes aluno

drop table IF EXISTS tmp_conselho_classe_aluno_recomendacoes_aluno_replace_img;

select tabela.id,
       regexp_replace(recomendacoes_aluno, '/Arquivos/Editor/', concat('/Arquivos/conselho_classe/',
                                                                       EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                       '/',
                                                                       EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                       '/'
           ), 'gi') as nova_descricao
into tmp_conselho_classe_aluno_recomendacoes_aluno_replace_img
from conselho_classe_aluno tabela
where recomendacoes_aluno like any (array ['%<img%','%<video%']);

update conselho_classe_aluno pc
set recomendacoes_aluno = tmp.nova_descricao
from tmp_conselho_classe_aluno_recomendacoes_aluno_replace_img tmp
where pc.id = tmp.id;

-- atualizar path recomendacoes familia

drop table IF EXISTS tmp_conselho_classe_aluno_recomendacoes_familia_replace_img;

select tabela.id,
       regexp_replace(recomendacoes_familia, '/Arquivos/Editor/', concat('/Arquivos/conselho_classe/',
                                                                         EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                         '/',
                                                                         EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                         '/'
           ), 'gi') as nova_descricao
into tmp_conselho_classe_aluno_recomendacoes_familia_replace_img
from conselho_classe_aluno tabela
where recomendacoes_familia like any (array ['%<img%','%<video%']);


update conselho_classe_aluno pc
set recomendacoes_familia = tmp.nova_descricao
from tmp_conselho_classe_aluno_recomendacoes_familia_replace_img tmp
where pc.id = tmp.id;


-- atualizar path anotacoes pedagogicas

drop table IF EXISTS tmp_conselho_classe_aluno_anotacoes_pedagogicas_replace_img;

select tabela.id,
       regexp_replace(anotacoes_pedagogicas, '/Arquivos/Editor/', concat('/Arquivos/conselho_classe/',
                                                                         EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                         '/',
                                                                         EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                         '/'
           ), 'gi') as nova_descricao
into tmp_conselho_classe_aluno_anotacoes_pedagogicas_replace_img
from conselho_classe_aluno tabela
where anotacoes_pedagogicas like any (array ['%<img%','%<video%']);


update conselho_classe_aluno pc
set anotacoes_pedagogicas = tmp.nova_descricao
from tmp_conselho_classe_aluno_anotacoes_pedagogicas_replace_img tmp
where pc.id = tmp.id;


-- Ocorrencia

drop table IF EXISTS tmp_ocorrencia_replace_img;

select tabela.id,
       regexp_replace(descricao, '/Arquivos/Editor/', concat('/Arquivos/aluno/ocorrencia/',
                                                             EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                             '/',
                                                             EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                             '/'
           ), 'gi') as nova_descricao
into tmp_ocorrencia_replace_img
from ocorrencia tabela
where descricao like any (array ['%<img%','%<video%']);

update ocorrencia pc
set descricao = tmp.nova_descricao
from tmp_ocorrencia_replace_img tmp
where pc.id = tmp.id;


-- relatorio semestral pap

drop table IF EXISTS tmp_relatorio_semestral_pap_aluno_secao_replace_img;

select tabela.id,
       regexp_replace(tabela.valor, '/Arquivos/Editor/', concat('/Arquivos/relatorio/semestral_pap/',
                                                                EXTRACT(YEAR FROM a.CRIADO_EM),
                                                                '/',
                                                                EXTRACT(MONTH FROM a.CRIADO_EM),
                                                                '/'
           ), 'gi')                                as nova_descricao,
       uuid(cast(regexp_matches(tabela.valor,
                                '[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}',
                                'gi') as varchar)) as codigo
into tmp_relatorio_semestral_pap_aluno_secao_replace_img
from relatorio_semestral_pap_aluno_secao tabela
         inner join arquivo a on a.codigo = codigo
where tabela.valor like any (array ['%<img%','%<video%']);

update relatorio_semestral_pap_aluno_secao pc
set valor = tmp.nova_descricao
from tmp_relatorio_semestral_pap_aluno_secao_replace_img tmp
where pc.id = tmp.id;



-- Plano aula

--atualizar path descricao


drop table IF EXISTS tmp_plano_aula_descricao_replace_img;

select tabela.id,
       regexp_replace(descricao, '/Arquivos/Editor/', concat('/Arquivos/planejamento/aula/descricao/',
                                                             EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                             '/',
                                                             EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                             '/'
           ), 'gi') as nova_descricao
into tmp_plano_aula_descricao_replace_img
from plano_aula tabela
where descricao like any (array ['%<img%','%<video%']);

update plano_aula pc
set descricao = tmp.nova_descricao
from tmp_plano_aula_descricao_replace_img tmp
where pc.id = tmp.id;

--atualizar path desenvolvimento aula

drop table IF EXISTS tmp_plano_aula_desenvolvimento_aula_replace_img;

select tabela.id,
       regexp_replace(desenvolvimento_aula, '/Arquivos/Editor/', concat('/Arquivos/planejamento/aula/desenvolvimento/',
                                                                        EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                        '/',
                                                                        EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                        '/'
           ), 'gi') as nova_descricao
into tmp_plano_aula_desenvolvimento_aula_replace_img
from plano_aula tabela
where desenvolvimento_aula like any (array ['%<img%','%<video%']);

update plano_aula pc
set desenvolvimento_aula = tmp.nova_descricao
from tmp_plano_aula_desenvolvimento_aula_replace_img tmp
where pc.id = tmp.id;


--atualizar path recuperacao aula

drop table IF EXISTS tmp_plano_aula_recuperacao_aula_replace_img;

select tabela.id,
       regexp_replace(recuperacao_aula, '/Arquivos/Editor/', concat('/Arquivos/planejamento/aula/recuperacao/',
                                                                    EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                    '/',
                                                                    EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                    '/'
           ), 'gi') as nova_descricao
into tmp_plano_aula_recuperacao_aula_replace_img
from plano_aula tabela
where recuperacao_aula like any (array ['%<img%','%<video%']);

update plano_aula pc
set recuperacao_aula = tmp.nova_descricao
from tmp_plano_aula_recuperacao_aula_replace_img tmp
where pc.id = tmp.id;


--atualizar path licao casa

drop table IF EXISTS tmp_plano_aula_licao_casa_replace_img;

select tabela.id,
       regexp_replace(licao_casa, '/Arquivos/Editor/', concat('/Arquivos/planejamento/aula/licao_casa/',
                                                              EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                              '/',
                                                              EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                              '/'
           ), 'gi') as nova_descricao
into tmp_plano_aula_licao_casa_replace_img
from plano_aula tabela
where licao_casa like any (array ['%<img%','%<video%']);

update plano_aula pc
set licao_casa = tmp.nova_descricao
from tmp_plano_aula_licao_casa_replace_img tmp
where pc.id = tmp.id;


-- Fechamento/Anotações


drop table IF EXISTS tmp_anotacao_aluno_fechamento_replace_img;

select tabela.id,
       regexp_replace(anotacao, '/Arquivos/Editor/', concat('/Arquivos/fechamento/aluno/anotacao/',
                                                            EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                            '/',
                                                            EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                            '/'
           ), 'gi') as nova_descricao
into tmp_anotacao_aluno_fechamento_replace_img
from anotacao_aluno_fechamento tabela
where anotacao like any (array ['%<img%','%<video%']);

update anotacao_aluno_fechamento pc
set anotacao = tmp.nova_descricao
from tmp_anotacao_aluno_fechamento_replace_img tmp
where pc.id = tmp.id;

-- Relatório de acompanhamento da aprendizagem

--atualizar path PERCURSO COLETIVO

drop table IF EXISTS TMP_ACOMPANHAMENTO_TURMA_REPLACE_IMG;

select tabela.id,
       regexp_replace(tabela.apanhado_geral, '/Arquivos/Editor/', concat('/Arquivos/aluno/acompanhamento/',
                                                                              EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                              '/',
                                                                              EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                              '/'
           ), 'gi') as nova_descricao
into TMP_ACOMPANHAMENTO_TURMA_REPLACE_IMG
from ACOMPANHAMENTO_TURMA tabela
where apanhado_geral like any (array ['%<img%','%<video%']);

update ACOMPANHAMENTO_TURMA pc
set apanhado_geral = tmp.nova_descricao
from TMP_ACOMPANHAMENTO_TURMA_REPLACE_IMG tmp
where pc.id = tmp.id;


--atualizar path PERCURSO INDIVIDUAL

drop table IF EXISTS TMP_ACOMPANHAMENTO_ALUNO_SEMESTRE_PERCURSO_INDIVIDUAL_REPLACE_IMG;

select tabela.id,
       regexp_replace(tabela.PERCURSO_INDIVIDUAL, '/Arquivos/Editor/', concat('/Arquivos/aluno/acompanhamento/',
                                                                              EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                              '/',
                                                                              EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                              '/'
           ), 'gi') as nova_descricao
into TMP_ACOMPANHAMENTO_ALUNO_SEMESTRE_PERCURSO_INDIVIDUAL_REPLACE_IMG
from ACOMPANHAMENTO_ALUNO_SEMESTRE tabela
where PERCURSO_INDIVIDUAL like any (array ['%<img%','%<video%']);

update ACOMPANHAMENTO_ALUNO_SEMESTRE pc
set PERCURSO_INDIVIDUAL = tmp.nova_descricao
from TMP_ACOMPANHAMENTO_ALUNO_SEMESTRE_PERCURSO_INDIVIDUAL_REPLACE_IMG tmp
where pc.id = tmp.id;

--atualizar path observaçoes 

drop table IF EXISTS TMP_ACOMPANHAMENTO_ALUNO_SEMESTRE_OBSERVACOES_REPLACE_IMG;

select tabela.id,
       regexp_replace(tabela.observacoes, '/Arquivos/Editor/', concat('/Arquivos/aluno/acompanhamento/',
                                                                      EXTRACT(YEAR FROM tabela.CRIADO_EM),
                                                                      '/',
                                                                      EXTRACT(MONTH FROM tabela.CRIADO_EM),
                                                                      '/'
           ), 'gi') as nova_descricao
into TMP_ACOMPANHAMENTO_ALUNO_SEMESTRE_OBSERVACOES_REPLACE_IMG
from ACOMPANHAMENTO_ALUNO_SEMESTRE tabela
where tabela.observacoes like any (array ['%<img%','%<video%']);

update ACOMPANHAMENTO_ALUNO_SEMESTRE pc
set observacoes = tmp.nova_descricao
from TMP_ACOMPANHAMENTO_ALUNO_SEMESTRE_OBSERVACOES_REPLACE_IMG tmp
where pc.id = tmp.id;