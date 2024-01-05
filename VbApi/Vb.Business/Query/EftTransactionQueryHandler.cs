using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;
public class EftTransactionQueryHandler :IRequestHandler<GetAllEftTransactionQuery, ApiResponse<List<EftTransactionResponse>>>,
    IRequestHandler<GetEftTransactionByIdQuery, ApiResponse<EftTransactionResponse>>,
    IRequestHandler<GetEftTransactionByParameterQuery, ApiResponse<List<EftTransactionResponse>>>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public EftTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetAllEftTransactionQuery request, CancellationToken cancellationToken){
        var list = await _dbContext.Set<EftTransaction>().Where(x => x.IsActive == true)
            .ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
        return new ApiResponse<List<EftTransactionResponse>>(mappedList);
    }
    public async Task<ApiResponse<EftTransactionResponse>> Handle(GetEftTransactionByIdQuery request,
        CancellationToken cancellationToken)
        {
            var entity =  await _dbContext.Set<EftTransaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive == true, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<EftTransactionResponse>("Record not found");
            }
            
            var mapped = _mapper.Map<EftTransaction, EftTransactionResponse>(entity);
            return new ApiResponse<EftTransactionResponse>(mapped);
        }

    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetEftTransactionByParameterQuery request,
        CancellationToken cancellationToken)
    {
        var list =  await _dbContext.Set<EftTransaction>()
            .Where(x =>
            x.ReferenceNumber.ToUpper().Contains(request.ReferenceNumber.ToUpper()) ||
            x.Description.ToUpper().Contains(request.Description.ToUpper()) ||
              x.SenderAccount.ToUpper().Contains(request.SenderAccount.ToUpper()) ||
                x.SenderIban.ToUpper().Contains(request.SenderIban.ToUpper()) ||
                  x.SenderName.ToUpper().Contains(request.SenderName.ToUpper()) 
        ).ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
        return new ApiResponse<List<EftTransactionResponse>>(mappedList);
    }
}
