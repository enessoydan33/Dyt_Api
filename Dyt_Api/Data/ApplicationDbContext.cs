using Dyt_Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dyt_Api.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<NutritionPlan> NutritionPlans { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealFood> MealFoods { get; set; }
        public DbSet<CustomFood> CustomFoods { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>()
                .HasOne(client => client.AssignedDietitian)
                .WithMany(user => user.AssignedClients)
                .HasForeignKey(client => client.AssignedDietitianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NutritionPlan>()
                .HasOne(plan => plan.CreatedByUser)
                .WithMany(user => user.NutritionPlans)
                .HasForeignKey(plan => plan.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NutritionPlan>()
                .HasOne(plan => plan.Client)
                .WithMany(client => client.NutritionPlans)
                .HasForeignKey(plan => plan.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Measurement>()
                .HasOne(measurement => measurement.Client)
                .WithMany(client => client.Measurements)
                .HasForeignKey(measurement => measurement.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Meal>()
                .HasOne(meal => meal.NutritionPlan)
                .WithMany(plan => plan.Meals)
                .HasForeignKey(meal => meal.NutritionPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MealFood>()
                .HasOne(mealFood => mealFood.Meal)
                .WithMany(meal => meal.MealFoods)
                .HasForeignKey(mealFood => mealFood.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CustomFood>()
                .HasOne(customFood => customFood.CreatedByUser)
                .WithMany(user => user.CustomFoods)
                .HasForeignKey(customFood => customFood.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MealFood>()
                .HasOne(mealFood => mealFood.CustomFood)
                .WithMany(customFood => customFood.MealFoods)
                .HasForeignKey(mealFood => mealFood.CustomFoodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(appointment => appointment.Dietitian)
                .WithMany(user => user.Appointments)
                .HasForeignKey(appointment => appointment.DietitianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(appointment => appointment.Client)
                .WithMany(client => client.Appointments)
                .HasForeignKey(appointment => appointment.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
