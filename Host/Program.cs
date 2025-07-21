using Host;

DotEnv.Load("../.env");

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> discordToken = builder.AddParameter("discord-token", secret: true);
IResourceBuilder<ParameterResource> dbUsername   = builder.AddParameter("db-username",   secret: true);
IResourceBuilder<ParameterResource> dbPassword   = builder.AddParameter("db-password",   secret: true);

IResourceBuilder<ParameterResource> cachePassword = builder.AddParameter("cache-password", secret: true);

IResourceBuilder<PostgresServerResource>   pg = builder.AddPostgres("pg", dbUsername, dbPassword);
IResourceBuilder<PostgresDatabaseResource> db = pg.AddDatabase("db");

IResourceBuilder<RedisResource> cache = builder.AddRedis("cache", 6379, cachePassword);

IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Api>("backend", options => {
	options.ExcludeKestrelEndpoints = true;
});

cache.WithImage("library/redis", "8-alpine");
cache.WithLifetime(ContainerLifetime.Persistent);
cache.WithDataBindMount(source: "./data/cache");
cache.WithHostPort(6379);

pg.WithImage("library/postgres", "17-alpine");
pg.WithLifetime(ContainerLifetime.Persistent);
pg.WithDataBindMount(source: "./data/db");
pg.WithHostPort(5432);

api.WithEnvironment("discord-token", discordToken);
api.WithReference(cache);
api.WaitFor(cache);
api.WithReference(db);
api.WaitFor(db);

builder.AddDockerComposeEnvironment("compose");

builder.Build().Run();