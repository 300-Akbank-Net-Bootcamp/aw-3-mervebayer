using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;
public class AccountTransactionCommandHandler : IRequestHandler<CreateAccountTransactionCommand, ApiResponse<AccountTransactionResponse>>, 
                                     IRequestHandler<UpdateAccountTransactionCommand, ApiResponse>,
                                     IRequestHandler<DeleteAccountTransactionCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;
    public AccountTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext =  dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AccountTransactionResponse>> Handle(CreateAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var check = await _dbContext.Set<AccountTransaction>().Where(x => x.Id == request.Model.Id).FirstOrDefaultAsync(cancellationToken);
        if (check != null)
        {
            return new ApiResponse<AccountTransactionResponse>($"{request.Model.Id} is used by another customer."); // check again Id??
        }

        var entity = _mapper.Map<AccountTransactionRequest, AccountTransaction>(request.Model);

        var entityResult = await _dbContext.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<AccountTransaction, AccountTransactionResponse>(entityResult.Entity);
        return new ApiResponse<AccountTransactionResponse>(mapped);
    }

    public async Task<ApiResponse> Handle (UpdateAccountTransactionCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<AccountTransaction>().Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.ReferenceNumber = request.Model.ReferenceNumber;
       check.Description = request.Model.Description;
       check.TransferType = request.Model.TransferType;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }
    public async Task<ApiResponse> Handle (DeleteAccountTransactionCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<AccountTransaction>().Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.IsActive = false;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }

}