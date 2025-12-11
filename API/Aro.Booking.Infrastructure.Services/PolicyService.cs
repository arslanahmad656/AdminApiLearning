using Aro.Booking.Application.Repository;
using Aro.Booking.Application.Services.Policy;
using Aro.Booking.Domain.Entities;
using Aro.Booking.Domain.Shared.Exceptions;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Authorization;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Booking.Infrastructure.Services;

public partial class PolicyService(
    Application.Repository.IRepositoryManager bookingRepository,
    IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService,
    IUniqueIdGenerator idGenerator,
    ILogManager<PolicyService> logger
) : IPolicyService
{
    private readonly IPolicyRepository policyRepository = bookingRepository.PolicyRepository;

    public async Task<CreatePolicyResponse> CreatePolicy(CreatePolicyDto policy, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreatePolicy], cancellationToken);

        logger.LogDebug("Starting {MethodName} for policy title: {PolicyTitle}", nameof(CreatePolicy), policy.Title);

        var policyEntity = new Policy
        {
            Id = idGenerator.Generate(),
            Title = policy.Title,
            Description = policy.Description,
            IsActive = policy.IsActive
        };

        await policyRepository.Create(policyEntity, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Policy created successfully with Id: {PolicyId}", policyEntity.Id);

        return new CreatePolicyResponse(policyEntity.Id, policyEntity.Title);
    }

    public async Task<GetPoliciesResponse> GetPolicies(GetPoliciesDto query, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetPolicies], cancellationToken);

        logger.LogDebug("Retrieving policies with filter: {Filter}, include: {Include}, page: {Page}, size: {PageSize}", query.Filter ?? '\0', query.Include ?? string.Empty, query.Page, query.PageSize);

        var baseQuery = policyRepository.GetAll();

        if (query.Filter is char c && c is >= 'A' and <= 'Z' or >= 'a' and <= 'z')
        {
            baseQuery = baseQuery.Where(p => EF.Functions.Like(EF.Property<string>(p, nameof(Policy.Title)), $"{c}%"));
        }

        baseQuery = baseQuery
            .IncludeElements(query.Include ?? string.Empty)
            .SortBy(query.SortBy, query.Ascending);

        var totalCount = await baseQuery.CountAsync(cancellationToken).ConfigureAwait(false);

        var pagedQuery = baseQuery.Paginate(query.Page, query.PageSize);

        var policies = await pagedQuery
            .Select(p => new PolicyDto(
                p.Id,
                p.Title,
                p.Description,
                p.IsActive
            ))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        logger.LogDebug("Retrieved {Count} policies", policies.Count);

        return new GetPoliciesResponse(policies, totalCount);
    }

    public async Task<GetPolicyResponse> GetPolicyById(GetPolicyDto dto, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetPolicy], cancellationToken);

        logger.LogDebug("Fetching policy with Id: {PolicyId}", dto.Id);

        var policy = await policyRepository.GetById(dto.Id)
            .IncludeElements(dto.Include ?? string.Empty)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroPolicyNotFoundException(dto.Id.ToString());

        var policyDto = new PolicyDto(
            policy.Id,
            policy.Title,
            policy.Description,
            policy.IsActive
        );

        return new GetPolicyResponse(policyDto);
    }

    public async Task<PatchPolicyResponse> PatchPolicy(PatchPolicyDto dto, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.PatchPolicy], cancellationToken);

        var patch = dto.Policy;

        logger.LogDebug("Patching policy with Id: {PolicyId}", patch.Id);

        var existingPolicy = await policyRepository.GetById(patch.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroPolicyNotFoundException(patch.Id.ToString());

        patch.Title.PatchIfNotNull(v => existingPolicy.Title = v);
        patch.Description.PatchIfNotNull(v => existingPolicy.Description = v);
        patch.IsActive.PatchIfNotNull(v => existingPolicy.IsActive = v);

        policyRepository.Update(existingPolicy);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Policy patched successfully. Id: {PolicyId}", existingPolicy.Id);

        var policyDto = new PolicyDto(
            existingPolicy.Id,
            existingPolicy.Title,
            existingPolicy.Description,
            existingPolicy.IsActive
        );

        return new PatchPolicyResponse(policyDto);
    }

    public async Task<DeletePolicyResponse> DeletePolicy(DeletePolicyDto dto, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.DeletePolicy], cancellationToken);

        logger.LogDebug("Deleting policy with Id: {PolicyId}", dto.Id);

        var existingPolicy = await policyRepository.GetById(dto.Id)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroPolicyNotFoundException(dto.Id.ToString());

        policyRepository.Delete(existingPolicy);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Policy deleted successfully. Id: {PolicyId}", dto.Id);

        return new DeletePolicyResponse(dto.Id);
    }
}

