using Contracts.Event.User;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NoteService.Data;
using NoteService.Modules.Notes.Model;

namespace NoteService.Modules.Notes.Consumer
{
    public class UserUpsertConsumer(NoteDbContext noteDbContext, ILogger<UserUpsertConsumer> logger) : IConsumer<UserUpsertEvent>
    {
        public async Task Consume(ConsumeContext<UserUpsertEvent> context)
        {
            var evt = context.Message;
            var user = await noteDbContext.Users.Where(u=>u.OldUserId == evt.UserId).FirstOrDefaultAsync();

            if (user != null)
            {
                noteDbContext.Users.Remove(user);   
            }
            user = new User
            {
                OldUserId = evt.UserId,
                Name = evt.Name ?? string.Empty,
                UserName = evt.UserName ?? string.Empty,
                UpdatedAt =evt.UpdatedAt
            };
            noteDbContext.Users.Add(user);
            await noteDbContext.SaveChangesAsync();
            logger.LogInformation("User upserted: {UserId}", evt.UserId);
        }
    }
}
