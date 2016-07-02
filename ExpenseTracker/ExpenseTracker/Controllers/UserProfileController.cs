// Copyright 2016 David Straw

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using ExpenseTracker.DataObjects;
using ExpenseTracker.Models;
using Microsoft.Azure.Mobile.Server;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class UserProfileController : TableController<UserProfile>
    {
        private MobileServiceContext _context;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<UserProfile>(_context, Request);
        }

        // GET tables/UserProfile
        public IQueryable<UserProfile> GetAllUserProfiles()
        {
            var userSid = this.GetCurrentUserSid();

            return Query().Where(userProfile => userProfile.UserId == userSid);
        }

        // GET tables/UserProfile/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<UserProfile> GetUserProfile(string id)
        {
            var userSid = this.GetCurrentUserSid();

            var query = _context.UserProfiles.Where(userProfile => userProfile.UserId == userSid && userProfile.Id == id);

            return SingleResult.Create(query);
        }

        // PATCH tables/UserProfile/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<UserProfile> PatchUserProfile(string id, Delta<UserProfile> patch)
        {
            var userSid = this.GetCurrentUserSid();

            var query = _context.UserProfiles.Where(userProfile => userProfile.UserId == userSid && userProfile.Id == id);
            if (!query.Any())
                throw new HttpResponseException(HttpStatusCode.NotFound);

            if (patch.GetChangedPropertyNames().Contains(nameof(UserProfile.UserId), StringComparer.OrdinalIgnoreCase))
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Unable to modify the UserId property of a user profile."));

            return UpdateAsync(id, patch);
        }

        // POST tables/UserProfile
        public async Task<IHttpActionResult> PostUserProfile(UserProfile item)
        {
            var userSid = this.GetCurrentUserSid();

            var query = _context.UserProfiles.Where(userProfile => userProfile.UserId == userSid);
            if (query.Any())
                return Conflict();

            item.UserId = userSid;
            item.GivenName = this.GetCurrentUserGivenName();

            UserProfile current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
    }
}