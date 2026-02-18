using Microsoft.EntityFrameworkCore;

namespace SliceCloud.Repository.Models;

public partial class SliceCloudContext : DbContext
{
    public SliceCloudContext(DbContextOptions<SliceCloudContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerReview> CustomerReviews { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemModifierGroupMap> ItemModifierGroupMaps { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<ModifierGroup> ModifierGroups { get; set; }

    public virtual DbSet<ModifierGroupModifierMapping> ModifierGroupModifierMappings { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderTable> OrderTables { get; set; }

    public virtual DbSet<OrderTaxMapping> OrderTaxMappings { get; set; }

    public virtual DbSet<OrderedItem> OrderedItems { get; set; }

    public virtual DbSet<OrderedItemModifier> OrderedItemModifiers { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionModule> PermissionModules { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<Taxis> Taxes { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersLogin> UsersLogins { get; set; }

    public virtual DbSet<WaitingTableMapping> WaitingTableMappings { get; set; }

    public virtual DbSet<WaitingToken> WaitingTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryName, "categories_category_name_key").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(30)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .HasColumnName("city_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cities_state_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CountryName)
                .HasMaxLength(150)
                .HasColumnName("country_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PhoneCode).HasColumnName("phone_code");
            entity.Property(e => e.ShortName)
                .HasMaxLength(5)
                .HasColumnName("short_name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(30)
                .HasColumnName("customer_name");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Dining'::character varying")
                .HasColumnName("order_type");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(15)
                .HasColumnName("phone_no");
            entity.Property(e => e.TotalOrder)
                .HasDefaultValue(0)
                .HasColumnName("total_order");
            entity.Property(e => e.VisitCount)
                .HasDefaultValue((short)0)
                .HasColumnName("visit_count");
        });

        modelBuilder.Entity<CustomerReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("customer_reviews_pkey");

            entity.ToTable("customer_reviews");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.AmbienceRating)
                .HasPrecision(1)
                .HasColumnName("ambience_rating");
            entity.Property(e => e.AverageRating).HasColumnName("average_rating");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.FoodRating)
                .HasPrecision(1)
                .HasColumnName("food_rating");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ServiceRating)
                .HasPrecision(1)
                .HasColumnName("service_rating");

            entity.HasOne(d => d.Order).WithMany(p => p.CustomerReviews)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_reviews_order_id_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("invoice_pkey");

            entity.ToTable("invoice");

            entity.HasIndex(e => e.InvoiceNumber, "invoice_invoice_number_key").IsUnique();

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("invoice_date");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(50)
                .HasColumnName("invoice_number");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoice_order_id_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("items_pkey");

            entity.ToTable("items");

            entity.HasIndex(e => e.ItemName, "items_item_name_key").IsUnique();

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(false)
                .HasColumnName("is_available");
            entity.Property(e => e.IsDefaultTax)
                .HasDefaultValue(false)
                .HasColumnName("is_default_tax");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsFavorite)
                .HasDefaultValue(false)
                .HasColumnName("is_favorite");
            entity.Property(e => e.ItemImage).HasColumnName("item_image");
            entity.Property(e => e.ItemName).HasColumnName("item_name");
            entity.Property(e => e.ItemType)
                .HasMaxLength(10)
                .HasDefaultValueSql("'Veg'::character varying")
                .HasColumnName("item_type");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.ShortCode)
                .HasMaxLength(30)
                .HasColumnName("short_code");
            entity.Property(e => e.TaxPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("tax_percentage");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_category_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Items)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("items_unit_id_fkey");
        });

