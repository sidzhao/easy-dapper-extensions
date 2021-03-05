# EasyDapperExtensions
EasyDapperExtensions is an extension that supports base CRUD operations (Get, Insert, Update, Delete) using predicate. 

# Installing via NuGet
The easiest way to install Daybreaksoft.Extensions.Functions is via [NuGet](https://www.nuget.org/packages/EasyDapperExtensions).  
In Visual Studio's [Package Manager Console](https://docs.microsoft.com/zh-cn/nuget/tools/package-manager-console), enter the following command:
```bash
Install-Package EasyDapperExtensions
```

# Supported Databases
- SqlServer
- MySql
- Postgres
- Sqlite
- SqlCe

# How to use

The following examples will use a User POCO defined as:

```c#
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string UserName { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedTime { get; set; }
}
```

## Get Operation (with Predicates)

Used to get exactly one entity with given predicate. The following cases will throw exception:
- Not found entity.
- Found more than one entity.
- Not found primary key column.
- Found more than one primary key column.


```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var user = cn.Get<User>(1); 
    // Or use cn.GetAsync<User>(1)
    cn.Close();
}
```

## GetAll Operation (with Predicates)

Used to get all entities based on given predicate.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var users = cn.GetAll<User>(p => p.IsActive && p.CreatedTime > DateTime.Now); 
    // Or use cn.GetAllAsync<User>(p => p.IsActive && p.CreatedTime > DateTime.Now)
    cn.Close();
}
```

## GetPaged Operation (with Predicates)

Used to get paged entities based on given predicate.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var users = cn.GetPaged<User>(0, 10, p => p.IsActive); 
    // Or use cn.GetPagedAsync<User>(0, 10, p => p.IsActive)
    cn.Close();
}
```

## GetSingle Operation (with Predicates)

Used to get exactly one entity with given predicate. Throws exception if no entity or more than one entity.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var user = cn.GetSingle<User>(p => p.Id == 1); 
    // Or use cn.GetSingleAsync<User>(p => p.Id == 1)
    cn.Close();
}
```

## GetSingleOrDefault Operation (with Predicates)

Used to get exactly one entity with given predicate or null if not found. Throws exception if more than one entity.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var user = cn.GetSingleOrDefault<User>(p => p.Id == 1); 
    // Or use cn.GetSingleOrDefaultAsync<User>(p => p.Id == 1)
    cn.Close();
}
```

## GetFirst Operation (with Predicates)

Used to get an entity with given given predicate. Throws exception if no entity.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var user = cn.GetFirst<User>(p => p.IsActive); 
    // Or use cn.GetFirstAsync<User>(p => p.IsActive)
    cn.Close();
}
```

## GetFirstOrDefault Operation (with Predicates)

Used to get an entity with given given predicate or null if not found.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var user = cn.GetFirst<User>(p => p.IsActive); 
    // Or use cn.GetFirstOrDefaultAsync<User>(p => p.IsActive)
    cn.Close();
}
```

## Count Operation (with Predicates)

Used to count quantity with given given predicate.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var count = cn.Count<User>();
    // Or use cn.CountAsync<User>()
    var activeCount = cn.Count<User>(p => p.IsActive);
    // Or use cn.CountAsync<User>(p => p.IsActive)
    cn.Close();
}
```

## Any Operation (with Predicates)

Used to determine whether an entity exists with given given predicate.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var result = cn.Any<User>();
    // Or use cn.AnyAsync<User>()
    var activeResult = cn.Any<User>(p => p.IsActive);
    // Or use cn.AnyAsync<User>(p => p.IsActive)
    cn.Close();
}
```

## Insert Operation

Used to insert a new entity.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    User user = new User 
    {
         UserName = "Admin",
         IsActive = true,
         CreatedTime = DateTime.Now
    };
    cn.Insert(user);
    // Or use cn.InsertAsync(user)
    cn.Close();
}
```
## InsertAndGetId Operation

Used to insert a new entity and return inserted primary key.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    User user = new User 
    {
         UserName = "Admin",
         IsActive = true,
         CreatedTime = DateTime.Now
    };
    var id = cn.InsertAndGetId(user);
    // Or use cn.InsertAndGetIdAsync(user)
    cn.Close();
}
```

## Update Operation

Used to update an existing entity. Throws exception if no primary key column.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    var user = cn.Get<User>(1);
    user.Name = "UpdatedUser";
    cn.Update(user);
    // Or use cn.UpdateAsync(user)
    cn.Close();
}
```

## Delete Operation (with Predicates)

Used to delete an existing entity. Throws exception if no primary key column.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    cn.Delete<User>(1);
    // Or use cn.DeleteAsync(1)
    cn.Close();
}
```

Used to delete entities with given given predicate.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    cn.Delete<User>(p => p.IsActive);
    // Or use cn.DeleteAsync<User>(p => p.IsActive)
    cn.Close();
}
```

# Logging

Support to use Microsoft.Extensions.Logging.ILogger to logging database command information.

```c#
using (var cn = new SqlConnection(_connectionString))
{
    cn.Open();
    cn.Get<User>(1, logger: Logger);
    cn.Close();
}
```

Output

```
Executed DbCommand [Parameters: Id = 1]
SELECT * FROM "Users" WHERE "Id" = @Id
```