using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Minibank.Core.Domains.Enums;
using Minibank.Web.Controllers.BankAccount.Dto;
using Minibank.Web.Controllers.Transactions.Dto;
using Minibank.Core.Domains.BankAccounts.Services;

namespace MiniBank.Web.Controllers.BankAccount
{ 
    [ApiController]
    [Route("[controller]")]
    public class BankAccountsController
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly IMapper _mapper;

        public BankAccountsController(IBankAccountService bankAccountService, IMapper mapper)
        {
            _bankAccountService = bankAccountService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task CreateBankAccount(
            CreateBankAccountDto createBankAccountDto,
            CancellationToken cancellationToken = default)
        {
           await _bankAccountService.CreateAsync(
               createBankAccountDto.UserId, 
               (CurrencyCore)createBankAccountDto.Currency, 
               cancellationToken);
        }

        [HttpPut("close/{id}")]
        public async Task CloseBankAccount(int id, CancellationToken cancellationToken)
        {
           await _bankAccountService.CloseAsync(id, cancellationToken);
        }  
        
        [HttpGet("getTransferCommission/")]
        public async Task<decimal> CalculateTransferСommission(
            CreateTransactionDto createTransactionDto,
            CancellationToken cancellationToken = default)
        {
           return await _bankAccountService.CalculateTransferСommissionAsync(
                   createTransactionDto.Amount, 
                   createTransactionDto.FromAccountId, 
                   createTransactionDto.ToAccountId, 
                   cancellationToken);
        }  
        
        [HttpPost("executeTransfer/")]
        public async Task ExecuteTransfer(
            CreateTransactionDto createTransactionDto,
            CancellationToken cancellationToken = default)
        {
            await _bankAccountService.ExecuteTransferAsync(
                    createTransactionDto.Amount,
                    createTransactionDto.FromAccountId,
                    createTransactionDto.ToAccountId,
                    cancellationToken);
        }

        [HttpGet]
        public async Task<List<BankAccountDto>> GetAll(CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<BankAccountDto>>(await _bankAccountService.GetAllAsync(cancellationToken));
        }
    }
}