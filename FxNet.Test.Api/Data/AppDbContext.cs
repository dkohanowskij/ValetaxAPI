using FxNet.Test.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tree> Trees { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<ExceptionJournal> ExceptionJournals { get; set; }
    public DbSet<Partner> Partners { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
                
        modelBuilder.Entity<Tree>(entity =>
        {
            entity.ToTable("trees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });
                
        modelBuilder.Entity<Node>(entity =>
        {
            entity.ToTable("nodes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.TreeId).HasColumnName("tree_id");
            entity.Property(e => e.ParentNodeId).HasColumnName("parent_node_id");

            entity.HasOne(e => e.Tree)
                .WithMany(t => t.Nodes)
                .HasForeignKey(e => e.TreeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentNode)
                .WithMany(n => n.Children)
                .HasForeignKey(e => e.ParentNodeId)
                .OnDelete(DeleteBehavior.Restrict);


            entity.HasIndex(e => new { e.ParentNodeId, e.Name }).IsUnique();
        });
                
        modelBuilder.Entity<ExceptionJournal>(entity =>
        {
            entity.ToTable("exception_journal");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").HasDefaultValueSql("now()");
            entity.Property(e => e.Parameters).HasColumnName("parameters").HasColumnType("jsonb");
            entity.Property(e => e.StackTrace).HasColumnName("stack_trace");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.HasIndex(e => e.EventId).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
        });
               
        modelBuilder.Entity<Partner>(entity =>
        {
            entity.ToTable("partners");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityByDefaultColumn();
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(512).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").HasDefaultValueSql("now()");
            entity.HasIndex(e => e.Code).IsUnique();
        });
    }
}
