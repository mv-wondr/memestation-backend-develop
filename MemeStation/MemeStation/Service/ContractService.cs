using System.Threading.Tasks;
using MemeStation.Contract.Function;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;

namespace MemeStation.Service
{
  public interface IContractService
  {
    public Task<TReturn> Get<TFunc, TReturn>(TFunc func) where TFunc:FunctionContract, new();
    public Task<TransactionReceipt> Post<TFunc>(TFunc func) where TFunc : FunctionContract, new();
  }

  public class ContractService : IContractService
  {
    private readonly ContractHandler _contractHandler;

    public ContractService(ContractHandler contractHandler)
    {
      _contractHandler = contractHandler;
    }

    public async Task<TReturn> Get<TFunc,TReturn>(TFunc func) where TFunc : FunctionContract, new()
    {
      return await _contractHandler.QueryAsync<TFunc, TReturn>(func);
    }

    public async Task<TransactionReceipt> Post<TFunc>(TFunc func) where TFunc : FunctionContract, new()
    {
      return await _contractHandler.SendRequestAndWaitForReceiptAsync(func);
    }
  }
}
