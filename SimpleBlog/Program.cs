using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Services;
using Microsoft.AspNetCore.Identity;
using SimpleBlog.Entities;
using SimpleBlog.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IMessageSender, ConsoleMessage>();
builder.Services.AddDbContext<SimpleBlogDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Add SignalR services
builder.Services.AddSignalR();
var app = builder.Build();

//Seeding Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>(); // هنجيب الـ logger factory
    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // استدعاء الـ Method اللي بتعمل الـ Seeding (هنعملها تحت)
        await SeedRolesAndAdminUserAsync(userManager, roleManager, loggerFactory);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>(); // نعمل logger خاص بالـ Program class
        logger.LogError(ex, "An error occurred while seeding the database Roles and Admin user.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();
app.MapHub<NotificationHub>("/notificationHub"); // Register the SignalR hub
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
async Task SeedRolesAndAdminUserAsync(UserManager<IdentityUser> userManager,
                                      RoleManager<IdentityRole> roleManager,
                                      ILoggerFactory loggerFactory)
{
    var logger = loggerFactory.CreateLogger("SeedRolesAndAdminUserAsync"); // نعمل logger للـ method دي

    // 1. إنشاء الأدوار لو مش موجودة
    string[] roleNames = { "Admin", "Author" }; // ممكن تسميهم أي حاجة أو تضيف أدوار تانية
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (roleResult.Succeeded)
            {
                logger.LogInformation($"Role '{roleName}' created successfully.");
            }
            else
            {
                logger.LogError($"Error creating role '{roleName}'.");
                foreach (var error in roleResult.Errors)
                {
                    logger.LogError($"- Code: {error.Code}, Description: {error.Description}");
                }
            }
        }
        else
        {
            logger.LogInformation($"Role '{roleName}' already exists.");
        }
    }

    // 2. تعيين مستخدم معين كـ "Admin"
    var adminUserEmail = "youssef@gmail.com"; 

    var adminUser = await userManager.FindByEmailAsync(adminUserEmail);

    if (adminUser != null)
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (addToRoleResult.Succeeded)
            {
                logger.LogInformation($"User '{adminUser.UserName}' added to 'Admin' role successfully.");
            }
            else
            {
                logger.LogError($"Error adding user '{adminUser.UserName}' to 'Admin' role.");
                foreach (var error in addToRoleResult.Errors)
                {
                    logger.LogError($"- Code: {error.Code}, Description: {error.Description}");
                }
            }
        }
        else
        {
            logger.LogInformation($"User '{adminUser.UserName}' is already in 'Admin' role.");
        }
    }
    else
    {
        logger.LogWarning($"Admin user with email '{adminUserEmail}' not found for admin role assignment. " +
                          $"Please ensure this user is registered before running the seeding process, " +
                          $"or create the user and assign the role manually.");
    }
}