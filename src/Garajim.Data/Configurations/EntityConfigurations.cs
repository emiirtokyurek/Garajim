using Garajim.Core.Entities;
using Garajim.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garajim.Data.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> b)
    {
        b.Property(x => x.Brand).IsRequired().HasMaxLength(50);
        b.Property(x => x.Model).IsRequired().HasMaxLength(50);
        b.Property(x => x.EngineInfo).HasMaxLength(100);
        b.Property(x => x.Plate).IsRequired().HasMaxLength(15);
        b.HasIndex(x => x.Plate).IsUnique();

        // Araç silinmeye çalışılırsa kayıtları varsa veritabanı engeller (veri kaybı koruması).
        b.HasMany(x => x.FuelRecords).WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.MaintenanceRecords).WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Expenses).WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Reminders).WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class FuelRecordConfiguration : IEntityTypeConfiguration<FuelRecord>
{
    public void Configure(EntityTypeBuilder<FuelRecord> b)
    {
        b.Property(x => x.Liters).HasConversion(new QuantityConverter());
        b.Property(x => x.UnitPrice).HasConversion(new QuantityConverter());
        b.Property(x => x.TotalCost).HasConversion(new MoneyConverter());
        b.Property(x => x.Note).HasMaxLength(500);
        b.HasIndex(x => new { x.VehicleId, x.Date, x.Odometer });
    }
}

public sealed class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> b)
    {
        b.Property(x => x.Description).IsRequired().HasMaxLength(300);
        b.Property(x => x.PartsCost).HasConversion(new MoneyConverter());
        b.Property(x => x.LaborCost).HasConversion(new MoneyConverter());
        b.Ignore(x => x.TotalCost);
        b.HasIndex(x => new { x.VehicleId, x.Date });
    }
}

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> b)
    {
        b.Property(x => x.Amount).HasConversion(new MoneyConverter());
        b.Property(x => x.Note).HasMaxLength(500);
        b.HasIndex(x => new { x.VehicleId, x.Date });
    }
}

public sealed class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> b)
    {
        b.Property(x => x.Title).IsRequired().HasMaxLength(150);
        b.HasIndex(x => new { x.VehicleId, x.IsCompleted });
    }
}

public sealed class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> b)
    {
        b.Property(x => x.Theme).IsRequired().HasMaxLength(20);
        b.Property(x => x.Currency).IsRequired().HasMaxLength(5);
        b.Property(x => x.DistanceUnit).IsRequired().HasMaxLength(5);

        // Tek satırlık ayar kaydı baştan hazır gelir.
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        b.HasData(new UserSettings
        {
            Id = 1, Theme = "Dark", Currency = "TRY", DistanceUnit = "km",
            CreatedAt = seedDate, UpdatedAt = seedDate
        });
    }
}
