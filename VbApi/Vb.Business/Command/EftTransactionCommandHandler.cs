using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;
public class EftTransactionCommandHandler : IRequestHandler<CreateEftTransactionCommand, ApiResponse<EftTransactionResponse>>, 
                                     IRequestHandler<UpdateEftTransactionCommand, ApiResponse>,
                                     IRequestHandler<DeleteEftTransactionCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;
    public EftTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext =  dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<EftTransactionResponse>> Handle(CreateEftTransactionCommand request, CancellationToken cancellationToken)
    {
        var check = await _dbContext.Set<EftTransaction>().Where(x => x.Id == request.Model.Id).FirstOrDefaultAsync(cancellationToken);
        if (check != null)
        {
            return new ApiResponse<EftTransactionResponse>($"{request.Model.Id} is used by another customer."); // check again Id??
        }

        var entity = _mapper.Map<EftTransactionRequest, EftTransaction>(request.Model);

        var entityResult = await _dbContext.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<EftTransaction, EftTransactionResponse>(entityResult.Entity);
        return new ApiResponse<EftTransactionResponse>(mapped);
    }

    public async Task<ApiResponse> Handle (UpdateEftTransactionCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<EftTransaction>().Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.ReferenceNumber = request.Model.ReferenceNumber;
       check.Amount = request.Model.Amount;
       check.Description = request.Model.Description;
       check.SenderIban = request.Model.SenderIban;
       check.SenderAccount = request.Model.SenderAccount;
       check.SenderName = request.Model.SenderName;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }
    public async Task<ApiResponse> Handle (DeleteEftTransactionCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<EftTransaction>().Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.IsActive = false;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }

}