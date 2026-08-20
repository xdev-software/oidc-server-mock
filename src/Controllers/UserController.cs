using System.Security.Claims;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Mvc;
using OpenIdConnectServerMock.Test;

namespace OpenIdConnectServer.Controllers
{
  [Route("api/v1/user")]
    public class UserController: Controller
    {
        private readonly MutableTestUserStore _usersStore;
        private readonly ILogger _logger;

        public UserController(MutableTestUserStore userStore, ILogger<UserController> logger)
        {
            _usersStore = userStore;
            _logger = logger;
        }

        [HttpGet("{subjectId}")]
        public IActionResult GetUser([FromRoute]string subjectId)
        {
            var user = _usersStore.FindBySubjectId(subjectId);
            if (user == null)
                return NotFound();

            _logger.LogDebug("User found: {subjectId}", subjectId);
            return Json(user);
        }

        [HttpPost]
        public IActionResult AddUser(
            // Enabling this (using ?validate=true) will execute a duplicate lookup
            // This is disabled by default due to performance reasons (the underlying store uses a List instead of a Map/Dictionary)
            [FromQuery(Name = "validate")] bool validate,
            [FromBody]TestUser user)
        {
            if (validate)
            {
                if (_usersStore.FindBySubjectId(user.SubjectId) != null)
                    return Conflict($"User with SubjectId {user.SubjectId} already exists");
            }

            var newUser =_usersStore.AutoProvisionUser("Dummy", user.SubjectId, new List<Claim>(user.Claims));
            newUser.SubjectId = user.SubjectId;
            newUser.Username = user.Username;
            newUser.Password = user.Password;
            newUser.ProviderName = string.Empty;
            newUser.ProviderSubjectId = string.Empty;

            _logger.LogDebug("New user added: {user}", user.SubjectId);

            return Json(user.SubjectId);
        }

        [HttpPut]
        public IActionResult ReplaceUser([FromBody] TestUser user)
        {
            // Clean (same logic as AddUser above)
            user.Claims = _usersStore.DetermineClaims(user.Claims);
            user.ProviderName = string.Empty;
            user.ProviderSubjectId = string.Empty;

            if (!_usersStore.ReplaceUser(user.SubjectId, user))
                return NotFound();

            _logger.LogDebug("Replaced user: {user}", user.SubjectId);

            return Json(user.SubjectId);
        }

        [HttpDelete("{subjectId}")]
        public IActionResult DeleteUser([FromRoute]string subjectId)
        {
            if (!_usersStore.RemoveUser(subjectId))
                return NotFound();

            _logger.LogDebug("User deleted: {subjectId}", subjectId);
            return Json(subjectId);
        }
    }
}
