/*
 * Starts a database container with the ASP.NET Core Webapp
 * Author: Michael Schletz, 2023
 * Dependencies: Docker.DotNet <PackageReference Include="Docker.DotNet" Version="3.*" />
 * Usage example (after var app = builder.Build()):
 *   await app.UseSqlServerContainer(
 *       containerName: "spengernews_sqlserver", version:"latest",
 *       connectionString: app.Configuration.GetConnectionString("Default"), 
 *       deleteAfterShutdown: true);
 */
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi
{
    public static class WebApplicationDockerExtensions
    {
        /// <summary>
        /// Creates a SQL Server container and waits for completition.
        /// Connection string is a normal EF Core connection string like
        ///     Server=127.0.0.1,1433;Initial Catalog=MeineDb;User Id=sa;Password=SqlServer2019;TrustServerCertificate=true
        /// Usage example:
        ///     await app.UseSqlServerContainer(
        ///         containerName: "spengernews_sqlserver", version:"latest",
        ///         connectionString: app.Configuration.GetConnectionString("SqlServer"), 
        ///         deleteAfterShutdown: true);
        /// </summary>
        public static async Task UseSqlServerContainer(
            this WebApplication app, string containerName, string version,
            string connectionString, bool deleteAfterShutdown = true)
        {
            var splittedConnectionString = SplitConnectionString(connectionString);
            if (!splittedConnectionString.TryGetValue("server", out var server))
                throw new Exception($"Missing property Server in connection string {connectionString}.");
            if (!splittedConnectionString.TryGetValue("user id", out var username))
                throw new Exception($"Missing property User Id in connection string {connectionString}.");
            if (!splittedConnectionString.TryGetValue("password", out var password))
                throw new Exception($"Missing property Password in connection string {connectionString}.");
            var serverSplitted = server.Split(",");
            var port = serverSplitted.Length > 1 ? serverSplitted[1] : "1433";

            var containerParameters = new CreateContainerParameters()
            {
                Image = $"mcr.microsoft.com/azure-sql-edge:{version}",
                Name = containerName,
                Env = new string[] { "ACCEPT_EULA=Y", $"SA_PASSWORD={password}" },
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>()
                    {
                        { "1433/tcp", new PortBinding[]{new PortBinding() {HostPort = $"{port}/tcp" } } }
                    }
                }
            };
            // When the container starts, it needs to perform some initialization logic.
            // So we need to send a command to the database and wait for the result.

            // The M1 container does not have the mssql-tools package to send a command...
            // await StartContainer(
            //    app: app, logger:logger, containerName: containerName, containerParameters: containerParameters,
            //    waitCommand: new string[] { "/opt/mssql-tools/bin/sqlcmd", "-U", username, "-P", password, "-Q", "SELECT 'i_am_ready';" },
            //    waitForMessage: "i_am_ready", deleteAfterShutdown: deleteAfterShutdown);

            // so we have to wait for a log message.
            await StartContainer(
                app: app, containerName: containerName, containerParameters: containerParameters,
                waitForMessage: "SQL Server is now ready for client connections", deleteAfterShutdown: deleteAfterShutdown);
        }

        /// <summary>
        /// Creates a mariaDb container and waits for completition.
        /// Connection string is a normal EF Core connection string like
        ///     server=localhost;port=13306;database=SpengernewsDb;user=root;password=MariaDbPassword
        /// Usage example:
        ///     await app.UseMariaDbContainer(
        ///         containerName: "spengernews_mariadb", version:"10.10.3",
        ///         connectionString: app.Configuration.GetConnectionString("Default"), 
        ///         deleteAfterShutdown: true);
        /// </summary>
        public static async Task UseMariaDbContainer(
            this WebApplication app, string containerName, string version,
            string connectionString, bool deleteAfterShutdown = true)
        {
            var splittedConnectionString = SplitConnectionString(connectionString);
            if (!splittedConnectionString.TryGetValue("port", out var port))
                port = "3306";
            if (!splittedConnectionString.TryGetValue("password", out var password))
                throw new Exception($"Missing property Password in connection string {connectionString}.");

            var containerParameters = new CreateContainerParameters()
            {
                Image = $"mariadb:{version}",
                Name = containerName,
                Env = new string[] { $"MARIADB_ROOT_PASSWORD={password}" },
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>()
                    {
                        { "3306/tcp", new PortBinding[]{new PortBinding() {HostPort = $"{port}/tcp" } } }
                    }
                }
            };
            // We wait for a log message "MariaDB init process done. Ready for start up.".
            await StartContainer(
                app: app, containerName: containerName, containerParameters: containerParameters,
                waitForMessage: "MariaDB init process done. Ready for start up.", deleteAfterShutdown: deleteAfterShutdown);
        }

        /// <summary>
        /// Creates a PostgreSQL container and waits for completition.
        /// Connection string is a normal EF Core connection string like
        ///     Host=127.0.0.1;Port=15432;Username=spengernews;Password=PostgresPassword
        /// Usage example:
        ///     await app.UsePostgresContainer(
        ///         containerName: "spengernews_postgres", version:"15.1",
        ///         connectionString: app.Configuration.GetConnectionString("Default"), 
        ///         deleteAfterShutdown: true);
        /// </summary>
        public static async Task UsePostgresContainer(
            this WebApplication app, string containerName, string version,
            string connectionString, bool deleteAfterShutdown = true)
        {
            var splittedConnectionString = SplitConnectionString(connectionString);
            if (!splittedConnectionString.TryGetValue("host", out var host))
                throw new Exception($"Missing property Host in connection string {connectionString}.");
            if (!splittedConnectionString.TryGetValue("port", out var port))
            {
                var hostSplitted = host.Split(":");
                port = hostSplitted.Length > 1 ? hostSplitted[1] : "5432";
            }
            if (!splittedConnectionString.TryGetValue("username", out var username))
                throw new Exception($"Missing property Username in connection string {connectionString}.");
            if (!splittedConnectionString.TryGetValue("password", out var password))
                throw new Exception($"Missing property Password in connection string {connectionString}.");


            var containerParameters = new CreateContainerParameters()
            {
                Image = $"postgres:{version}",
                Name = containerName,
                Env = new string[] { $"POSTGRES_USER={username}", $"POSTGRES_PASSWORD={password}" },
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>()
                    {
                        { "5432/tcp", new PortBinding[]{new PortBinding() {HostPort = $"{port}/tcp" } } }
                    }
                }
            };
            // We wait for a log message "database system is ready to accept connections".
            await StartContainer(
                app: app, containerName: containerName, containerParameters: containerParameters,
                waitForMessage: "database system is ready to accept connections", deleteAfterShutdown: deleteAfterShutdown);
        }

        /// <summary>
        /// Core logic for container creation.
        /// </summary>
        private static async Task StartContainer(WebApplication app, string containerName,
            CreateContainerParameters containerParameters, string waitForMessage, string[]? waitCommand = null,
            bool deleteAfterShutdown = true)
        {
            using var client = new DockerClientConfiguration().CreateClient();

            // Pull the image if it is not local.
            ImagesListResponse? image = null;
            try
            {
                var images = await client.Images.ListImagesAsync(new ImagesListParameters());
                image = images.FirstOrDefault(i => i.RepoTags?.Contains(containerParameters.Image) ?? false);
            }
            catch (TimeoutException)
            {
                throw new Exception("Timeout when listing Docker images. Is Docker running?");
            }
            if (image is null)
            {
                app.Logger.LogInformation($"Downloading {containerParameters.Image}...");
                // Show some progress in the log.
                var pullProgress = new Progress<JSONMessage>(e =>
                {
                    if (e.Progress is not null && e.Progress.Total > 0 && e.Progress.Current == e.Progress.Total)
                        app.Logger.LogInformation(e.ProgressMessage);
                });
                await client.Images.CreateImageAsync(
                    new ImagesCreateParameters() { FromImage = containerParameters.Image },
                    new AuthConfig(), pullProgress);
            }

            // If the last shutdown was not graceful the container was not removed. So we remove
            // an existing container before we create one. After that we register the deletion
            // of the created container.
            if (deleteAfterShutdown)
            {
                await DeleteContainer(containerName, force: true);
                app.Lifetime.ApplicationStopped.Register(() =>
                {
                    // Wait synchronously because this is an Action<T> not a Func<Task>
                    DeleteContainer(containerName, force: true).Wait(5000);
                });
            }

            var container = await client.Containers.CreateContainerAsync(containerParameters);
            await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            if (waitCommand is not null)
            {
                app.Logger.LogInformation($"Send command {waitCommand} and checks if it returns {waitForMessage}...");
                var result = await WaitForOutputCommand(client, container.ID, waitCommand, waitForMessage);
                if (!result)
                    throw new Exception($"Container {containerName} could not start. {string.Join(" ", waitCommand)} returns not {waitForMessage} or exit code 0.");
            }
            else
            {
                app.Logger.LogInformation($"Wait for the message {waitForMessage} in docker logs...");
                var result = await WaitForOutputLog(client, container.ID, waitForMessage);
                if (!result)
                    throw new Exception($"Failed to wait for the message ${waitForMessage} in docker logs.");
            }
        }

        /// <summary>
        /// Remove a container.
        /// </summary>
        private static async Task DeleteContainer(string containerName, bool force = false)
        {
            // We call DeleteContainer at shutdown, so we need an own client.
            using var client = new DockerClientConfiguration().CreateClient();

            // To list stopped containers we have to set All = true
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            var id = containers.FirstOrDefault(c => c.Names.Any(n => n.Contains(containerName)))?.ID;
            if (id is null) { return; }
            await client.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters() { Force = force });
            // Delete orphaned volumes.
            await client.Volumes.PruneAsync(new VolumesPruneParameters());
        }

        private static async Task<bool> WaitForOutputCommand(DockerClient client, string containerId, string[] waitCommand, string waitForMessage)
        {
            var execDeclareExchangeParameters = new ContainerExecCreateParameters
            {
                AttachStdout = true,
                AttachStderr = true,
                Cmd = waitCommand
            };
            var response = await client.Exec.ExecCreateContainerAsync(containerId, execDeclareExchangeParameters);
            using var stdOutAndErrStream = await client.Exec.StartAndAttachContainerExecAsync(response.ID, false);
            var (stdout, stderr) = await stdOutAndErrStream.ReadOutputToEndAsync(default);
            var result = await client.Exec.InspectContainerExecAsync(response.ID);
            return result.ExitCode == 0 && stdout.Contains(waitForMessage);
        }

        /// <summary>
        /// Reads Docker logs and waits for a specific string to occur.
        /// </summary>
        private static async Task<bool> WaitForOutputLog(DockerClient client, string containerId, string waitForMessage, int timeout = 30)
        {
            var started = DateTime.UtcNow;
            bool ready = false;

            while (!ready && (DateTime.UtcNow - started).TotalSeconds < timeout)
            {
                using var stream = await client.Containers.GetContainerLogsAsync(
                    containerId, false, new ContainerLogsParameters() { ShowStdout = true, ShowStderr = true });
                (string stdout, string stderr) = await stream.ReadOutputToEndAsync(default);
                if (stdout.Contains(waitForMessage) || stderr.Contains(waitForMessage)) { ready = true; }
            }
            return ready;
        }

        /// <summary>
        /// Extracts the parts of the connection string. Properties are returned in lower case.
        /// </summary>
        private static Dictionary<string, string> SplitConnectionString(string connectionString)
        {
            return connectionString.Split(";").Select(part =>
            {
                var parts = part.Split("=");
                if (parts.Length < 2) throw new Exception($"Part {part} in connection string is not key=value.");
                if (string.IsNullOrEmpty(parts[1])) throw new Exception($"Part {part} has no value.");
                return (Key: parts[0].ToLower().Trim(), Value: parts[1].Trim());
            })
            .ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
