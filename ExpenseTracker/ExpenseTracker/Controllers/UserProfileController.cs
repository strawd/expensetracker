// Copyright 2016 David Straw

using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
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
        //public Task<UserProfile> PatchUserProfile(string id, Delta<UserProfile> patch)
        //{
        //    var userSid = this.GetCurrentUserSid();

        //    var query = _context.UserProfiles.Where(userProfile => userProfile.UserId == userSid && userProfile.Id == id);
        //    if (!query.Any())
        //        NotFound();

        //    if (patch.GetChangedPropertyNames().Contains(nameof(UserProfile.UserId), StringComparer.OrdinalIgnoreCase))
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Unable to modify the UserId property of a user profile."));

        //    return UpdateAsync(id, patch);
        //}

        // POST tables/UserProfile
        public async Task<IHttpActionResult> PostUserProfile(UserProfile item)
        {
            var userSid = this.GetCurrentUserSid();

            //var query = _context.UserProfiles.Where(userProfile => userProfile.UserId == userSid);
            //if (query.Any())
            //    Conflict();

            item.UserId = userSid;

            return new SimpleActionResult(Request);

            //UserProfile current = await InsertAsync(item);
            //return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        class SimpleActionResult : IHttpActionResult
        {
            HttpRequestMessage _request;

            public SimpleActionResult(HttpRequestMessage request)
            {
                _request = request;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = _request.CreateResponse(System.Net.HttpStatusCode.Created);
                response.Content = new StringContent("{ \"Hello\": \"World\" }", Encoding.UTF8, "application/json");
                return Task.FromResult(response);
            }
        }
    }
}