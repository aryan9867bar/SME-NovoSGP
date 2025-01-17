﻿using System;
using MediatR;
using SME.SGP.Dominio.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SME.SGP.Dominio;

namespace SME.SGP.Aplicacao.Queries
{
    public class ValidaSeExisteDrePorCodigoQueryHandler : IRequestHandler<ValidaSeExisteDrePorCodigoQuery, bool>
    {
        private readonly IRepositorioDreConsulta repositorioDre;

        public ValidaSeExisteDrePorCodigoQueryHandler(IRepositorioDreConsulta repositorioDre)
        {
            this.repositorioDre = repositorioDre ?? throw new ArgumentNullException(nameof(repositorioDre));
        }

        public async Task<bool> Handle(ValidaSeExisteDrePorCodigoQuery request, CancellationToken cancellationToken)
        {
            if (repositorioDre.ObterPorCodigo(request.CodigoDre).EhNulo())
                throw new NegocioException("Não foi possível encontrar a DRE");

            return true;
        }
    }
}
