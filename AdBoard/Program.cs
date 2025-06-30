using AdBoard.Core;
using AdBoard.Core.Models.Domains;
using AdBoard.Core.Repositories;
using AdBoard.Persistence;
using AdBoard.Persistence.Repositories;
using AdBoard.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Image = AdBoard.Core.Models.Domains.Image;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddScoped<IBrowseRepository, BrowseRepository>();
builder.Services.AddScoped<IAddEditDeleteRepository, AddEditDeleteRepository>();
builder.Services.AddScoped<IBrowseService, BrowseService>();
builder.Services.AddScoped<IAddEditDeleteService, AddEditDeleteService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); ;

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseExceptionHandler("/Error/Exception");

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseStatusCodePagesWithReExecute("/Error/NotFound");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await context.Database.MigrateAsync();
    await SeedDatabase(context, userManager);
}

await app.RunAsync();

static async Task SeedDatabase(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    if (!context.Users.Any() && !context.Ads.Any() && !context.Images.Any())
    {
        ApplicationUser user1 = new()
        {
            Id = "92ECB8FE-4256-4E81-A257-D73406B8004C",
            Name = "Handlarz Mirek",
            Email = "handlarzmirek@tanieauta123.de",
            UserName = "handlarzmirek@tanieauta123.de", // UserName nie może być nullem ani zawierać białych ani diakretycznych znaków.
            EmailConfirmed = true,
            PhoneNumber = "123 123 123",
        };
        await userManager.CreateAsync(user1, "Qwe123!");
        user1 = await userManager.FindByEmailAsync(user1.Email);

        ApplicationUser user2 = new()
        {
            Id = "B772BD2D-83FA-4A2B-AD9D-5157CE96767B",
            Name = "Smerf Ważniak",
            Email = "ważniak@smerf123.pl",
            UserName = "wazniak@smerf123.pl",
            EmailConfirmed = true,
            PhoneNumber = "123 123 123",
        };
        await userManager.CreateAsync(user2, "Qwe123!");
        user2 = await userManager.FindByEmailAsync(user2.Email);

        ApplicationUser user3 = new()
        {
            Id = "DD090E62-F20B-4C38-A5EC-7EBF57A7B639",
            Name = "Fryzjerka Ania",
            Email = "ania@fryzjer123.pl",
            UserName = "ania@fryzjer123.pl",
            EmailConfirmed = true,
            PhoneNumber = "123 123 123",
        };
        await userManager.CreateAsync(user3, "Qwe123!");
        user3 = await userManager.FindByEmailAsync(user3.Email);

        Ad ad1 = new()
        {
            UserId = user1.Id,
            Category = "Sprzedam",
            Title = "Sprzedam Multiplę",
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Description = "Jedyny minus tego auta, to ten na akumulatorze.\n" +
                    "Daję gwarancję do stu dni - studnia jest przy bramie.",
            Value = 12000.00m,
            Unit = "zł"
        };

        Ad ad2 = new()
        {
            UserId = user2.Id,
            Category = "Zatrudnię dorywczo",
            Title = "Zapłacę za głos w wyborach",
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Description = "Dam 10 zł za zagłosowanie na mnie w wyborach prezydenta smerfów.\n" +
                        "Ja, smerf Ważniak jestem mądrym kandydatem, bo zawsze 2 razy powiem, zanim coś pomyślę.\n" +
                        "Rozwiążę problem głodu i bezrobocia - niech głodni zjedzą bezrobotnych.\n" +
                        "Będę walczył o prawa kobiet, prawa zwierząt i prawo Ohma.",
            Value = 10.00m,
            Unit = "zł"
        };

        Ad ad3 = new()
        {
            UserId = user3.Id,
            Category = "Usługi",
            Title = "Strzyżenie włosów przez praktykanta",
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Description = "Zapraszamy do salonu fryzjerskiego w Programowie przy ul. Asynchronicznej 64.",
            Value = 5.00m,
            Unit = "zł"
        };
        context.Ads.AddRange([ad1, ad2, ad3]);
        await context.SaveChangesAsync();

        List<Image> images =
        [
            new() { FileName = "car-1.png", FilePath = "/images/car-1.png", AdId = ad1.Id},
            new() { FileName = "car-2.png", FilePath = "/images/car-2.png", AdId = ad1.Id},
            new() { FileName = "car-3.png", FilePath = "/images/car-3.png", AdId = ad1.Id},
            new() { FileName = "brainy-smurf.png", FilePath = "/images/brainy-smurf.png", AdId = ad2.Id},
            new() { FileName = "hairdresser.png", FilePath = "/images/hairdresser.png", AdId = ad3.Id}
        ];
        context.Images.AddRange(images);
        await context.SaveChangesAsync();
    }
}