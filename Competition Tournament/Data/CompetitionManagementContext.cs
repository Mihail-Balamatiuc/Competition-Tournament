using System;
using System.Collections.Generic;
using Competition_Tournament.Models;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Data;

public partial class CompetitionManagementContext : DbContext
{
    public CompetitionManagementContext()
    {
    }

    public CompetitionManagementContext(DbContextOptions<CompetitionManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<CompetitionType> CompetitionTypes { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-8VVD6UT\\SQLEXPRESS; Initial catalog=Competition Management; trusted_connection=yes; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Competit__3214EC0759754C96");

            entity.HasOne(d => d.CompetitionTypeNavigation).WithMany(p => p.Competitions).HasConstraintName("FK__Competiti__Compe__403A8C7D");

            entity.HasMany(d => d.Teams).WithMany(p => p.Competitions)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamCompetition",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Team_Comp__Team___5DCAEF64"),
                    l => l.HasOne<Competition>().WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Team_Comp__Compe__5CD6CB2B"),
                    j =>
                    {
                        j.HasKey("CompetitionId", "TeamId").HasName("PK__Team_Com__37F107D5B1BAF092");
                        j.ToTable("Team_Competition");
                        j.IndexerProperty<int>("CompetitionId").HasColumnName("Competition_Id");
                        j.IndexerProperty<int>("TeamId").HasColumnName("Team_Id");
                    });
        });

        modelBuilder.Entity<CompetitionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Type__3214EC0794078C77");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Game__3214EC07B4BABFF4");

            entity.HasOne(d => d.Competition).WithMany(p => p.Games).HasConstraintName("FK__Game__Competitio__47DBAE45");

            entity.HasOne(d => d.Team1).WithMany(p => p.GameTeam1s).HasConstraintName("FK__Game__Team1_Id__45F365D3");

            entity.HasOne(d => d.Team2).WithMany(p => p.GameTeam2s).HasConstraintName("FK__Game__Team2_Id__46E78A0C");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Player__3214EC07B25BF916");

            entity.HasOne(d => d.Team).WithMany(p => p.Players).HasConstraintName("FK__Player__Team_Id__3D5E1FD2");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Team__3214EC0733820F55");

            entity.ToTable("Team", tb => tb.HasTrigger("TR_TEAM_DELETE_GAME"));
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
