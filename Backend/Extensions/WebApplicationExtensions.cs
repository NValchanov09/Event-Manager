using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using EventManagerBackend.Models;
using EventManagerBackend.Models.DTOs;
using EventManagerBackend.Seeders;
using System.Security.Claims;
using System.Text;

namespace EventManagerBackend.Extensions
{
    public static class WebApplicationExtensions
    {
        //Swagger configuration
        public static void ConfigureSwagger(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", " Event API V1");
                options.InjectStylesheet("/css/swagger-darkTheme.css"); //setting dark theme for the swagger UI
                options.DocumentTitle = "Event API UI";
                options.RoutePrefix = "docs"; // Swagger UI at https://localhost:<port>/docs
            });
        }
        public static async Task ConfigureDemoSeederAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            await RolesSeeder.SeedAsync(roleManager);
            await AdministratorSeeder.SeedAsync(userManager, roleManager);

            var dbSeeder = new DataSeeder(dbContext);
            await dbSeeder.SeedAsync();
        }


        //Admin Seeder
        public static async Task ConfigureSeederAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            // Seed roles (static)
            await RolesSeeder.SeedAsync(roleManager);

            // Seed admin user (static)
            await AdministratorSeeder.SeedAsync(userManager, roleManager);
        }

        // Map Identity API endpoints
        public static void MapEventEndpoints(this WebApplication app)
        {
            ////EVENT ENDPOINTS

            // Get events with filters (optional parameters)
            app.MapGet("/events",
            [Authorize]
            (
                IEventService service,
                HttpContext http,
                ClaimsPrincipal user,
                DateTime? fromDate,
                DateTime? toDate,
                bool? activeOnly,
                bool alphabetical = false,
                bool sortDescending = false
            ) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");

                return service.GetEvents(fromDate, toDate, activeOnly, userId, alphabetical, sortDescending);
            })
            .WithSummary("Search for events")
            .WithDescription("Fetches events with optional filters: date range, activity status, and sorting options.");

            //Get users events
            app.MapGet("/events/joined",
            [Authorize]
            (
                IEventService service,
                HttpContext http,
                ClaimsPrincipal user,
                DateTime? fromDate,
                DateTime? toDate,
                bool? activeOnly,
                bool alphabetical = false,
                bool sortDescending = false
            ) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();
                
                var events = service.GetJoinedEvents(fromDate, toDate, activeOnly, userId, alphabetical, sortDescending);
                return Results.Ok(events);
            })
            .WithSummary("Get joined events for current user")
            .WithDescription("Returns a list of events the currently authenticated user has joined.");

            // Get event by ID
            app.MapGet("/events/{id}",
            [Authorize]
            (IEventService service, int id, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var ev = service.GetEventById(id, userId);

                if(ev == null)
                    return Results.BadRequest(new { error = "Събитието не беше намерено." });

                return Results.Ok(ev);
            })
                .WithSummary("Get event by ID")
                .WithDescription("Fetches an event by its unique identifier (ID).");

            //Create new event
            app.MapPost("/events",
            [Authorize(Roles = "Administrator")]
            (IEventService service, CreateEventDto dto) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Results.BadRequest(new { error = "Името на събитието е задължително." });

                var newEvent = EventMapper.ToEntity(dto);

                var success = service.Create(newEvent);

                return success
                    ? Results.Created($"/events/{newEvent.Id}", new { newEvent.Id })
                    : Results.BadRequest(new { error = "Неуспешно създаване на събитие."});
            })
            .WithSummary("Create a new event")
            .WithDescription("Creates a new event with the provided details. The server sets CreatedAt and UpdatedAt.");


            // Update event by ID (primitive params)

            app.MapPut("/events/{id}",
            [Authorize(Roles = "Administrator")]
            async (IEventService service, int id, UpdateEventDto dto) =>
            {
                var success = await service.Update(id, dto);
                return success ? Results.Ok() : Results.BadRequest();
            })
            .WithSummary("Update event by ID")
            .WithDescription("Updates an existing event using its ID and provided details. Returns 404 if not found.");

            // Delete event by ID
            app.MapDelete("/events/{id}",
            [Authorize(Roles = "Administrator")]
            async (IEventService service, int id) =>
            {
                var success = await service.RemoveById(id);
                return success ? Results.Ok() : Results.BadRequest();
            });

            //// SUBMISSION ENDPOINTS


            // GET submissions by eventId
            app.MapGet("/submissions/{eventId}",
            [Authorize]
            (ISubmissionService service, int eventId) =>
            {
                var submissions = service.GetSubmissionsByEventId(eventId);
                return Results.Ok(submissions);
            })
            .WithSummary("Get all submissions for event")
            .WithDescription("Get all submissions for event. Returns empty if the submissions do not exist.");

            // GET submissions for current user by eventId
            app.MapGet("/submissions/{eventId}/me",
            [Authorize]
            (ISubmissionService service, int eventId, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var submissions = service.GetSubmissionByEventAndUser(eventId, userId);
                return submissions != null ? Results.Ok(submissions) : Results.BadRequest();
            })
            .WithSummary("Fetch current authenticated user for event")
            .WithDescription("Fetches a submission for the authenticated user by event ID.");

            // Create new submission for authenticated user
            app.MapPost("/submissions/{eventId}",
            [Authorize]
            (int eventId, CreateSubmissionDto dto, ISubmissionService service, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var created = service.Create(eventId, userId, dto);
                return created;
            })
            .WithSummary("Create new submission for authenticated user")
            .WithDescription("Creates a new submission for the authenticated user. ");

            // PUT endpoint
            app.MapPut("/submissions/{eventId}",
            [Authorize]
            (int eventId, UpdateSubmissionDto dto, ISubmissionService service, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var updated = service.UpdateSubmission(eventId, userId, dto);
                return updated;
            })
            .WithSummary("Update submission for authenticated user")
            .WithDescription("Updates the submission of the authenticated user for the specified event.");

            //Removes a submission user from an event
           app.MapDelete("/submissions/{eventId}",
           [Authorize]
<<<<<<< Updated upstream
           async (int eventId, ISubmissionService service, ClaimsPrincipal user, IEmailSender emailSender) =>
=======
           async (int eventId, ISubmitService service, ClaimsPrincipal user) =>
>>>>>>> Stashed changes
           {
               var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");

               var success = await service.RemoveUserFromEvent(eventId, userId);
               return success;
            })
            .WithSummary("Removes authenticated user from event")
            .WithDescription("Allows user to remove himself from an event and notifies the user by email.");


            //Admin delete
            app.MapDelete("/submissions/{eventId}/{userId}",
            [Authorize(Roles = "Administrator")]
<<<<<<< Updated upstream
            async (int eventId, string userId, ISubmissionService service, ClaimsPrincipal user, IEmailSender emailSender) =>
            {
                var success = await service.AdminRemoveUserFromEvent(eventId, userId, emailSender);
=======
            async (int eventId, string userId, ISubmitService service, ClaimsPrincipal user) =>
            {

                var success = await service.AdminRemoveUserFromEvent(eventId, userId);
>>>>>>> Stashed changes
                return success ? Results.Ok() : Results.NotFound();
            })
            .WithSummary("Remove user submission from event by admin")
            .WithDescription("Allows an admin to remove a user's submission from a specific event.");

            //enpoint to get all users
            app.MapGet("/users",
            [Authorize(Roles = "Administrator")]
            async (UserManager<User> manager) =>
            {
                try
                {
                    var users = await manager.Users.ToListAsync();

                    var userInfos = new List<object>();

                    foreach (var user in users)
                    {
                        var roles = await manager.GetRolesAsync(user);

                        userInfos.Add(new
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Roles = roles,
                            CreatedAT = user.CreatedAt,
                            UpdatedAT = user.UpdatedAt
                        });
                    }

                    return Results.Ok(userInfos);
                }
                catch (Exception ex)
                {
                    return Results.Problem("Error message: " + ex);
                }
            }).WithSummary("Gets a user list.")
            .WithDescription("Returns the id, Email, Created at date and Updated At date for every user.");

            // User me
            app.MapGet("/users/me",
            [Authorize]
            async
            (HttpContext httpContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager) =>
            {
                var principal = httpContext.User;

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return Results.Unauthorized();

                // Set times only if set to null
                if (user.CreatedAt == null)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }

                if (user.UpdatedAt == null)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                }

                // Save changes to DB
                await userManager.UpdateAsync(user);

                var roles = await userManager.GetRolesAsync(user);

                // If the user has no roles, assign "User"
                if (roles.Count == 0)
                {
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    var result = await userManager.AddToRoleAsync(user, "User");
                    if (!result.Succeeded)
                    {
                        return Results.InternalServerError(new { error = "Поставянето на роля по подразбиране се провали." });
                    }

                    roles = await userManager.GetRolesAsync(user); // Re-fetch roles
                }

                return Results.Ok(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                });
            }).WithSummary("Gets information for the current user")
            .WithDescription("Gives UserID, Email, Role, Created At date and Updated At date. If the user doesn't have a role, it assigns the role \"User\".");

            //Administrator promotes user to administrator
            app.MapPost("/users/admin/{id}",
            [Authorize(Roles = "Administrator")]
            async (UserManager<User> userManager, string id) =>
            {
                try
                {
                    var users = await userManager.Users.ToListAsync();

                    var user_to_admin = users.FirstOrDefault(users => users.Id.Equals(id));

                    if (user_to_admin == null)
                    {
                        return Results.NotFound(new { error = "Потребителят не е намерен"});
                    }

                    await userManager.RemoveFromRoleAsync(user_to_admin, "User"); // Remove default role if exists
					          var result = await userManager.AddToRoleAsync(user_to_admin, "Administrator");
                    if (result.Succeeded)
                    {
                        return Results.Ok(new { error = "Потребителят вече е администратор." });
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user_to_admin, "User"); // Ensure the user has a default role
                        return Results.BadRequest(new { error = "Потребителят не беше направен администратор." });
                    }
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.ToString());
                }
            }).WithSummary("Admin adds new admins.")
            .WithDescription("Only Amins can add new admins as it selects them by ID.");

            //Administrator demotes administrator to user
            app.MapDelete("/users/admin/{id}",
            [Authorize(Roles = "Administrator")]
            async (UserManager<User> manager, string id) =>
            {
                var users = await manager.Users.ToListAsync();

                var user_to_remove = users.FirstOrDefault(users => users.Id.Equals(id));

                if (user_to_remove == null)
                {
                    return Results.NotFound(new { error = "Потребителят не е намерен" });
                }

                const string seededAdminEmail = "admin@example.com";

                if (user_to_remove.Email == seededAdminEmail)
                {
                    return Results.BadRequest(new { error = "Не можете да премахнете началният администратор." });
                }

                var result = await manager.RemoveFromRoleAsync(user_to_remove!, "Administrator");

                        if (result.Succeeded)
                        {
                            await manager.AddToRoleAsync(user_to_remove!, "User"); // Ensure the user has a default role
                            return Results.Ok(new { error = "Потребителя вече е админ."});
                        }
                        else
                        {
                            await manager.AddToRoleAsync(user_to_remove!, "Administrator"); // Ensure the user has his role back
                  return Results.BadRequest(new { error = "Действието се провали!"});
                }
            }).WithSummary("Removes admin.")
            .WithDescription("Only admins remove other admins which are selected by ID as once the admin role is removed the user gets the role 'User'.");

            // Export submissions of a certain event as a .csv file
            app.MapGet("/csv/{eventId}",
            [Authorize(Roles = "Administrator")]
            (HttpContext httpContext,
            int eventId,
            ISubmissionService submissionService,
            IEventService eventService) =>
            {
                if (!eventService.Exists(eventId))
                {
                    return Results.NotFound(new { error = "Събитието не беше намерено." });
                }

                var data = submissionService.GetSubmissionsByEventId(eventId);

                // Get all unique submission names (header columns)
                var fieldsNames = data
                    .SelectMany(d => d.Answers ?? [])
                    .Select(a => a.Name ?? "")
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                var csvBuilder = new StringBuilder();

                // Header
                csvBuilder.Append("Email,");
                csvBuilder.Append("Date");
                foreach (var name in fieldsNames)
                {
                    csvBuilder.Append($",\"{name.Replace("\"", "\"\"")}\"");
                }
                csvBuilder.AppendLine();

                // Rows
                foreach (var summary in data)
                {
                    var email = summary.Email ?? "";

                    csvBuilder.Append($"{email},");

                    var date = summary.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                    csvBuilder.Append($"\"{date}\"");

                    var submissionsDict = (summary.Answers ?? []).ToDictionary(
                        a => a.Name ?? "",
                        a => a.Options != null ? string.Join(" \n", a.Options.Select(o => o.Replace("\"", "\"\""))) : ""
                    );

                    foreach (var name in fieldsNames)
                    {
                        if (submissionsDict.TryGetValue(name, out var options))
                            csvBuilder.Append($",\"{options}\"");
                        else
                            csvBuilder.Append(","); // empty if nothing
                    }

                    csvBuilder.AppendLine();
                }

                var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
                var csvString = csvBuilder.ToString();
                var csvBytes = utf8WithBom.GetBytes(csvString);

                httpContext.Response.Headers.ContentType = "text/csv; charset=utf-8";
                httpContext.Response.Headers.ContentDisposition = "attachment; filename=\"submissions.csv\"";

                return Results.File(csvBytes, "text/csv; charset=utf-8");

            }).WithSummary("Exports all submissions for a certain event in .csv file.")
            .WithDescription("Exports all submissions for a certain event in .csv file. If the event doesn't exist it return code 404.");

            // Export submissions of an event as a .xlsx file

            app.MapGet("/xlsx/{eventId}",
            [Authorize(Roles = "Administrator")]
            (HttpContext httpContext,
            int eventId,
            ISubmissionService submissionService,
            IEventService eventService) =>
            {
                if(!eventService.Exists(eventId))
                {
                    return Results.NotFound(new { error = "Събитието не беше намерено." });
                }

                var data = submissionService.GetSubmissionsByEventId(eventId);

                // Get all unique fields names (columns)
                var fieldsNames = data
                    .SelectMany(d => d.Answers ?? [])
                    .Select(a => a.Name ?? "")
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Submissions");

                // Header row
                worksheet.Cell(1, 1).Value = "Email";
                worksheet.Cell(1, 2).Value = "Date";
                for (int i = 0; i < fieldsNames.Count; i++)
                {
                    worksheet.Cell(1, i + 3).Value = fieldsNames[i];
                }

                int totalColumns = fieldsNames.Count + 2;
                var headerRange = worksheet.Range(1, 1, 1, totalColumns);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Data rows
                int row = 2;
                foreach (var summary in data)
                {
                    worksheet.Cell(row, 1).Value = summary.Email ?? "";
                    worksheet.Cell(row, 2).Value = summary.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

                    var submissionsDict = (summary.Answers ?? []).ToDictionary(
                    a => a.Name ?? "",
                    a => a.Options != null ? string.Join(Environment.NewLine, a.Options) : "");

                    for (int i = 0; i < fieldsNames.Count; i++)
                    {
                        var name = fieldsNames[i];
                        if (submissionsDict.TryGetValue(name, out var value))
                        {
                            var cell = worksheet.Cell(row, i + 3);
                            cell.Value = value;
                            cell.Style.Alignment.WrapText = true;
                        }
                        else
                        {
                            worksheet.Cell(row, i + 3).Value = "";
                        }
                    }

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                // Apply borders to the whole table
                var usedRange = worksheet.RangeUsed();
                usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                return Results.File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "submissions.xlsx");

            }).WithSummary("Exports all submissions for a certain event in .xlsx file.")
            .WithDescription("Exports all submissions for a certain event in .xlsx file. If the event doesn't exist it return code 404.");
        }
    }
}
