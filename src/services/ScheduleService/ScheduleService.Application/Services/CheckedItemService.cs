using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;

public class CheckedItemService : ICheckedItemService
{
    private readonly ICheckedItemRepository _checkedItemRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly ICheckItemParticipantRepository _checkItemParticipantRepository;

    public CheckedItemService(
        ICheckedItemRepository checkedItemRepository,
        IScheduleRepository scheduleRepository,
        IScheduleParticipantRepository scheduleParticipantRepository,
        IAuthServiceClient authServiceClient,
        ICheckItemParticipantRepository checkItemParticipantRepository)
    {
        _checkedItemRepository = checkedItemRepository;
        _scheduleRepository = scheduleRepository;
        _scheduleParticipantRepository = scheduleParticipantRepository;
        _authServiceClient = authServiceClient;
        _checkItemParticipantRepository = checkItemParticipantRepository;
    }

    public async Task AddCheckedItemsAsync(List<CheckedItem> items)
    {
        var participants = await _scheduleParticipantRepository.GetAllParticipantByScheduleIdAsync(items[0].ScheduleId);

        await _checkedItemRepository.AddCheckedItemsAsync(items);

        var checkedItemParticipants = new List<CheckedItemParticipant>();

        foreach (var participant in participants)
        {
            foreach (var item in items)
            {
                checkedItemParticipants.Add(new CheckedItemParticipant
                {
                    CheckedItemId = item.Id,
                    ScheduleParticipantId = participant.Id,
                    IsChecked = false,
                    IsDeleted = false,
                    CheckedAt = DateTime.UtcNow
                });
            }
        }

        await _checkItemParticipantRepository.AddRangeAsync(checkedItemParticipants);
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

        //var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
        //if (participant == null) throw new Exception("You do not have permission to view checked items of this schedule");
        var checkItems = await _checkedItemRepository.GetByScheduleIdAsync(scheduleId);
        if (!checkItems.Any()) return new List<CheckedItem>();
        return checkItems;
    }

    public async Task<List<CheckedItem>> GetAvailableByScheduleIdAsync(Guid scheduleId)
    {
        var user = await _authServiceClient.GetCurrentAccountAsync();
        var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule == null) throw new Exception("Schedule not found");

        //var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
        //if (participant == null) throw new Exception("You do not have permission to view checked items of this schedule");

        var checkItems = await _checkedItemRepository.GetAvailableByScheduleIdAsync(scheduleId);
        if (!checkItems.Any()) return new List<CheckedItem>();
        return checkItems;
    }

    public async Task DeleteManyById(List<int> itemIds)
    {
        var currentUser = await _authServiceClient.GetCurrentAccountAsync();
        if (currentUser == null) throw new Exception("Can not find user");

        var item = await _checkedItemRepository.GetByIdAsync(itemIds[0]);
        if (item == null) throw new Exception("Can not find item");

        var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(currentUser.Id, item.ScheduleId);
        if (participant == null) throw new Exception("Can not find participant");

        if (participant.Role != ParticipantRole.Owner)
            throw new UnauthorizedAccessException("You are not the Owner of this schedule");

        await _checkedItemRepository.DeleteManyAsync(itemIds);

        // Delete CheckedItemParticipants
        await _checkItemParticipantRepository.DeleteManyAsync(itemIds);
    }
}