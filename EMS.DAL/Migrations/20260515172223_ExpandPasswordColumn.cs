using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpeakersDetails_Email",
                table: "SpeakersDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "UserInfo",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "UserInfo",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(4916));

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "SpeakersDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Organization",
                table: "SpeakersDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SpeakersDetails",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Designation",
                table: "SpeakersDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SpeakersDetails",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "SpeakersDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "SessionInfo",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SessionInfo",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SessionInfo",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ParticipantEventDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "ParticipantEventDetails",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "EventDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EventDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "EventDetails",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(7857));

            migrationBuilder.UpdateData(
                table: "EventDetails",
                keyColumn: "EventId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "EventDetails",
                keyColumn: "EventId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"),
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Discover how digital transformation is revolutionising the manufacturing industry. Explore IoT, Industry 4.0, and automation technologies." });

            migrationBuilder.UpdateData(
                table: "EventDetails",
                keyColumn: "EventId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"),
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Enhance your leadership skills with industry experts. Learn strategies for team building, decision making, and organisational management." });

            migrationBuilder.UpdateData(
                table: "SessionInfo",
                keyColumn: "SessionId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"),
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "SessionInfo",
                keyColumn: "SessionId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "SpeakersDetails",
                keyColumn: "SpeakerId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"),
                column: "CreatedDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "SpeakersDetails",
                keyColumn: "SpeakerId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333302"),
                columns: new[] { "Bio", "CreatedDate" },
                values: new object[] { "Specialised in IoT and Industry 4.0 with extensive manufacturing background.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "UserInfo",
                keyColumn: "EmailId",
                keyValue: "admin@upgrad.com",
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_SpeakersDetails_Email",
                table: "SpeakersDetails",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpeakersDetails_Email",
                table: "SpeakersDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "UserInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "UserInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(4916),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "SpeakersDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Organization",
                table: "SpeakersDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SpeakersDetails",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Designation",
                table: "SpeakersDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SpeakersDetails",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "SpeakersDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "SessionInfo",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SessionInfo",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "SessionInfo",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ParticipantEventDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "ParticipantEventDetails",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "EventDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EventDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "EventDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(7857),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.UpdateData(
                table: "EventDetails",
                keyColumn: "EventId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                column: "CreatedDate",
                value: new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(3351));

            migrationBuilder.UpdateData(
                table: "EventDetails",
                keyColumn: "EventId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"),
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(3357), "Discover how digital transformation is revolutionizing the manufacturing industry. Explore IoT, Industry 4.0, and automation technologies." });

            migrationBuilder.UpdateData(
                table: "EventDetails",
                keyColumn: "EventId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"),
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(3364), "Enhance your leadership skills with industry experts. Learn strategies for team building, decision making, and organizational management." });

            migrationBuilder.UpdateData(
                table: "SessionInfo",
                keyColumn: "SessionId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"),
                column: "CreatedDate",
                value: new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(6494));

            migrationBuilder.UpdateData(
                table: "SessionInfo",
                keyColumn: "SessionId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "CreatedDate",
                value: new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(6501));

            migrationBuilder.UpdateData(
                table: "SpeakersDetails",
                keyColumn: "SpeakerId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"),
                column: "CreatedDate",
                value: new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(8177));

            migrationBuilder.UpdateData(
                table: "SpeakersDetails",
                keyColumn: "SpeakerId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333302"),
                columns: new[] { "Bio", "CreatedDate" },
                values: new object[] { "Specialized in IoT and Industry 4.0 with extensive manufacturing background.", new DateTime(2026, 5, 15, 8, 13, 53, 501, DateTimeKind.Utc).AddTicks(8182) });

            migrationBuilder.UpdateData(
                table: "UserInfo",
                keyColumn: "EmailId",
                keyValue: "admin@upgrad.com",
                column: "CreatedAt",
                value: new DateTime(2026, 5, 15, 8, 13, 53, 499, DateTimeKind.Utc).AddTicks(5949));

            migrationBuilder.CreateIndex(
                name: "IX_SpeakersDetails_Email",
                table: "SpeakersDetails",
                column: "Email",
                unique: true);
        }
    }
}
