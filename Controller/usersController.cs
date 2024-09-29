using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using log4net;
using LibraryManagementSystem.Services;


namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;
        private readonly DatabaseService _databaseService;
        private static readonly ILog log = LogManager.GetLogger(typeof(usersController));
        public usersController(LibraryContext context, IConfiguration configuration, DatabaseService databaseService)
        {
            _context = context;
            _configuration = configuration;
            _databaseService = databaseService;
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public DateTime Timestamp { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                // Checking for empty fields
                if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "The Username and Password fields are required.");
                }

                // Encoding the password
                var password = Encoding.UTF8.GetBytes(loginRequest.Password);
                var encodedPassword = Convert.ToBase64String(password);

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@username", loginRequest.Username },
                    { "@password", encodedPassword }
                };

                // Call the stored procedure to check the user credentials
                DataTable userInfoTable = await _databaseService.RunStoredProcedureAsync("prc_CheckUserCredential", parameters);

                // Check if user exists
                if (userInfoTable.Rows.Count > 0)
                {
                    var userId = (Guid)userInfoTable.Rows[0]["UserId"];
                    var roleName = (string)userInfoTable.Rows[0]["RoleName"];
                    var expirationTime = DateTime.UtcNow.AddHours(24);
                    Guid sessionId = Guid.NewGuid();

                    // Generate token and then insert it 
                    var token = GenerateJwtToken(sessionId.ToString(), userId.ToString(), roleName);

                    var tokenParams = new Dictionary<string, object>
                    {
                        { "@sessionID", sessionId },
                        { "@userId", userId },
                        { "@token", token },
                        { "@dateCreated", DateTime.UtcNow },
                        { "@dateExpired", expirationTime }
                    };

                    // Call the stored procedure to insert session token
                    await _databaseService.RunStoredProcedureAsync("prc_InsertSessionToken", tokenParams);

                    return Ok(new
                    {
                        sessionToken = token,
                        userId = userId,
                        expiryDate = expirationTime
                    });
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "The user could not be found");
                }
            }
            catch (SqlException ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", "Database/SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("getrole")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                // Call the stored procedure to fetch roles 
                DataTable rolesTable = await _databaseService.RunStoredProcedureAsync("prc_GetAllRoles", new Dictionary<string, object>());

                // Return table if the roles are found
                if (rolesTable.Rows.Count > 0)
                {
                    var rolesList = new List<object>();

                    foreach (DataRow row in rolesTable.Rows)
                    {
                        rolesList.Add(new
                        {
                            RoleId = row["RoleId"],
                            RoleName = row["RoleName"]
                        });
                    }

                    return Ok(new { roleList = rolesList });
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No roles found.");
                }
            }
            catch (SqlException ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", "Database/SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", ex.Message);
            }
        }

        public class RegisterRequest
        {
            public string RoleId { get; set; }
            public string Username { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("registeruser")]
        public async Task<IActionResult> RegisterUser(RegisterRequest registerRequest)
        {
            try
            {
                // Checking for empty fields
                if (registerRequest == null || 
                    string.IsNullOrWhiteSpace(registerRequest.RoleId) || 
                    string.IsNullOrWhiteSpace(registerRequest.Username) || 
                    string.IsNullOrWhiteSpace(registerRequest.DisplayName) ||
                    string.IsNullOrWhiteSpace(registerRequest.Email) ||
                    string.IsNullOrWhiteSpace(registerRequest.Phone))
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "All fields are required.");
                }

                // Creating temporary password and insert new user to the database
                Guid userId = Guid.NewGuid();
                var temp = registerRequest.Username + "password";
                var password = Encoding.UTF8.GetBytes(temp);
                string encodedPassword = Convert.ToBase64String(password);
                var dateCreated = DateTime.UtcNow;

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@userId", userId },
                    { "@roleId" , registerRequest.RoleId},
                    { "@username", registerRequest.Username },
                    { "@password", encodedPassword },
                    { "@displayName", registerRequest.DisplayName },
                    { "@email", registerRequest.Email },
                    { "@phone", registerRequest.Phone },
                    { "@dateCreated", dateCreated },
                };

                // Call the stored procedure to register the user
                DataTable user = await _databaseService.RunStoredProcedureAsync("prc_RegisterUser", parameters);

                return Ok(new
                {
                    username = registerRequest.Username,
                    password = temp,
                    displayName = registerRequest.DisplayName,
                    userid = userId,
                });
            }
            catch (SqlException ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", "Database/SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Fetch session id from token
                var sessionId = User.FindFirst("sid")?.Value;
                if (sessionId == null)
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "Session Id could not be found in the token.");
                }

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@sessionId", Guid.Parse(sessionId) },
                    { "@dateUpdated", DateTime.UtcNow }
                };

                // Call the stored procedure to log out the user
                await _databaseService.RunStoredProcedureAsync("prc_Logout", parameters);
                var currentSes = await _context.SessionTokens.FindAsync(Guid.Parse(sessionId));
                return Ok();
            }
            catch (SqlException ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", "Database/SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", ex.Message);
            }
        }

        public class ViewUserRequest
        {
            public string? DisplayName { get; set; }
            public string? UserEmail { get; set; }
            public string? UserPhone { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }

        public class UserObject
        {
            public Guid UserId { get; set; }
            public string DisplayName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public DateTime CreatedDateUTC { get; set; }
            public DateTime LastUpdatedDateUTC { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("viewuser")]
        public async Task<IActionResult> ViewUsers(ViewUserRequest viewUserRequest)
        {
            try
            {
                if (viewUserRequest.PageIndex < 1 || viewUserRequest.PageSize < 1)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "PageIndex and PageSize must be greater than zero.");
                }
                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@displayName", viewUserRequest.DisplayName},
                    { "@userEmail", viewUserRequest.UserEmail },
                    { "@userPhone", viewUserRequest.UserPhone }
                };

                // Call the stored procedure to get all users matching the filters
                DataTable usersTable = await _databaseService.RunStoredProcedureAsync("prc_ViewUsers", parameters);

                // Process the DataTable to get the users
                var users = new List<UserObject>();
                foreach (DataRow row in usersTable.Rows)
                {
                    users.Add(new UserObject
                    {
                        UserId = (Guid)row["UserId"],
                        DisplayName = row["DisplayName"].ToString(),
                        Email = row["Email"].ToString(),
                        Phone = row["Phone"].ToString(),
                        CreatedDateUTC = (DateTime)row["CreatedDateUTC"],
                        LastUpdatedDateUTC = (DateTime)row["LastUpdatedDateUTC"]
                    });
                }

                // Apply pagination
                var paginatedUsers = users
                    .Skip((viewUserRequest.PageIndex - 1) * viewUserRequest.PageSize)
                    .Take(viewUserRequest.PageSize)
                    .ToList();

                // Return the paginated list and total count
                return Ok(new
                {
                    userList = paginatedUsers,
                    totalCount = users.Count
                });
            }
            catch (SqlException ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", "Database/SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", ex.Message);
            }
        }

        public class UpdateUserDetailsRequest
        {
            public Guid UserId { get; set; }
            public string? DisplayName { get; set; } = null;
            public string? UserEmail { get; set; } = null;
            public string? UserPhone { get; set; } = null;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updateuser")]
        public async Task<IActionResult> UpdateUserDetails(UpdateUserDetailsRequest updateUserRequest)
        {
            try
            {
                // Checking for empty fields
                if (updateUserRequest == null || updateUserRequest.UserId == Guid.Empty)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "UserId is required.");
                }
                var user = await _context.UserInfos
                                .FirstOrDefaultAsync(u => u.UserId == updateUserRequest.UserId);

                if (user == null)
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No user found with the given UserId.");
                }

                // Fetch the role of the user from the UserRoles table
                var userRole = await (from ui in _context.UserInfos
                                      join ur in _context.UserRoles on ui.RoleId equals ur.RoleId
                                      where ui.UserId == updateUserRequest.UserId
                                      select ur.RoleName).FirstOrDefaultAsync();

                // Get the UserId from the token 
                var userId = User.FindFirst("uid")?.Value;

                if (userId == null)
                {
                    return _databaseService.CreateErrorResponse(401, "Unauthorized", "Invalid token, no uid found.");
                }

                // Checking role constraints
                if (userRole == "Admin" && updateUserRequest.UserId.ToString() != userId)
                {
                    return _databaseService.CreateErrorResponse(401, "Unauthorized", "You cannot modify details of another Admin.");
                }

                var updatedDisplayName = !string.IsNullOrWhiteSpace(updateUserRequest.DisplayName) ? updateUserRequest.DisplayName : user.DisplayName;
                var updatedEmail = !string.IsNullOrWhiteSpace(updateUserRequest.UserEmail) ? updateUserRequest.UserEmail : user.Email;
                var updatedPhone = !string.IsNullOrWhiteSpace(updateUserRequest.UserPhone) ? updateUserRequest.UserPhone : user.Phone;
                var updatedDate = DateTime.UtcNow;

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@userId", updateUserRequest.UserId },
                    { "@displayName", updatedDisplayName },
                    { "@userEmail", updatedEmail },
                    { "@userPhone", updatedPhone },
                    { "@lastUpdatedDate" , updatedDate}
                };

                // Call the stored procedure to update the user details
                DataTable resultTable = await _databaseService.RunStoredProcedureAsync("prc_UpdateUserDetails", parameters);
                var lastUpdatedBy = await (from ui in _context.UserInfos
                                           where ui.UserId.ToString() == userId
                                           select ui.DisplayName).FirstOrDefaultAsync();
                // Check if the update was successful by verifying if any rows were returned
                if (resultTable.Rows.Count > 0)
                {
                    var userDetails = new
                    {
                        UserId = resultTable.Rows[0]["UserId"],
                        DisplayName = resultTable.Rows[0]["DisplayName"].ToString(),
                        Email = resultTable.Rows[0]["Email"].ToString(),
                        Phone = resultTable.Rows[0]["Phone"].ToString(),
                        LastUpdatedDateUTC = resultTable.Rows[0]["LastUpdatedDateUTC"],
                        LastUpdatedBy = lastUpdatedBy
                    };

                    return Ok(userDetails);
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No user found with the given UserId.");
                }
            }
            catch (SqlException ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", "Database/SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _databaseService.CreateErrorResponse(500, "Internal Server Error", ex.Message);
            }
        }

        private string GenerateJwtToken(string sessionId, string userId, string roleName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Getting jwt token configuration from appsettings.json
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]); 
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // Create token descriptor 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("sid", sessionId),
                    new Claim("uid", userId),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim("ctd", DateTime.UtcNow.ToString("o"))
                }),
                // Set expiration to 24 hours
                Expires = DateTime.UtcNow.AddHours(24),   
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
