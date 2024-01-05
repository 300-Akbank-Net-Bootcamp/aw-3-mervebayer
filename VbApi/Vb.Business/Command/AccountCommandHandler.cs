using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;
public class AccountCommandHandler : IRequestHandler<CreateAccountCommand, ApiResponse<AccountResponse>>, 
                                     IRequestHandler<UpdateAccountCommand, ApiResponse>,
                                     IRequestHandler<DeleteAccountCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;
    public AccountCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext =  dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var check = await _dbContext.Set<Account>().Where(x => x.AccountNumber == request.Model.AccountNumber).FirstOrDefaultAsync(cancellationToken);
        if (check != null)
        {
            return new ApiResponse<AccountResponse>($"{request.Model.AccountNumber} is used by another customer."); // check again Id??
        }

        var entity = _mapper.Map<AccountRequest, Account>(request.Model);

        var entityResult = await _dbContext.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<Account, AccountResponse>(entityResult.Entity);
        return new ApiResponse<AccountResponse>(mapped);
    }

    public async Task<ApiResponse> Handle (UpdateAccountCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<Account>().Where(x => x.AccountNumber == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.Balance = request.Model.Balance;
       check.Name = request.Model.Name;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }
    public async Task<ApiResponse> Handle (DeleteAccountCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<Account>().Where(x => x.AccountNumber == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.IsActive = false;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }

}