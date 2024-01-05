using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;
public class AccountTransactionQueryHandler :IRequestHandler<GetAllAccountTransactionQuery, ApiResponse<List<AccountTransactionResponse>>>,
    IRequestHandler<GetAccountTransactionByIdQuery, ApiResponse<AccountTransactionResponse>>,
    IRequestHandler<GetAccountTransactionByParameterQuery, ApiResponse<List<AccountTransactionResponse>>>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public AccountTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetAllAccountTransactionQuery request, CancellationToken cancellationToken){
        var list = await _dbContext.Set<AccountTransaction>()
            .ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<AccountTransaction>, List<AccountTransactionResponse>>(list);
        return new ApiResponse<List<AccountTransactionResponse>>(mappedList);
    }
    public async Task<ApiResponse<AccountTransactionResponse>> Handle(GetAccountTransactionByIdQuery request,
        CancellationToken cancellationToken)
        {
            var entity =  await _dbContext.Set<AccountTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<AccountTransactionResponse>("Record not found");
            }
            
            var mapped = _mapper.Map<AccountTransaction, AccountTransactionResponse>(entity);
            return new ApiResponse<AccountTransactionResponse>(mapped);
        }

    public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetAccountTransactionByParameterQuery request,
        CancellationToken cancellationToken)
    {
        var list =  await _dbContext.Set<AccountTransaction>()
            .Where(x =>
            x.ReferenceNumber.ToUpper().Contains(request.ReferenceNumber.ToUpper()) ||
            x.Description.ToUpper().Contains(request.Description.ToUpper()) ||
              x.TransferType.ToUpper().Contains(request.TransferType.ToUpper()) 
        ).ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<AccountTransaction>, List<AccountTransactionResponse>>(list);
        return new ApiResponse<List<AccountTransactionResponse>>(mappedList);
    }
}
