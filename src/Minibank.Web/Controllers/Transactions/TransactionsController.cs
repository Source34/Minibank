using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Minibank.Web.Controllers.Transactions.Dto;
using Minibank.Core.Domains.Transactions.Services;

namespace Minibank.Web.Controllers.Transactions
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionsController
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionsController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<TransactionDto>> GetAll(CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<TransactionDto>>(await _transactionService.GetAllAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<TransactionDto> GetUserById(int id, CancellationToken cancellationToken = default)
        {
            return _mapper.Map<TransactionDto>(await _transactionService.GetByIdAsync(id, cancellationToken));
        }
    }
}