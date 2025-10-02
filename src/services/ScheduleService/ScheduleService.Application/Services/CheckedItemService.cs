using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;

public class CheckedItemService : ICheckedItemService
{
    private readonly ICheckedItemRepository _checkedItemRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
    private readonly IAuthServiceClient _authServiceClient;

    public CheckedItemService(
        ICheckedItemRepository checkedItemRepository,
        IScheduleRepository scheduleRepository,
        IScheduleParticipantRepository scheduleParticipantRepository,
        IAuthServiceClient authServiceClient)
    {
        _checkedItemRepository = checkedItemRepository;
        _scheduleRepository = scheduleRepository;
        _scheduleParticipantRepository = scheduleParticipantRepository;
        _authServiceClient = authServiceClient;
    }

    public async Task AddCheckedItemsAsync(List<CheckedItem> items)
    {
        await _checkedItemRepository.AddCheckedItemsAsync(items);
    }

    public async Task SaveChangesAsync()
    {
        await _checkedItemRepository.SaveChangesAsync();
    }

    public async Task<List<CheckedItem>> GetByScheduleIdAsync(Guid scheduleId)
    {
        var user = await _authServiceClient.GetCurrentAccountAsync();
        var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule == null) throw new Exception("Schedule not found");

        var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
        if (participant == null) throw new Exception("You do not have permission to view checked items of this schedule");

        return await _checkedItemRepository.GetByScheduleIdAsync(scheduleId);
    }

    public async Task DeleteManyById(List<int> itemIds)
    {
        await _checkedItemRepository.DeleteManyAsync(itemIds);
    }
}