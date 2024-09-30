using LibraryManagementSystem.Data;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace LibraryManagementSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class booksController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly DatabaseService _databaseService;
        private static readonly ILog log = LogManager.GetLogger(typeof(booksController));

        public booksController(LibraryContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        public class GetBookRequest
        {
            public string? Title { get; set; } = null;
            public string? Author { get; set; } = null;
            public string? Isbn { get; set; } = null;

            public string? Genre { get; set; } = null;
            public string? Language { get; set; } = null;
            public bool IsInGoodConditionOnly { get; set; }
            public bool IsAvailable { get; set; }

            public int PageIndex { get; set; }
            public int PageSize { get; set; }
        }

        public class TrackingObject
        {
            public Guid TrackingId { get; set; }
            public string MemberName { get; set; }
            public string MemberEmail { get; set; }
            public string MemberPhone { get; set; }
            public DateOnly BorrowDate { get; set; }
            public DateOnly DueDate { get; set; }
        }

        public class BookObject
        {
            public Guid BookId { get; set; }
            public string Title { get; set; }
            public string Genre { get; set; }
            public string Author { get; set; }
            public string Synopsis { get; set; }
            public string Publisher { get; set; }
            public DateOnly PublicationDate { get; set; }
            public string Edition { get; set; }
            public int NumberOfPages { get; set; }
            public string Language { get; set; }
            public string? Isbn { get; set; } = null;
            public string Status { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDateUTC { get; set; }
            public string LastUpdatedBy { get; set; }
            public DateTime LastUpdatedDateUTC { get; set; }
            public TrackingObject? BookTracking { get; set; } = null;
        }

        public class MemberObject
        {
            public Guid MemberId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDateUTC { get; set; }
            public string LastUpdatedBy { get; set; }
            public DateTime LastUpdatedDateUTC { get; set; }
        }

        [Authorize]
        [HttpPost("getbooks")]
        public async Task<IActionResult> GetBooks(GetBookRequest getBookRequest)
        {
            try
            {
                // Checking for empty fields
                if (getBookRequest.PageIndex < 1 || getBookRequest.PageSize < 1)
                {
                    return _databaseService.CreateErrorResponse(400, "Invalid request parameter", "PageIndex and PageSize must be greater than zero.");
                }

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@title", getBookRequest.Title },
                    { "@author", getBookRequest.Author },
                    { "@isbn", getBookRequest.Isbn },
                    { "@genre", getBookRequest.Genre },
                    { "@language", getBookRequest.Language },
                    { "@isInGoodConditionOnly", getBookRequest.IsInGoodConditionOnly },
                    { "@isAvailable", getBookRequest.IsAvailable }
                };

                // Call the stored procedure to get books
                DataTable bookTable = await _databaseService.RunStoredProcedureAsync("prc_GetBooks", parameters);
                var books = new List<BookObject>();
                if (bookTable.Rows.Count > 0)
                {
                    foreach (DataRow row in bookTable.Rows)
                    {
                        var createdByDisplayName = await _context.UserInfos
                            .Where(u => u.UserId == (Guid)row["CreatedByUserId"])
                            .Select(u => u.DisplayName)
                            .FirstOrDefaultAsync();
                        var lastUpdatedByDisplayName = await _context.UserInfos
                            .Where(u => u.UserId == (Guid)row["LastUpdatedByUserId"])
                            .Select(u => u.DisplayName)
                            .FirstOrDefaultAsync();
                        var book = new BookObject
                        {
                            BookId = Guid.Parse(row["BookId"].ToString()),
                            Title = row["Title"].ToString(),
                            Genre = row["Genre"].ToString(),
                            Author = row["Author"].ToString(),
                            Synopsis = row["Synopsis"].ToString(),
                            Publisher = row["Publisher"].ToString(),
                            PublicationDate = DateOnly.FromDateTime(Convert.ToDateTime(row["PublicationDate"].ToString())),
                            Edition = row["Edition"].ToString(),
                            NumberOfPages = Convert.ToInt32(row["NumberOfPages"]),
                            Language = row["Language"].ToString(),
                            Isbn = row["Isbn"].ToString(),
                            Status = row["Status"].ToString(),
                            CreatedBy = createdByDisplayName,
                            CreatedDateUTC = Convert.ToDateTime(row["CreatedDateUTC"]),
                            LastUpdatedBy = lastUpdatedByDisplayName,
                            LastUpdatedDateUTC = Convert.ToDateTime(row["LastUpdatedDateUTC"]),
                            BookTracking = null
                        };

                        // If tracking info exists, populate the TrackingObject
                        if (!string.IsNullOrEmpty(row["TrackingId"].ToString()))
                        {
                            var tracking = new TrackingObject
                            {
                                TrackingId = Guid.Parse(row["TrackingId"].ToString()),
                                MemberName = row["FullName"].ToString(),
                                MemberEmail = row["Email"].ToString(),
                                MemberPhone = row["Phone"].ToString(),
                                BorrowDate = DateOnly.FromDateTime(Convert.ToDateTime(row["BorrowDate"])),
                                DueDate = DateOnly.FromDateTime(Convert.ToDateTime(row["DueDate"]))
                            };

                            book.BookTracking = tracking;
                        }

                        books.Add(book);
                    }
                    return Ok(new { bookList = books, totalCount = bookTable.Rows.Count });
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No books can be found");
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

        public class AddBookRequest
        {
            public string Title { get; set; }
            public string Genre { get; set; }
            public string Author { get; set; }
            public string Synopsis { get; set; }
            public string Publisher { get; set; }
            public DateOnly PublicationDate { get; set; }
            public string Edition { get; set; }
            public int NumberOfPages { get; set; }
            public string Language { get; set; }
            public string Isbn { get; set; }
            public int Status { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addbook")]
        public async Task<IActionResult> AddBook(AddBookRequest addBookRequest)
        {
            try
            {
                if (addBookRequest == null ||
                   string.IsNullOrWhiteSpace(addBookRequest.Title) ||
                   string.IsNullOrWhiteSpace(addBookRequest.Genre) ||
                   string.IsNullOrWhiteSpace(addBookRequest.Author) ||
                   string.IsNullOrWhiteSpace(addBookRequest.Synopsis) ||
                   string.IsNullOrWhiteSpace(addBookRequest.Publisher) ||
                   string.IsNullOrWhiteSpace(addBookRequest.Edition) ||
                   string.IsNullOrWhiteSpace(addBookRequest.Language) ||
                   string.IsNullOrWhiteSpace(addBookRequest.Isbn))
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "All fields are required.");
                }
                Guid bookId = Guid.NewGuid();
                var userId = User.FindFirst("uid")?.Value;
                var dateCreated = DateTime.UtcNow;

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@bookId",bookId },
                    { "@title", addBookRequest.Title },
                    { "@genre", addBookRequest.Genre },
                    { "@author", addBookRequest.Author},
                    { "@synopsis", addBookRequest.Synopsis },
                    { "@publisher", addBookRequest.Publisher},
                    { "@publicationDate",addBookRequest.PublicationDate},
                    { "@edition", addBookRequest.Edition},
                    { "@numberOfPages", addBookRequest.NumberOfPages},
                    { "@language", addBookRequest.Language },
                    { "@isbn", addBookRequest.Isbn },
                    { "@status", addBookRequest.Status },
                    { "@userId", userId },
                    { "@dateCreated",  dateCreated}
                };

                // Call the stored procedure to add book
                await _databaseService.RunStoredProcedureAsync("prc_AddBook", parameters);
                return Ok(new
                {
                    bookId = bookId,
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

        public class UpdateBookRequest
        {
            public Guid BookId { get; set; }
            public string? Title { get; set; }
            public string? Genre { get; set; }
            public string? Author { get; set; }
            public string? Synopsis { get; set; }
            public string? Publisher { get; set; }
            public DateOnly? PublicationDate { get; set; }
            public string? Edition { get; set; }
            public int? NumberOfPages { get; set; }
            public string? Language { get; set; }
            public string? Isbn { get; set; }
            public int? Status { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updatebook")]
        public async Task<IActionResult> UpdateBook(UpdateBookRequest updateBookRequest)
        {
            try
            {
                // Checking for empty fields
                if (updateBookRequest == null || updateBookRequest.BookId == Guid.Empty)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "BookId is required.");
                }
                var book = await _context.BookInfos
                                .FirstOrDefaultAsync(b => b.BookId == updateBookRequest.BookId);

                if (book == null)
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No book found with the given BookId.");
                }
                var updatedTitle = !string.IsNullOrWhiteSpace(updateBookRequest.Title) ? updateBookRequest.Title : book.Title;
                var updatedGenre = !string.IsNullOrWhiteSpace(updateBookRequest.Genre) ? updateBookRequest.Genre : book.Genre;
                var updatedAuthor = !string.IsNullOrWhiteSpace(updateBookRequest.Author) ? updateBookRequest.Author : book.Author;
                var updatedSynopsis = !string.IsNullOrWhiteSpace(updateBookRequest.Synopsis) ? updateBookRequest.Synopsis : book.Synopsis;
                var updatedPublisher = !string.IsNullOrWhiteSpace(updateBookRequest.Publisher) ? updateBookRequest.Publisher : book.Publisher;
                var updatedPublicationDate = updateBookRequest.PublicationDate.HasValue ? updateBookRequest.PublicationDate : book.PublicationDate;
                var updatedEdition = !string.IsNullOrWhiteSpace(updateBookRequest.Edition) ? updateBookRequest.Edition : book.Edition;
                var updatedNumberOfPages = updateBookRequest.NumberOfPages.HasValue ? updateBookRequest.NumberOfPages : book.NumberOfPages;
                var updatedLanguage = !string.IsNullOrWhiteSpace(updateBookRequest.Language) ? updateBookRequest.Language : book.Language;
                var updatedIsbn = !string.IsNullOrWhiteSpace(updateBookRequest.Isbn) ? updateBookRequest.Isbn : book.Isbn;
                var updatedStatus = updateBookRequest.Status.HasValue ? updateBookRequest.Status : book.Status;
                var userId = User.FindFirst("uid")?.Value;
                var dateUpdated = DateTime.UtcNow;

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@bookId", updateBookRequest.BookId },
                    { "@title", updatedTitle },
                    { "@genre", updatedGenre },
                    { "@author", updatedAuthor },
                    { "@synopsis", updatedSynopsis },
                    { "@publisher", updatedPublisher },
                    { "@publicationDate", updatedPublicationDate },
                    { "@edition", updatedEdition },
                    { "@numberOfPages", updatedNumberOfPages },
                    { "@language", updatedLanguage },
                    { "@isbn", updatedIsbn },
                    { "@status", updatedStatus },
                    { "@userId", userId },
                    { "@dateUpdated",  dateUpdated}

                };

                // Call the stored procedure to update book details
                DataTable bookTable = await _databaseService.RunStoredProcedureAsync("prc_UpdateBookDetails", parameters);
                if (bookTable.Rows.Count > 0)
                {
                    var lastUpdatedBy = await (from ui in _context.UserInfos
                                               where ui.UserId.ToString() == bookTable.Rows[0]["LastUpdatedByUserId"].ToString()
                                               select ui.DisplayName).FirstOrDefaultAsync();
                    var createdBy = await (from ui in _context.UserInfos
                                           where ui.UserId.ToString() == bookTable.Rows[0]["CreatedByUserId"].ToString()
                                           select ui.DisplayName).FirstOrDefaultAsync();
                    var status = bookTable.Rows[0]["Status"].ToString();
                    string statusDescription = string.Empty;
                    switch (status)
                    {
                        case "0":
                            statusDescription = "lost";
                            break;
                        case "1":
                            statusDescription = "good";
                            break;
                        case "-1":
                            statusDescription = "damaged";
                            break;
                    }

                    var bookDetails = new
                    {
                        BookId = bookTable.Rows[0]["BookId"],
                        Title = bookTable.Rows[0]["Title"],
                        Genre = bookTable.Rows[0]["Genre"],
                        Author = bookTable.Rows[0]["Author"],
                        Synopsis = bookTable.Rows[0]["Synopsis"],
                        Publisher = bookTable.Rows[0]["Publisher"],
                        PublicationDate = bookTable.Rows[0]["PublicationDate"],
                        Edition = bookTable.Rows[0]["Edition"],
                        NumberOfPages = bookTable.Rows[0]["NumberOfPages"],
                        Language = bookTable.Rows[0]["Language"],
                        Isbn = bookTable.Rows[0]["Isbn"],
                        Status = statusDescription,
                        CreateBy = createdBy,
                        CreatedDateUTC = bookTable.Rows[0]["CreatedDateUTC"],
                        LastUpdatedBy = lastUpdatedBy,
                        LastUpdateDate = bookTable.Rows[0]["LastUpdatedDateUTC"]
                    };
                    return Ok(bookDetails);
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Book Not Found", "No book found with the given BookId.");
                };
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

        public class TrackBookRequest
        {
            public Guid BookId { get; set; }
            public Guid MemberId { get; set; }
            public Guid? TrackingId { get; set; } = null;
            public Guid? ReserveId { get; set; } = null;
            public int? BorrowPeriodByDay { get; set; } = null;
        }

        [Authorize]
        [HttpPost("bookTrack")]
        public async Task<IActionResult> TrackBook(TrackBookRequest trackBookRequest)
        {
            try
            {
                string action = string.Empty;
                // Check for empty fields
                if (trackBookRequest == null || trackBookRequest.BookId == Guid.Empty || trackBookRequest.MemberId == Guid.Empty)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "BookId and MemberId is required.");
                }
                var date = DateTime.UtcNow;
                DateOnly? borrowDate = null;
                DateOnly? dueDate = null;
                DateOnly? returnDate = null;
                var userId = User.FindFirst("uid")?.Value;
                Guid trackingId = Guid.Empty;
                Guid reserveId = Guid.Empty;

                // Logic for borrowing a book
                if (trackBookRequest.TrackingId == null && trackBookRequest.BorrowPeriodByDay.HasValue)
                {
                    action = "borrow";
                    trackingId = Guid.NewGuid();
                    borrowDate = DateOnly.FromDateTime(date); // Convert current DateTime to DateOnly
                    dueDate = borrowDate.Value.AddDays(trackBookRequest.BorrowPeriodByDay.Value); // Adjust due date
                }
                // Logic for returning a book
                else if (trackBookRequest.TrackingId != Guid.Empty)
                {
                    action = "return";
                    trackingId = (Guid)trackBookRequest.TrackingId;
                    returnDate = DateOnly.FromDateTime(date); // Convert current DateTime to DateOnly
                }
                // Logic for reserving a book
                else if (trackBookRequest.ReserveId != Guid.Empty)
                {
                    action = "reserve";
                    reserveId = (Guid)trackBookRequest.ReserveId;
                    trackingId = Guid.NewGuid();
                    borrowDate = DateOnly.FromDateTime(date); // Convert current DateTime to DateOnly
                    dueDate = borrowDate.Value.AddDays(trackBookRequest.BorrowPeriodByDay.Value); // Adjust due date
                }
                else
                {
                    action = "undefined";
                    return _databaseService.CreateErrorResponse(500, "Internal Server Error", "");
                }

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@bookId", trackBookRequest.BookId },
                    { "@memberId", trackBookRequest.MemberId },
                    { "@trackingId", trackingId },
                    { "@reserveId", reserveId },
                    { "@action", action },
                    { "@borrowDate", borrowDate },
                    { "@returndate", returnDate },
                    { "@dueDate", dueDate },
                    { "@userId", userId },
                    { "@date", date }
                };
                // Call the stored procedure to track books
                DataTable trackingTable = await _databaseService.RunStoredProcedureAsync("prc_TrackBook", parameters);
                if (trackingTable.Rows.Count > 0)
                {
                    var bookLastUpdatedBy = await (from ui in _context.UserInfos
                                                   where ui.UserId.ToString() == trackingTable.Rows[0]["BookLastUpdatedByUserId"].ToString()
                                                   select ui.DisplayName).FirstOrDefaultAsync();
                    var bookCreatedBy = await (from ui in _context.UserInfos
                                               where ui.UserId.ToString() == trackingTable.Rows[0]["BookCreatedByUserId"].ToString()
                                               select ui.DisplayName).FirstOrDefaultAsync();
                    var memberLastUpdatedBy = await (from ui in _context.UserInfos
                                                     where ui.UserId.ToString() == trackingTable.Rows[0]["MemberLastUpdatedByUserId"].ToString()
                                                     select ui.DisplayName).FirstOrDefaultAsync();
                    var memberCreatedBy = await (from ui in _context.UserInfos
                                                 where ui.UserId.ToString() == trackingTable.Rows[0]["MemberCreatedByUserId"].ToString()
                                                 select ui.DisplayName).FirstOrDefaultAsync();
                    var trackingCreatedBy = await (from ui in _context.UserInfos
                                                   where ui.UserId.ToString() == trackingTable.Rows[0]["TrackingCreatedByUserId"].ToString()
                                                   select ui.DisplayName).FirstOrDefaultAsync();
                    var status = trackingTable.Rows[0]["BookStatus"].ToString();
                    string statusDescription = string.Empty;
                    switch (status)
                    {
                        case "0":
                            statusDescription = "lost";
                            break;
                        case "1":
                            statusDescription = "good";
                            break;
                        case "-1":
                            statusDescription = "damaged";
                            break;
                    }
                    var member = new MemberObject
                    {
                        MemberId = Guid.Parse(trackingTable.Rows[0]["MemberId"].ToString()),
                        FullName = trackingTable.Rows[0]["Fullname"].ToString(),
                        Email = trackingTable.Rows[0]["Email"].ToString(),
                        Phone = trackingTable.Rows[0]["Phone"].ToString(),
                        CreatedBy = memberCreatedBy,
                        CreatedDateUTC = Convert.ToDateTime(trackingTable.Rows[0]["MemberCreatedDateUTC"]),
                        LastUpdatedBy = memberLastUpdatedBy,
                        LastUpdatedDateUTC = Convert.ToDateTime(trackingTable.Rows[0]["MemberLastUpdatedDateUTC"]),
                    };
                    var book = new BookObject
                    {
                        BookId = Guid.Parse(trackingTable.Rows[0]["BookId"].ToString()),
                        Title = trackingTable.Rows[0]["Title"].ToString(),
                        Genre = trackingTable.Rows[0]["Genre"].ToString(),
                        Author = trackingTable.Rows[0]["Author"].ToString(),
                        Synopsis = trackingTable.Rows[0]["Synopsis"].ToString(),
                        Publisher = trackingTable.Rows[0]["Publisher"].ToString(),
                        PublicationDate = DateOnly.FromDateTime(Convert.ToDateTime(trackingTable.Rows[0]["PublicationDate"].ToString())),
                        Edition = trackingTable.Rows[0]["Edition"].ToString(),
                        NumberOfPages = Convert.ToInt32(trackingTable.Rows[0]["NumberOfPages"]),
                        Language = trackingTable.Rows[0]["Language"].ToString(),
                        Isbn = trackingTable.Rows[0]["Isbn"].ToString(),
                        Status = statusDescription,
                        CreatedBy = bookCreatedBy,
                        CreatedDateUTC = Convert.ToDateTime(trackingTable.Rows[0]["BookCreatedDateUTC"]),
                        LastUpdatedBy = bookLastUpdatedBy,
                        LastUpdatedDateUTC = Convert.ToDateTime(trackingTable.Rows[0]["BookLastUpdatedUTC"]),
                    };

                    borrowDate = trackingTable.Rows[0]["BorrowDate"] == DBNull.Value
                        ? (DateOnly?)null
                        : DateOnly.FromDateTime(Convert.ToDateTime(trackingTable.Rows[0]["BorrowDate"]));

                   

                    var bookReturnDate = trackingTable.Rows[0]["ReturnDate"] == DBNull.Value
                                           ? (DateOnly?)null
                                           : DateOnly.FromDateTime(Convert.ToDateTime(trackingTable.Rows[0]["ReturnDate"]));

                    return Ok(new
                    {
                        trackingId = Guid.Parse(trackingTable.Rows[0]["TrackingId"].ToString()),
                        bookDetails = book,
                        memberDetails = member,
                        borrowDate = DateOnly.FromDateTime(Convert.ToDateTime(trackingTable.Rows[0]["BorrowDate"])),
                        dueDate = DateOnly.FromDateTime(Convert.ToDateTime(trackingTable.Rows[0]["DueDate"])),
                        returnDate = bookReturnDate,
                        status = trackingTable.Rows[0]["TrackingStatus"].ToString(),
                        createdBy = trackingCreatedBy,
                        createdDateUTC = Convert.ToDateTime(trackingTable.Rows[0]["TrackingCreatedDateUTC"]),
                        lastUpdatedDateUTC = Convert.ToDateTime(trackingTable.Rows[0]["TrackingLastUpdatedDateUTC"]),
                    });
                }
                else
                {
                    return _databaseService.CreateErrorResponse(404, "Not Found", "No books can be found");
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

        public class ReserveBookRequest
        {
            public Guid BookId { get; set; }
            public Guid MemberId { get; set; }
        }

        [Authorize]
        [HttpPost("reservebook")]
        public async Task<IActionResult> ReserveBook(ReserveBookRequest reserveBookRequest)
        {
            try
            {
                // Checking for empty fields
                if (reserveBookRequest == null || reserveBookRequest.BookId == Guid.Empty || reserveBookRequest.MemberId == Guid.Empty)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "BookId and MemberId is required.");
                }
                Guid reserveId = Guid.NewGuid();
                var dateCreated = DateTime.UtcNow;
                var userId = User.FindFirst("uid")?.Value;

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@bookId" ,reserveBookRequest.BookId},
                    { "@memberId", reserveBookRequest.MemberId},
                    { "@reserveId", reserveId },
                    { "@dateCreated", dateCreated },
                    { "@userId", userId}
                };

                // Call the stored procedure to reserve book
                await _databaseService.RunStoredProcedureAsync("prc_ReserveBook", parameters);
                return Ok(new { reserveId = reserveId });
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

        public class PaymentRequest
        {
            public Guid TrackingId { get; set; }
            public int DayOfOverdue { get; set; }
        }

        [Authorize]
        [HttpPost("payment")]
        public async Task<IActionResult> Payment(PaymentRequest paymentRequest)
        {
            try
            {
                if (paymentRequest == null || paymentRequest.TrackingId == Guid.Empty)
                {
                    return _databaseService.CreateErrorResponse(400, "Bad Request", "TrackingId and Days of Overdue is required.");
                }
                var paymentId = Guid.NewGuid();
                var userId = User.FindFirst("uid")?.Value;
                var dateCreated = DateTime.UtcNow;
                var totalAmountinRM = "RM" + paymentRequest.DayOfOverdue.ToString();

                // Set up parameters for the stored procedure
                var parameters = new Dictionary<string, object>
                {
                    { "@paymentId",paymentId },
                    { "@trackingId", paymentRequest.TrackingId },
                    { "@dayOfOverdue",paymentRequest.DayOfOverdue },
                    { "@totalAmount",totalAmountinRM },
                    { "@userId",userId },
                    { "@dateCreated", dateCreated }
                };
                // Call the stored procedure to insert payment
                await _databaseService.RunStoredProcedureAsync("prc_ChargePayment", parameters);
                return Ok(new
                {
                    paymentId = paymentId,
                    totalAmountInCents = paymentRequest.DayOfOverdue * 100
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
    }
}
