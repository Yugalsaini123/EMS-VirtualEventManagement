using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventDetails",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(7857)),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDetails", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "SpeakersDetails",
                columns: table => new
                {
                    SpeakerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpeakerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Organization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakersDetails", x => x.SpeakerId);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    EmailId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(4916)),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.EmailId);
                });

            migrationBuilder.CreateTable(
                name: "SessionInfo",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpeakerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SessionStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionInfo", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_SessionInfo_EventDetails_EventId",
                        column: x => x.EventId,
                        principalTable: "EventDetails",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionInfo_SpeakersDetails_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "SpeakersDetails",
                        principalColumn: "SpeakerId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantEventDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantEmailId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAttended = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RegistrationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantEventDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantEventDetails_EventDetails_EventId",
                        column: x => x.EventId,
                        principalTable: "EventDetails",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantEventDetails_UserInfo_ParticipantEmailId",
                        column: x => x.ParticipantEmailId,
                        principalTable: "UserInfo",
                        principalColumn: "EmailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EventDetails",
                columns: new[] { "EventId", "CreatedDate", "Description", "EventCategory", "EventDate", "EventName", "LastModifiedDate", "Location", "MaxParticipants", "Status" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(3351), "Join us for an exciting summit on AI and Machine Learning innovations. Learn from industry experts about the latest trends and technologies.", "Tech & Innovations", new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "AI & Machine Learning Summit", null, "Virtual", 500, "Active" },
                    { new Guid("11111111-1111-1111-1111-111111111102"), new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(3357), "Discover how digital transformation is revolutionizing the manufacturing industry. Explore IoT, Industry 4.0, and automation technologies.", "Industrial Events", new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Digital Transformation in Manufacturing", null, "Hybrid (In-person + Virtual)", 300, "Active" },
                    { new Guid("11111111-1111-1111-1111-111111111103"), new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(3364), "Enhance your leadership skills with industry experts. Learn strategies for team building, decision making, and organizational management.", "Leadership", new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Leadership Excellence Workshop", null, "Convention Center, New York", 200, "Active" }
                });

            migrationBuilder.InsertData(
                table: "SpeakersDetails",
                columns: new[] { "SpeakerId", "Bio", "CreatedDate", "Designation", "Email", "IsActive", "LinkedInUrl", "Organization", "PhoneNumber", "SpeakerName" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333301"), "Expert in artificial intelligence and machine learning with 10+ years of experience.", new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(8177), "AI Research Lead", "sarah.johnson@example.com", true, "https://linkedin.com/in/sarahjohnson", "Tech Innovations Inc.", "+1-555-0101", "Dr. Sarah Johnson" },
                    { new Guid("33333333-3333-3333-3333-333333333302"), "Specialized in IoT and Industry 4.0 with extensive manufacturing background.", new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(8182), "Manufacturing Engineer", "david.kumar@example.com", true, "https://linkedin.com/in/davidkumar", "Industrial Solutions Ltd.", "+1-555-0102", "Mr. David Kumar" }
                });

            migrationBuilder.InsertData(
                table: "UserInfo",
                columns: new[] { "EmailId", "CreatedAt", "IsActive", "Password", "Role", "UserName" },
                values: new object[] { "admin@upgrad.com", new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(5949), true, "Admin@321", "Admin", "Admin User" });

            migrationBuilder.InsertData(
                table: "SessionInfo",
                columns: new[] { "SessionId", "Capacity", "CreatedDate", "Description", "EventId", "LastModifiedDate", "Location", "SessionEnd", "SessionStart", "SessionTitle", "SessionUrl", "SpeakerId", "Status" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222201"), 100, new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(6494), "An introductory session on artificial intelligence concepts and applications.", new Guid("11111111-1111-1111-1111-111111111101"), null, "Virtual - Zoom", new DateTime(2026, 5, 17, 10, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 17, 9, 0, 0, 0, DateTimeKind.Unspecified), "Introduction to AI", "https://example.com/session1", new Guid("33333333-3333-3333-3333-333333333301"), "Scheduled" },
                    { new Guid("22222222-2222-2222-2222-222222222202"), 80, new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(6501), "Explore Internet of Things applications in modern manufacturing.", new Guid("11111111-1111-1111-1111-111111111102"), null, "Convention Center, New York", new DateTime(2026, 5, 22, 11, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), "IoT in Manufacturing", "https://example.com/session2", new Guid("33333333-3333-3333-3333-333333333302"), "Scheduled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantEventDetails_EventId",
                table: "ParticipantEventDetails",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantEventDetails_ParticipantEmailId_EventId",
                table: "ParticipantEventDetails",
                columns: new[] { "ParticipantEmailId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionInfo_EventId",
                table: "SessionInfo",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionInfo_SpeakerId",
                table: "SessionInfo",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakersDetails_Email",
                table: "SpeakersDetails",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantEventDetails");

            migrationBuilder.DropTable(
                name: "SessionInfo");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "EventDetails");

            migrationBuilder.DropTable(
                name: "SpeakersDetails");
        }
    }
}
