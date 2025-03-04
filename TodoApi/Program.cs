// using Microsoft.EntityFrameworkCore; // לאפשר שימוש ב-DbContext
// using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // ל-UseMySql
// using TodoApi;

// var builder = WebApplication.CreateBuilder(args);


// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), new MySqlServerVersion(new Version(8, 0, 40))));

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder =>
//         {
//             builder.AllowAnyOrigin()
//                    .AllowAnyMethod()
//                    .AllowAnyHeader();
//         });
// });
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Todo API", Version = "v1" });
// });

// var app = builder.Build();

// app.UseCors("AllowAllOrigins");

// app.UseSwagger();

// app.UseSwaggerUI(c => 
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
// });




// app.MapGet("/items", async (ToDoDbContext db) =>

// await db.Item.ToListAsync());


// app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>

// {

// db.Item.Add(newItem);

// await db.SaveChangesAsync();

// return Results.Created($"/items/{newItem.Id}", newItem);

// });


// app.MapPut("/items/{id}", async (Item item,ToDoDbContext db) =>

// {

// var newItem = await db.Item.FindAsync(item.Id);

// if (item is null) return Results.NotFound();


// if (newItem.Id != item.Id)

// return Results.BadRequest("Cannot change the ID.");


// newItem.Name = item.Name;

// newItem.IsComplete = item.IsComplete;


// await db.SaveChangesAsync();


// return Results.Ok(newItem);

// });


// app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>

// {

// var item = await db.Item.FindAsync(id);

// if (item is null) return Results.NotFound();


// db.Item.Remove(item);

// await db.SaveChangesAsync();


// return Results.NoContent();

// });


// // app.MapGet("/",()=>"Yaaaaaaaaaa, it's working");
// app.Run();

using Microsoft.EntityFrameworkCore; // שימוש ב-DbContext
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // ל-UseMySql
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ הגדרת חיבור למסד נתונים
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToDoDB"),
        new MySqlServerVersion(new Version(8, 0, 40))
    )
);

// 2️⃣ הגדרת CORS בצורה נכונה
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Todo API", Version = "v1" });
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 3️⃣ סדר נכון של ה-Middleware
app.UseCors("AllowAllOrigins"); // ✅ חייב להיות לפני Swagger והרשאות
app.UseRouting();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
});

// 4️⃣ הגדרת המסלולים עם טיפול בשגיאות
app.MapGet("/items", async (ToDoDbContext db) =>
{
    try
    {
        return Results.Ok(await db.Item.ToListAsync());
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    try
    {
        db.Item.Add(newItem);
        await db.SaveChangesAsync();
        return Results.Created($"/items/{newItem.Id}", newItem);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapPut("/items/{id}", async (int id, ToDoDbContext db, Item item) =>
{
    try
    {
        var newItem = await db.Item.FindAsync(id);
        if (newItem == null) return Results.NotFound("Item not found");

        newItem.Name = item.Name;
        newItem.IsComplete = item.IsComplete;

        await db.SaveChangesAsync();
        return Results.Ok(newItem);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    try
    {
        var item = await db.Item.FindAsync(id);
        if (item == null) return Results.NotFound("Item not found");

        db.Item.Remove(item);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

// 5️⃣ מסלול לבדיקה שהשרת עובד
app.MapGet("/", () => "Yaaaaaaaaaa, it's working");

app.Run();