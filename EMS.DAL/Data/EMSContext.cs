// EMS.DAL/Data/EMSContext.cs
using Microsoft.EntityFrameworkCore;
using EMS.DAL.Models;

namespace EMS.DAL.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for Event Management System.
    /// Manages all database operations and entity configurations.
    /// </summary>
    public class EMSContext : DbContext
    {
        public EMSContext(DbContextOptions<EMSContext> options) : base(options) { }

        public DbSet<UserInfo>                 UserInfos                 { get; set; }
        public DbSet<EventDetails>             EventDetails              { get; set; }
        public DbSet<SessionInfo>              SessionInfos              { get; set; }
        public DbSet<SpeakersDetails>          SpeakersDetails           { get; set; }
        public DbSet<ParticipantEventDetails>  ParticipantEventDetails   { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── UserInfo ────────────────────────────────────────────────────────────
            modelBuilder.Entity<UserInfo>().HasKey(u => u.EmailId);

            // FIX: HasDefaultValue(DateTime.UtcNow) bakes a snapshot timestamp into
            //      the migration file. Use a SQL function so the DB always uses the
            //      real current time when a row is inserted without an explicit value.
            modelBuilder.Entity<UserInfo>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Seed – use FIXED dates so EF Core never detects a change and tries
            // to regenerate the migration on every scaffold.
            // modelBuilder.Entity<UserInfo>().HasData(
            //     new UserInfo
            //     {
            //         EmailId   = "admin@upgrad.com",
            //         UserName  = "Admin User",
            //         Role      = "Admin",
            //         Password  = "Admin@321",   // hashed in a real system
            //         IsActive  = true,
            //         CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            //     }
            // );

            // ── EventDetails ────────────────────────────────────────────────────────
            modelBuilder.Entity<EventDetails>().HasKey(e => e.EventId);

            modelBuilder.Entity<EventDetails>()
                .Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");   // FIX: same reason as above

            // One-to-Many: EventDetails → SessionInfo
            modelBuilder.Entity<SessionInfo>()
                .HasOne(s => s.Event)                  // FIX: was "EventDetails" – renamed
                .WithMany(e => e.Sessions)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: EventDetails → ParticipantEventDetails
            modelBuilder.Entity<ParticipantEventDetails>()
                .HasOne(p => p.Event)
                .WithMany(e => e.ParticipantRegistrations)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed – fixed dates
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<EventDetails>().HasData(
                new EventDetails
                {
                    EventId         = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                    EventName       = "AI & Machine Learning Summit",
                    EventCategory   = "Tech & Innovations",
                    EventDate       = new DateTime(2026, 5, 17),
                    Description     = "Join us for an exciting summit on AI and Machine Learning innovations. " +
                                      "Learn from industry experts about the latest trends and technologies.",
                    Status          = "Active",
                    Location        = "Virtual",
                    MaxParticipants = 500,
                    CreatedDate     = seedDate
                },
                new EventDetails
                {
                    EventId         = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                    EventName       = "Digital Transformation in Manufacturing",
                    EventCategory   = "Industrial Events",
                    EventDate       = new DateTime(2026, 5, 22),
                    Description     = "Discover how digital transformation is revolutionising the manufacturing " +
                                      "industry. Explore IoT, Industry 4.0, and automation technologies.",
                    Status          = "Active",
                    Location        = "Hybrid (In-person + Virtual)",
                    MaxParticipants = 300,
                    CreatedDate     = seedDate
                },
                new EventDetails
                {
                    EventId         = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                    EventName       = "Leadership Excellence Workshop",
                    EventCategory   = "Leadership",
                    EventDate       = new DateTime(2026, 6, 5),
                    Description     = "Enhance your leadership skills with industry experts. Learn strategies for " +
                                      "team building, decision making, and organisational management.",
                    Status          = "Active",
                    Location        = "Convention Center, New York",
                    MaxParticipants = 200,
                    CreatedDate     = seedDate
                }
            );

            // ── SessionInfo ─────────────────────────────────────────────────────────
            modelBuilder.Entity<SessionInfo>().HasKey(s => s.SessionId);

            modelBuilder.Entity<SessionInfo>()
                .Property(s => s.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // Many-to-One: SessionInfo → SpeakersDetails (optional speaker)
            modelBuilder.Entity<SessionInfo>()
                .HasOne(s => s.Speaker)
                .WithMany(sp => sp.Sessions)
                .HasForeignKey(s => s.SpeakerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SessionInfo>().HasData(
                new SessionInfo
                {
                    SessionId    = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                    EventId      = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                    SessionTitle = "Introduction to AI",
                    SpeakerId    = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                    Description  = "An introductory session on artificial intelligence concepts and applications.",
                    SessionStart = new DateTime(2026, 5, 17, 9,  0, 0, DateTimeKind.Utc),
                    SessionEnd   = new DateTime(2026, 5, 17, 10, 30, 0, DateTimeKind.Utc),
                    SessionUrl   = "https://example.com/session1",
                    Capacity     = 100,
                    Location     = "Virtual - Zoom",
                    Status       = "Scheduled",
                    CreatedDate  = seedDate
                },
                new SessionInfo
                {
                    SessionId    = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                    EventId      = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                    SessionTitle = "IoT in Manufacturing",
                    SpeakerId    = Guid.Parse("33333333-3333-3333-3333-333333333302"),
                    Description  = "Explore Internet of Things applications in modern manufacturing.",
                    SessionStart = new DateTime(2026, 5, 22, 10, 0,  0, DateTimeKind.Utc),
                    SessionEnd   = new DateTime(2026, 5, 22, 11, 30, 0, DateTimeKind.Utc),
                    SessionUrl   = "https://example.com/session2",
                    Capacity     = 80,
                    Location     = "Convention Center, New York",
                    Status       = "Scheduled",
                    CreatedDate  = seedDate
                }
            );

            // ── SpeakersDetails ─────────────────────────────────────────────────────
            modelBuilder.Entity<SpeakersDetails>().HasKey(s => s.SpeakerId);

            modelBuilder.Entity<SpeakersDetails>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<SpeakersDetails>()
                .Property(s => s.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<SpeakersDetails>().HasData(
                new SpeakersDetails
                {
                    SpeakerId    = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                    SpeakerName  = "Dr. Sarah Johnson",
                    Email        = "sarah.johnson@example.com",
                    Designation  = "AI Research Lead",
                    Organization = "Tech Innovations Inc.",
                    Bio          = "Expert in artificial intelligence and machine learning with 10+ years of experience.",
                    PhoneNumber  = "+1-555-0101",
                    LinkedInUrl  = "https://linkedin.com/in/sarahjohnson",
                    IsActive     = true,
                    CreatedDate  = seedDate
                },
                new SpeakersDetails
                {
                    SpeakerId    = Guid.Parse("33333333-3333-3333-3333-333333333302"),
                    SpeakerName  = "Mr. David Kumar",
                    Email        = "david.kumar@example.com",
                    Designation  = "Manufacturing Engineer",
                    Organization = "Industrial Solutions Ltd.",
                    Bio          = "Specialised in IoT and Industry 4.0 with extensive manufacturing background.",
                    PhoneNumber  = "+1-555-0102",
                    LinkedInUrl  = "https://linkedin.com/in/davidkumar",
                    IsActive     = true,
                    CreatedDate  = seedDate
                }
            );

            // ── ParticipantEventDetails ─────────────────────────────────────────────
            modelBuilder.Entity<ParticipantEventDetails>().HasKey(p => p.Id);

            // Composite unique index: one registration per participant per event
            modelBuilder.Entity<ParticipantEventDetails>()
                .HasIndex(p => new { p.ParticipantEmailId, p.EventId })
                .IsUnique();

            // FK: ParticipantEventDetails → UserInfo
            modelBuilder.Entity<ParticipantEventDetails>()
                .HasOne(p => p.User)
                .WithMany(u => u.ParticipantEvents)
                .HasForeignKey(p => p.ParticipantEmailId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK: ParticipantEventDetails → EventDetails  (already configured above via
            //     the EventDetails side; repeated here for explicitness and clarity)
            modelBuilder.Entity<ParticipantEventDetails>()
                .HasOne(p => p.Event)
                .WithMany(e => e.ParticipantRegistrations)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}