﻿drop table if exists TMP_ACOMPANHAMENTO_ALUNO;

SELECT
    AAF.ACOMPANHAMENTO_ALUNO_SEMESTRE_ID,
    STRING_AGG(
            CONCAT('<br/><IMG SRC=',
                   '"https://hom2-novosgp.sme.prefeitura.sp.gov.br/Arquivos/Editor/PercursoIndividual/',
                   EXTRACT(YEAR FROM AAF.CRIADO_EM),
                   '/',
                   EXTRACT(MONTH FROM AAF.CRIADO_EM),
                   '/',
                   CONCAT(CODIGO, REVERSE(SUBSTRING(REVERSE(NOME) FROM 1 FOR STRPOS(REVERSE(NOME),'.')-0))),
                   '">'), ' ')
        AS IMAGENS
 INTO TEMP TMP_ACOMPANHAMENTO_ALUNO
FROM ACOMPANHAMENTO_ALUNO AA
         INNER JOIN ACOMPANHAMENTO_ALUNO_SEMESTRE AAS ON AAS.ACOMPANHAMENTO_ALUNO_ID = AA.ID
         INNER JOIN ACOMPANHAMENTO_ALUNO_FOTO AAF ON AAF.ACOMPANHAMENTO_ALUNO_SEMESTRE_ID = AAS.ID
         INNER JOIN ARQUIVO A ON A.ID = AAF.ARQUIVO_ID
WHERE MINIATURA_ID IS NOT null
GROUP BY AAF.ACOMPANHAMENTO_ALUNO_SEMESTRE_ID;

UPDATE ACOMPANHAMENTO_ALUNO_SEMESTRE AAS SET PERCURSO_INDIVIDUAL = CONCAT(AAS.PERCURSO_INDIVIDUAL, TAA.IMAGENS) FROM TMP_ACOMPANHAMENTO_ALUNO TAA  WHERE TAA.ACOMPANHAMENTO_ALUNO_SEMESTRE_ID = AAS.ID 