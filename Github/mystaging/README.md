# 欢迎使用 MyStaging

```
////////////////////////////////////////////////////////
///                                                  ///
///                        | |      (_)              ///
///    _ __ ___  _   _ ___| |_ __ _ _ _ __   __ _    ///
///   | '_ ` _ \| | | / __| __/ _` | | '_ \ / _` |   ///
///   | | | | | | |_| \__ \ || (_| | | | | | (_| |   ///
///   |_| |_| |_|\__, |___/\__\__,_|_|_| |_|\__, |   ///
///               __/ |                      __/ |   ///
///              |___/                      |___/    ///
///                                                  ///
////////////////////////////////////////////////////////
```

 MyStaging 是一款基于 .NETCore 平台的 ORM 中间件，提供简单易用的接入工具，全链路写法，支持 DbFirst/CodeFirst，而且两种模式（DbFirst/CodeFirst）可以无缝切换。比如一开始你是先创建数据库，然后生成了实体，在接下来的开发过程中，改动实体对象后，可以使用CodeFirst进行无缝迁移，自由使用DbFirst/CodeFirst进行迁移工作 。
 
 支持多种数据库类型，和 EF 不同的是，对单个项目的多路上下文支持中引进了主从数据库概念，查询默认从库，也可以指定主库，删除/修改/新增操作默认走主库，地层还提供了对单个查询数据的分布式缓存操作，可以自由灵活配置，目前 MyStaging 还在持续完善中，欢迎加入 Star/Contributors/Fork。

# 相关组件

MyStaging一共分为三个部分，分别是：
* 1、基础框架 - MyStaging
* 2、提供程序 - MyStaging.Mysql/MyStaging.PostgreSQL
* 3、迁移工具 - MyStaging.Gen


# 在包管理控制台安装 MyStaging.Gen 到 dotnet tool 命令

  MyStaging.Gen 是一个独立的数据库迁移组件，其本质上是一个控制台程序，你可以单独下载这个包到本地，也可以将他安装到 dotnet tool ，安装到 dotnet tool 后，你就可以在 visual studio 中使用命令进行数据库的迁移工作。
  
## 安装迁移工具到 dotnet tool

```
dotnet tool install -g MyStaging.Gen
```

要使用 MyStaging.Gen 请根据下面的参数说明，执行创建实体对象映射.

```
--help 查看帮助
-m [mode，db[DbFirst]/code[CodeFirst]，默认为 DbFirst
-t [dbtype[Mysql/PostgreSQL]，数据库提供程序]  required
-d [database，数据库连接字符串] required
-n [name，数据库上下文名称]  required
-o [output，实体对象输出路径]，默认为 {name}/Model
```
```
==============示例==============
  CodeFirst：
  mystaging.gen -m code -t PostgreSQL -p Pgsql -d "Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=mystaging;"

  DbFirst：
  mystaging.gen -m db -t PostgreSQL -p Pgsql -d "Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=mystaging;"
================================
```

** 注意，上面的两种迁移模式可以实时切换使用，不影响一开始选择的模式，且无副作用。

# 如何选择数据库提供程序

 MyStaging 提供了多种数据库的支持，目前提供了 PostgreSQL/Mysql 的支持，后续将陆续开发更多提供程序，比如基于 PostgreSQL 进行开发的程序，那么可以选择引用包 MyStaing.PostgreSQL。
 
| 数据库 | 提供程序 |
|-----|-----|
| PostgreSQL | MyStaing.PostgreSQL  |
| Mysql | MyStaging.Mysql   |

# 迁移过程

当你进行数据库关系迁移后，MyStaging会在指定的路径上生成实体对象文件目录：Model，该目录包含了数据库上下文对象 xxxDbContext 和实体对象的文件，以 /examples/Mysql 项目为例子，执行迁移后，将生成 Model/MysqlDbContext.cs，该文件即为上下文对象；相反的，可以使用了CodeFirst进行 Model 实体对象的迁移，MyStaging 会检查指定程序集的实体对象，当发现对象携带 TableAttribute 特性时，MyStaging会认为该对象参与迁移。

```
    [Table(name: "article", Schema = "mystaging")]
    public partial class Article
    {
        [Column(TypeName = "tinyint(1)")]
        public bool State { get; set; }
        /// <summary>
        ///  主键，自增
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int id { get; set; }
        public int userid { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime createtime { get; set; }
        public string IP { get; set; }
    }
```

## 属性迁移

迁移后的实体对象，都是分部类（partial），在有些情况下，我们需要在实体对象上增加一些影子属性，影子属性通常指数据库中存在的字段，而实体对象中并没有定义，反之一样。如果需要定义影子属性在实体对象中，你只需要在影子属性上增加特性 NotMappedAttribute 即可。

```
    [Table(name: "article", Schema = "mystaging")]
    public partial class Article
    {
       [NotMapped]
       public string IP { get; set; }
    }
```

# 初始化数据库上下文

初始化 DbContext 上下文对象时，要求传入一个配置 StagingOptions，该 StagingOptions 包含了一些必须和可选的配置

```
public StagingOptions(string name, string master, params string[] slaves)
```

上面的 StagingOptions 构造函数中的参数表示： name=配置的名称，master=主数据库的连接字符串，slaves=从库的连接字符串（支持多个），其它没有出现在构造函数中的属性，表示可选参数，可选参数包含了 CacheOptions（缓存选项）和 Logger（日志组件），如果你配置了日志和缓存，MyStaging将在某些场景下启用该设置，比如针对查询单个对象的主键缓存，缓存还支持分布式缓存（IDistributedCache）

## 最终初始化上下文对象

```
    // 控制台应用程序
    static void Main(string[] args)
    {
        var options = new MyStaging.Metadata.StagingOptions("MySql", "server=127.0.0.1;user id=root;password=root;database=mystaging");
        var context = new MysqlDbContext(options);
    }

    // Web应用程序
    public void ConfigureServices(IServiceCollection services)
	{
        var options = new MyStaging.Metadata.StagingOptions("MySql", "server=127.0.0.1;user id=root;password=root;database=mystaging");
        var context = new MysqlDbContext(options);
		services.AddScoped(this.Configuration);
    }
```

# CURD合集

## 插入

此示例包含单次插入和批量插入
```
    var article = new Article()
    {
        content = "你是谁？你从哪里来？要到哪里去？",
        createtime = DateTime.Now,
        userid = customer.Id,
        IP = "127.0.0.1",
        State = true,
        title = "振聋发聩的人生三问"
    };
    
    var list = new System.Collections.Generic.List<Article>();
    for (int i = 0; i < 10; i++)
    {
        list.Add(article);
    }
    
    // 单个插入
    var newArticle = context.Article.Insert.Add(article);
    // 批量插入
    var affrows = context.Article.Insert.AddRange(list).SaveChange();
```

## 更新

与 EF 不同的是，MyStaging的更新采用无附加实体的方式，直接执行更新过程

```
    var article = context.Article.Update.SetValue(f => f.content, "未来已来，从这里开始").Where(f => f.id == 1001).SaveChange();
```

## 删除

删除和更新类似，都是直接执行

```
    context.Article.Delete.Where(f => f.id == 1001).SaveChange();
```

## 查询

查询比较复杂，不过基本和 EF 类似的语法

```
    // 单个查询
    var article = context.Customer.Select.Where(f => f.Id == 2 && f.Name == "Ron").ToOne();
    // 列表查询，排序、分页、分组
    var articles = context.Customer.Select.OrderBy(f => f.Id).Page(1, 10).GroupBy("Name").ToList();
    // 表连接查询
    var article = context.Article.Select.InnerJoin<Customer>("b", (a, b) => a.userid == b.Id).Where<Customer>(f => f.Id == 2).ToOne();
    // 首字段查询，ToScalar 参数可以传递 Sql 参数，比如 SUM(x)
    var id = context.Customer.Select.Where(f => f.Id == 2 && f.Name == "Ron").ToScalar<int>("Id");
```
#
更多示例，请访问 /examples
