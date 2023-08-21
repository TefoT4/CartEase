using CartEase.Models;
using Microsoft.EntityFrameworkCore;

namespace CartEase.Infrastructure;

public class CartEaseContext : DbContext
{
    public CartEaseContext(DbContextOptions<CartEaseContext> options) : base(options)
    { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<CartItemModel> CartItems { get; set; }
    public DbSet<ItemImageModel> ItemImages { get; set; }
}