        modelBuilder.Entity<ItemModifierGroupMap>(entity =>
        {
            entity.HasKey(e => e.ItemModifierGroupId).HasName("item_modifier_group_map_pkey");

            entity.ToTable("item_modifier_group_map");

            entity.Property(e => e.ItemModifierGroupId).HasColumnName("item_modifier_group_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.MaxSelectionAllowed).HasColumnName("max_selection_allowed");
            entity.Property(e => e.MinSelectionRequired)
                .HasDefaultValue((short)0)
                .HasColumnName("min_selection_required");
            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemModifierGroupMaps)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifier_group_map_item_id_fkey");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.ItemModifierGroupMaps)
                .HasForeignKey(d => d.ModifierGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifier_group_map_modifier_group_id_fkey");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.ModifierId).HasName("modifiers_pkey");

            entity.ToTable("modifiers");

            entity.HasIndex(e => e.ModifierName, "modifiers_modifier_name_key").IsUnique();

            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierName).HasColumnName("modifier_name");
            entity.Property(e => e.ModifierType).HasColumnName("modifier_type");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.Unit).WithMany(p => p.Modifiers)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiers_unit_id_fkey");
        });

        modelBuilder.Entity<ModifierGroup>(entity =>
        {
            entity.HasKey(e => e.ModifierGroupId).HasName("modifier_groups_pkey");

            entity.ToTable("modifier_groups");

            entity.HasIndex(e => e.ModifierGroupName, "modifier_groups_modifier_group_name_key").IsUnique();

            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierGroupName)
                .HasMaxLength(30)
                .HasColumnName("modifier_group_name");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<ModifierGroupModifierMapping>(entity =>
        {
            entity.HasKey(e => e.ModifierGroupModifierMappingId).HasName("modifier_group_modifier_mappings_pkey");

            entity.ToTable("modifier_group_modifier_mappings");

            entity.Property(e => e.ModifierGroupModifierMappingId)
                .HasDefaultValueSql("nextval('modifier_group_modifier_mappi_modifier_group_modifier_mappi_seq'::regclass)")
                .HasColumnName("modifier_group_modifier_mapping_id");
            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.ModifierGroupModifierMappings)
                .HasForeignKey(d => d.ModifierGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifier_group_modifier_mappings_modifier_group_id_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.ModifierGroupModifierMappings)
                .HasForeignKey(d => d.ModifierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifier_group_modifier_mappings_modifier_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Discount)
                .HasPrecision(3)
                .HasColumnName("discount");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(50)
                .HasColumnName("invoice_number");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.NoOfPerson).HasColumnName("no_of_person");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("order_date");
            entity.Property(e => e.OrderType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Dining'::character varying")
                .HasColumnName("order_type");
            entity.Property(e => e.OrderWiseComment).HasColumnName("order_wise_comment");
            entity.Property(e => e.PaymentMode)
                .HasMaxLength(20)
                .HasColumnName("payment_mode");
            entity.Property(e => e.Rating)
                .HasPrecision(2, 1)
                .HasDefaultValueSql("0")
                .HasColumnName("rating");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
            entity.Property(e => e.SubAmount)
                .HasPrecision(10, 2)
                .HasColumnName("sub_amount");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.TotalTax)
                .HasPrecision(10, 2)
                .HasColumnName("total_tax");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_customer_id_fkey");
        });

        modelBuilder.Entity<OrderTable>(entity =>
        {
            entity.HasKey(e => e.OrderTableId).HasName("order_tables_pkey");

            entity.ToTable("order_tables");

            entity.Property(e => e.OrderTableId).HasColumnName("order_table_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTables)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tables_order_id_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.OrderTables)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tables_table_id_fkey");
        });

        modelBuilder.Entity<OrderTaxMapping>(entity =>
        {
            entity.HasKey(e => e.OrderTaxId).HasName("order_tax_mapping_pkey");

            entity.ToTable("order_tax_mapping");

            entity.Property(e => e.OrderTaxId).HasColumnName("order_tax_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TaxId).HasColumnName("tax_id");
            entity.Property(e => e.TaxValue).HasColumnName("tax_value");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_order_id_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.TaxId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_tax_id_fkey");
        });

        modelBuilder.Entity<OrderedItem>(entity =>
        {
            entity.HasKey(e => e.OrderedItemId).HasName("ordered_items_pkey");

            entity.ToTable("ordered_items");

            entity.Property(e => e.OrderedItemId).HasColumnName("ordered_item_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemWiseComment).HasColumnName("item_wise_comment");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.ReadyQuantity)
                .HasDefaultValue(0)
                .HasColumnName("ready_quantity");
            entity.Property(e => e.ServedAt).HasColumnName("served_at");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_items_item_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_items_order_id_fkey");
        });

        modelBuilder.Entity<OrderedItemModifier>(entity =>
        {
            entity.HasKey(e => e.ModifiedItemId).HasName("ordered_item_modifiers_pkey");

            entity.ToTable("ordered_item_modifiers");

            entity.Property(e => e.ModifiedItemId).HasColumnName("modified_item_id");
            entity.Property(e => e.ItemModifierId).HasColumnName("item_modifier_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderedItemId).HasColumnName("ordered_item_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.ItemModifier).WithMany(p => p.OrderedItemModifiers)
                .HasForeignKey(d => d.ItemModifierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_item_modifiers_item_modifier_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedItemModifiers)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_item_modifiers_order_id_fkey");

            entity.HasOne(d => d.OrderedItem).WithMany(p => p.OrderedItemModifiers)
                .HasForeignKey(d => d.OrderedItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_item_modifiers_ordered_item_id_fkey");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("password_reset_tokens_pkey");

            entity.ToTable("password_reset_tokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpiryTime).HasColumnName("expiry_time");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("password_reset_tokens_user_id_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.CanAddEdit)
                .HasDefaultValue(true)
                .HasColumnName("can_add_edit");
            entity.Property(e => e.CanDelete)
                .HasDefaultValue(true)
                .HasColumnName("can_delete");
            entity.Property(e => e.CanView)
                .HasDefaultValue(true)
                .HasColumnName("can_view");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Module).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_module_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_role_id_fkey");
        });

        modelBuilder.Entity<PermissionModule>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("permission_modules_pkey");

            entity.ToTable("permission_modules");

            entity.HasIndex(e => e.ModuleName, "permission_modules_module_name_key").IsUnique();

            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(30)
                .HasColumnName("module_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "roles_role_name_key").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("sections_pkey");

            entity.ToTable("sections");

            entity.HasIndex(e => e.SectionName, "sections_section_name_key").IsUnique();

            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.SectionName)
                .HasMaxLength(30)
                .HasColumnName("section_name");
            entity.Property(e => e.SectionOrder).HasColumnName("section_order");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("states_pkey");

            entity.ToTable("states");

            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.StateName)
                .HasMaxLength(50)
                .HasColumnName("state_name");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("states_country_id_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(20)
                .HasColumnName("table_name");
            entity.Property(e => e.TableStatus)
                .HasDefaultValue(1)
                .HasColumnName("table_status");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.SectionId)
                .HasConstraintName("tables_section_id_fkey");
        });

        modelBuilder.Entity<Taxis>(entity =>
        {
            entity.HasKey(e => e.TaxId).HasName("taxes_pkey");

            entity.ToTable("taxes");

            entity.Property(e => e.TaxId).HasColumnName("tax_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(true)
                .HasColumnName("is_default");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_enabled");
            entity.Property(e => e.IsInclusive)
                .HasDefaultValue(false)
                .HasColumnName("is_inclusive");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.TaxName)
                .HasMaxLength(10)
                .HasColumnName("tax_name");
            entity.Property(e => e.TaxType)
                .HasMaxLength(20)
                .HasColumnName("tax_type");
            entity.Property(e => e.TaxValue).HasColumnName("tax_value");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("units_pkey");

            entity.ToTable("units");

            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.ShortName)
                .HasMaxLength(20)
                .HasColumnName("short_name");
            entity.Property(e => e.UnitName)
                .HasMaxLength(50)
                .HasColumnName("unit_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "users_phone_number_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
            entity.Property(e => e.ProfileImage).HasColumnName("profile_image");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasColumnType("character varying")
                .HasColumnName("user_name");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .HasColumnName("zip_code");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_city_id_fkey");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_country_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_state_id_fkey");
        });

        modelBuilder.Entity<UsersLogin>(entity =>
        {
            entity.HasKey(e => e.UserLoginId).HasName("users_logins_pkey");

            entity.ToTable("users_logins");

            entity.HasIndex(e => e.Email, "users_logins_email_key").IsUnique();

            entity.Property(e => e.UserLoginId).HasColumnName("user_login_id");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.IsFirstLogin)
                .HasDefaultValue(true)
                .HasColumnName("is_first_login");
            entity.Property(e => e.IsResetTokenUsed)
                .HasDefaultValue(false)
                .HasColumnName("is_reset_token_used");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.ResetToken).HasColumnName("reset_token");
            entity.Property(e => e.ResetTokenExpiration).HasColumnName("reset_token_expiration");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UsersLogins)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_logins_role_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UsersLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("users_logins_user_id_fkey");
        });

        modelBuilder.Entity<WaitingTableMapping>(entity =>
        {
            entity.HasKey(e => e.WaitingTableId).HasName("waiting_table_mappings_pkey");

            entity.ToTable("waiting_table_mappings");

            entity.Property(e => e.WaitingTableId).HasColumnName("waiting_table_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.WaitingTokenId).HasColumnName("waiting_token_id");

            entity.HasOne(d => d.Table).WithMany(p => p.WaitingTableMappings)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_table_mappings_table_id_fkey");

            entity.HasOne(d => d.WaitingToken).WithMany(p => p.WaitingTableMappings)
                .HasForeignKey(d => d.WaitingTokenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_table_mappings_waiting_token_id_fkey");
        });

        modelBuilder.Entity<WaitingToken>(entity =>
        {
            entity.HasKey(e => e.WaitingTokenId).HasName("waiting_tokens_pkey");

            entity.ToTable("waiting_tokens");

            entity.Property(e => e.WaitingTokenId).HasColumnName("waiting_token_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsAssigned)
                .HasDefaultValue(false)
                .HasColumnName("is_assigned");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.NoOfPeople).HasColumnName("no_of_people");
            entity.Property(e => e.SectionId).HasColumnName("section_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.WaitingTokens)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_tokens_customer_id_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.WaitingTokens)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_tokens_section_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
