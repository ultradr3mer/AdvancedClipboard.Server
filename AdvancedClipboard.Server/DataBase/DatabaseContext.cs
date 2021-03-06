﻿using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AdvancedClipboard.Server.Database
{
  public class DatabaseContext : DbContext
  {
    #region Fields

    private readonly DbConnection connection;

    #endregion Fields

    #region Constructors

    public DatabaseContext(DbConnection connection)
    {
      this.connection = connection;
    }

    #endregion Constructors

    #region Properties

    public DbSet<ClipboardContentEntity> ClipboardContent { get; set; }
    public DbSet<ContentTypeEntity> ContentType { get; set; }
    public DbSet<FileAccessTokenEntity> FileAccessToken { get; set; }
    public DbSet<UserEntity> ClipboardUser { get; set; }
    public DbSet<LaneEntity> Lane { get; set; }

    #endregion Properties

    #region Methods

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(this.connection);
    }

    #endregion Methods
  }
}