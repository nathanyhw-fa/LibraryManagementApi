using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using log4net;


namespace LibraryManagementSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class membersController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly DatabaseService _databaseService;
        private static readonly ILog log = LogManager.GetLogger(typeof(membersController));
        public membersController(LibraryContext context, DatabaseService databaseService) {
            _context = context;
            _databaseService = databaseService;
        }

        public class RegisterMemberRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        [Authorize]
        [HttpPost("registermember")]
        public async Task<IActionResult> RegisterMember(RegisterMemberRequest registerMemberRequest)
        {
            try
            {
                // Checking for empty fields
                if ((registerMemberRequest == null ||
                    string.IsNullOrWhiteSpace(registerMemberRequest.FullName) ||
                    string.IsNullOrWhiteSpace(registerMemberRequest.Email) ||
                    string.IsNullOrWhiteSpace(registerMemberRequest.Phone))) 
                { 
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "All fields are required.");
                }
                Guid memberId = Guid.NewGuid();
                var dateCreated = DateTime.UtcNow;
                var userId = User.FindFirst("uid")?.Value;
                if (userId == null)
                {
                    return _databaseService.CreateErrorResponse(401, "Unauthorized", "Invalid token, no uid found.");
                }

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@memberId", memberId },
                    { "@fullname", registerMemberRequest.FullName },
                    { "@email", registerMemberRequest.Email },
                    { "@phone", registerMemberRequest.Phone },
                    { "@userId", userId },
                    { "@dateCreated", dateCreated },
                };

                // Call the stored procedure to register member
                DataTable resultTable = await _databaseService.RunStoredProcedureAsync("prc_RegisterMember", parameters);
                
                return Ok(new
                {
                    fullName = registerMemberRequest.FullName,
                    email = registerMemberRequest.Email,
                    memberId = memberId,
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

        public class ViewMemberRequest
        {
            public string? MemberName { get; set; } = null;
            public string? MemberEmail { get; set; } = null;
            public string? MemberPhone { get; set; } = null;
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }

        public class UserObject
        {
            public Guid MemberId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }   
            public string CreatedBy { get; set; }
            public string LastUpdatedBy { get; set; }
            public DateTime CreatedDateUTC { get; set; }
            public DateTime LastUpdatedDateUTC { get; set; }
        }

        [Authorize]
        [HttpPost("viewmember")]
        public async Task<IActionResult> ViewMember(ViewMemberRequest viewMemberRequest)
        {
            try
            {
                if (viewMemberRequest.PageIndex < 1 || viewMemberRequest.PageSize < 1)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "PageIndex and PageSize must be greater than zero.");
                }
                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@memberName", viewMemberRequest.MemberName},
                    { "@memberEmail", viewMemberRequest.MemberEmail },
                    { "@memberPhone", viewMemberRequest.MemberPhone }
                };

                // Call the stored procedure to view the members details that are matching with the filter
                DataTable membersTable = await _databaseService.RunStoredProcedureAsync("prc_ViewMembers", parameters);

                // Process the DataTable to get the users
                var members = new List<UserObject>();
                if (membersTable.Rows.Count > 0)
                {
                    foreach (DataRow row in membersTable.Rows)
                    {
                        var createdByDisplayName = await _context.UserInfos
                            .Where(u => u.UserId == (Guid)row["CreatedByUserId"])
                            .Select(u => u.DisplayName)
                            .FirstOrDefaultAsync();
                        var lastUpdatedByDisplayName = await _context.UserInfos
                            .Where(u => u.UserId == (Guid)row["LastUpdatedByUserId"])
                            .Select(u => u.DisplayName)
                            .FirstOrDefaultAsync();
                        members.Add(new UserObject
                        {
                            MemberId = (Guid)row["MemberId"],
                            FullName = row["FullName"].ToString(),
                            Email = row["Email"].ToString(),
                            Phone = row["Phone"].ToString(),
                            CreatedBy = createdByDisplayName,
                            CreatedDateUTC = (DateTime)row["CreatedDateUTC"],
                            LastUpdatedBy = lastUpdatedByDisplayName,
                            LastUpdatedDateUTC = (DateTime)row["LastUpdatedDateUTC"]
                        });
                    }

                    // Apply pagination
                    var paginatedMembers = members
                        .Skip((viewMemberRequest.PageIndex - 1) * viewMemberRequest.PageSize)
                        .Take(viewMemberRequest.PageSize)
                        .ToList();

                    // Return the paginated list and total count
                    return Ok(new
                    {
                        memberList = paginatedMembers,
                        totalCount = members.Count
                    });
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "Members matching the filter could not be found" );
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

        public class UpdateMemberRequest
        {
            public Guid MemberId { get; set; }
            public string? MemberName { get; set; } = null;
            public string? MemberEmail { get; set; } = null;
            public string? MemberPhone { get; set; } = null;
        }

        [Authorize]
        [HttpPost("updatemember")]
        public async Task<IActionResult> UpdateMemberDetails(UpdateMemberRequest updateMemberRequest)
        {
            try {
                // Checking for empty fields
                if (updateMemberRequest == null || updateMemberRequest.MemberId == Guid.Empty)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "MemberId is required.");
                }
                var member = await _context.MemberInfos
                                .FirstOrDefaultAsync(u => u.MemberId == updateMemberRequest.MemberId);
                if (member == null)
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No member found with the given MemberId.");
                }

                var updatedMemberName = !string.IsNullOrWhiteSpace(updateMemberRequest.MemberName) ? updateMemberRequest.MemberName : member.FullName;
                var updatedMemberEmail = !string.IsNullOrWhiteSpace(updateMemberRequest.MemberEmail) ? updateMemberRequest.MemberEmail : member.Email;
                var updatedMemberPhone = !string.IsNullOrWhiteSpace(updateMemberRequest.MemberPhone) ? updateMemberRequest.MemberPhone : member.Phone;
                var updatedDate = DateTime.UtcNow;
                var userId = User.FindFirst("uid")?.Value;

                if (userId == null)
                {
                    return _databaseService.CreateErrorResponse(401, "Unauthorized", "Invalid token, no uid found.");
                }

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@memberId",updateMemberRequest.MemberId.ToString()},
                    { "@memberName", updatedMemberName },
                    { "@memberEmail", updatedMemberEmail },
                    { "@memberPhone", updatedMemberPhone },
                    { "@lastUpdatedDate",  updatedDate },
                    { "@userId", userId }
                };

                // Call the stored procedure to update member details
                DataTable memberTable = await _databaseService.RunStoredProcedureAsync("prc_UpdateMemberDetails", parameters);
                if (memberTable.Rows.Count > 0) {
                    var createdBy = await (from ui in _context.UserInfos
                                               where ui.UserId.ToString() == memberTable.Rows[0]["CreatedByUserId"].ToString()
                                               select ui.DisplayName).FirstOrDefaultAsync();
                    var lastUpdatedBy = await (from ui in _context.UserInfos
                                           where ui.UserId.ToString() == memberTable.Rows[0]["LastUpdatedByUserId"].ToString()
                                           select ui.DisplayName).FirstOrDefaultAsync();
                    var memberDetails = new
                    {
                        memberId = memberTable.Rows[0]["MemberId"].ToString(),
                        fullName = memberTable.Rows[0]["FullName"].ToString(),
                        email = memberTable.Rows[0]["Email"].ToString(),
                        phone = memberTable.Rows[0]["Phone"].ToString(),
                        createdBy = createdBy,
                        createdDateUtc = memberTable.Rows[0]["CreatedDateUTC"],
                        lastUpdatedBy = lastUpdatedBy,
                        lastUpdatedDateUTC = memberTable.Rows[0]["LastUpdatedByUserId"]
                    };
                    return Ok(memberDetails);
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No member found with the given MemberId.");
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

    }
}
