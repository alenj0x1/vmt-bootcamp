using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Database.SqlServer.Context;

public partial class TalentInsightsContext : DbContext
{
    public TalentInsightsContext()
    {
    }

    public TalentInsightsContext(DbContextOptions<TalentInsightsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Collaborator> Collaborators { get; set; }

    public virtual DbSet<CollaboratorHistory> CollaboratorHistories { get; set; }

    public virtual DbSet<CollaboratorPermission> CollaboratorPermissions { get; set; }

    public virtual DbSet<CollaboratorSkill> CollaboratorSkills { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuPermission> MenuPermissions { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectCollaborator> ProjectCollaborators { get; set; }

    public virtual DbSet<ProjectMessage> ProjectMessages { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;User=sa;Password=Admin1234@;Database=TalentInsights;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Collaborator>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DeletedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.GitlabProfile).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<CollaboratorHistory>(entity =>
        {
            entity.ToTable("CollaboratorHistory");

            entity.HasIndex(e => e.CollaboratorId, "IX_CollaboratorHistory_Collaborator");

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "IX_CollaboratorHistory_EntityType");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.EntityName).HasMaxLength(200);
            entity.Property(e => e.EntityType).HasMaxLength(20);
            entity.Property(e => e.RecordedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Collaborator).WithMany(p => p.CollaboratorHistories)
                .HasForeignKey(d => d.CollaboratorId)
                .HasConstraintName("FK_CollaboratorHistory_Collaborator");
        });

        modelBuilder.Entity<CollaboratorPermission>(entity =>
        {
            entity.HasKey(e => new { e.CollaboratorId, e.PermissionId });

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.CollaboratorPermissionAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_CollaboratorPermissions_AssignedBy");

            entity.HasOne(d => d.Collaborator).WithMany(p => p.CollaboratorPermissionCollaborators)
                .HasForeignKey(d => d.CollaboratorId)
                .HasConstraintName("FK_CollaboratorPermissions_Collaborator");

            entity.HasOne(d => d.Permission).WithMany(p => p.CollaboratorPermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK_CollaboratorPermissions_Permission");
        });

        modelBuilder.Entity<CollaboratorSkill>(entity =>
        {
            entity.HasKey(e => new { e.CollaboratorId, e.SkillId });

            entity.HasIndex(e => e.SkillId, "IX_CollaboratorSkills_SkillId");

            entity.Property(e => e.AcquiredAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Collaborator).WithMany(p => p.CollaboratorSkills)
                .HasForeignKey(d => d.CollaboratorId)
                .HasConstraintName("FK_CollaboratorSkills_Collaborator");

            entity.HasOne(d => d.Skill).WithMany(p => p.CollaboratorSkills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK_CollaboratorSkills_Skill");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasIndex(e => e.ParentId, "IX_Menus_ParentId");

            entity.HasIndex(e => e.SortOrder, "IX_Menus_SortOrder");

            entity.HasIndex(e => e.Code, "UQ_Menus_Code").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IconName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsVisible).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Path).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Menus_ParentId");
        });

        modelBuilder.Entity<MenuPermission>(entity =>
        {
            entity.HasKey(e => new { e.MenuId, e.PermissionId });

            entity.HasIndex(e => e.PermissionId, "IX_MenuPermissions_PermissionId");

            entity.Property(e => e.MatchMode)
                .HasMaxLength(10)
                .HasDefaultValue("ANY");

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuPermissions)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK_MenuPermissions_Menu");

            entity.HasOne(d => d.Permission).WithMany(p => p.MenuPermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK_MenuPermissions_Permission");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_Permissions_Code").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Module).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Specificity)
                .HasMaxLength(20)
                .HasDefaultValue("ByAssignment");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(e => e.CollaboratorId, "IX_Posts_CollaboratorId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Collaborator).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CollaboratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_Collaborator");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_CreatedBy");

            entity.HasMany(d => d.Teams).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectTeam",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProjectTeams_Team"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK_ProjectTeams_Project"),
                    j =>
                    {
                        j.HasKey("ProjectId", "TeamId");
                        j.ToTable("ProjectTeams");
                    });
        });

        modelBuilder.Entity<ProjectCollaborator>(entity =>
        {
            entity.HasKey(e => new { e.ProjectId, e.CollaboratorId });

            entity.HasIndex(e => e.CollaboratorId, "IX_ProjectCollaborators_Collaborator");

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Collaborator).WithMany(p => p.ProjectCollaborators)
                .HasForeignKey(d => d.CollaboratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectCollaborators_Collaborator");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectCollaborators)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectCollaborators_Project");
        });

        modelBuilder.Entity<ProjectMessage>(entity =>
        {
            entity.HasIndex(e => e.CollaboratorId, "IX_ProjectMessages_CollaboratorId");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectMessages_ProjectId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.SentAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Collaborator).WithMany(p => p.ProjectMessages)
                .HasForeignKey(d => d.CollaboratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMessages_Collaborator");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMessages)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectMessages_Project");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Skills_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Teams)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teams_CreatedBy");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => new { e.TeamId, e.CollaboratorId });

            entity.HasIndex(e => e.CollaboratorId, "IX_TeamMembers_CollaboratorId");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Collaborator).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.CollaboratorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeamMembers_Collaborator");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_TeamMembers_Team");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
