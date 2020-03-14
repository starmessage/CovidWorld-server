﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sygic.Corona.Domain;
using Sygic.Corona.Domain.Common;

namespace Sygic.Corona.Infrastructure.Repositories
{
    public class CoronaRepository : IRepository
    {
        private readonly CoronaContext context;
        public IUnitOfWork UnitOfWork => context;

        public CoronaRepository(CoronaContext context)
        {
            this.context = context;
        }
        public async Task CreateProfileAsync(Profile profile, CancellationToken cancellationToken)
        {
            //await context.Database.EnsureCreatedAsync(cancellationToken);
            await context.Profiles.AddAsync(profile, cancellationToken);
        }

        public async Task CreateContactAsync(Contact contact, CancellationToken cancellationToken)
        {
            await context.Contacts.AddAsync(contact, cancellationToken);
        }

        public Task<Profile> GetProfileAsync(uint profileId, string deviceId, CancellationToken cancellationToken)
        {
            return context.Profiles.SingleOrDefaultAsync(x => x.Id == profileId && x.DeviceId == deviceId, cancellationToken);
        }

        public Task<Profile> GetProfileAsync(string deviceId, CancellationToken cancellationToken)
        {
            return context.Profiles.SingleOrDefaultAsync(x => x.DeviceId == deviceId, cancellationToken);
        }

        public async Task<uint> GetLastIdAsync(CancellationToken cancellationToken)
        {
            return await context.Profiles.OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> AlreadyCreatedAsync(string deviceId, CancellationToken cancellationToken)
        {
            var profile = await context.Profiles.FirstOrDefaultAsync(x => x.DeviceId == deviceId, cancellationToken);

            return profile != null;
        }

        public async Task<string> GetProfilePushTokenAsync(uint profileId, string deviceId, CancellationToken cancellationToken)
        {
            var result = await context.Profiles.Select(x => new {x.Id, x.DeviceId, x.PushToken})
                .SingleOrDefaultAsync(x => x.Id == profileId && x.DeviceId == deviceId, cancellationToken);
            return result.PushToken;
        }

        public async Task<string> GetProfilePushTokenAsync(uint profileId, CancellationToken cancellationToken)
        {
            var result = await context.Profiles.Where(x => x.Id == profileId)
                .Select(x => new { x.PushToken })
                .FirstOrDefaultAsync(cancellationToken);

            return result.PushToken;
        }

        public async Task<string> GetProfileMfaTokenAsync(uint profileId, string deviceId, CancellationToken cancellationToken)
        {
            var result = await context.Profiles.Where(x => x.Id == profileId && x.DeviceId == deviceId)
                .Select(x => new { x.AuthToken })
                .FirstOrDefaultAsync(cancellationToken);

            return result.AuthToken;
        }

        public async Task<IEnumerable<Contact>> GetContactsForProfileAsync(uint profileId, CancellationToken cancellationToken)
        {
            return await context.Contacts.Where(x => x.SeenProfileId == profileId)
                .ToListAsync(cancellationToken);
        }
    }
}
