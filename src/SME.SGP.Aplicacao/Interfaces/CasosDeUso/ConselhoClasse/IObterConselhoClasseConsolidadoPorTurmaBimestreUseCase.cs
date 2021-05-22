﻿using SME.SGP.Infra;
using System.Collections.Generic;

namespace SME.SGP.Aplicacao
{
    public interface IObterConselhoClasseConsolidadoPorTurmaBimestreUseCase : IUseCase<FiltroConselhoClasseConsolidadoTurmaBimestreDto, IEnumerable<StatusTotalFechamentoDto>>
    {
    }
}