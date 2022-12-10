using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace webapi
{
    public static class WebApplicationExtensions
    {
        // Message in docker logs indicating the database is ready to use.
        private static Regex _readyMessage = new Regex(@"port:\s+3306\s+mariadb.org binary distribution", RegexOptions.Compiled);
        /// <summary>
        /// Creates a mariadb container with the specified name and port.
        /// </summary>
        public static async Task UseMariadbContainer(
            this WebApplication app, string containerName,
            string connectionString, bool deleteAfterShutdown = true)
        {
            // Den Port aus dem Connection String (z. B. server=localhost;port=13306;database=spengernews;user=root;password=password)
            // extrahieren.
            var port = Regex.Match(connectionString, @"(?<=port=)\d+", RegexOptions.IgnoreCase).Value;
            port = string.IsNullOrEmpty(port) ? "3306" : port;
            var rootPassword = Regex.Match(connectionString, @"(?<=password=).*", RegexOptions.IgnoreCase).Value;

            // On a graceful shutdown we want to delete the container.
            if (deleteAfterShutdown)
                app.Lifetime.ApplicationStopped.Register(async () => await DeleteContainer(containerName, force:true));

            await DeleteContainer(containerName, force: true);
            using var client = new DockerClientConfiguration().CreateClient();
            //docker run --name containerName -d -p 13306:3306 -e MARIADB_USER=root -e MARIADB_ROOT_PASSWORD=xxx mariadb:latest
            var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "mariadb:latest",
                Name = containerName,
                Env = new string[] { "MARIADB_USER=root", $"MARIADB_ROOT_PASSWORD={rootPassword}" },
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>()
                    {
                        { "3306/tcp", new PortBinding[]{new PortBinding() {HostPort = $"{port}/tcp" } } }
                    }
                }
            });
            await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

            var started = DateTime.UtcNow;
            bool ready = false;
            // Important: wait until the database is fully started. We ware waitung for the message
            //    ... port: 3306  mariadb.org binary distribution
            // in docker logs.
            while (!ready && (DateTime.UtcNow - started).TotalSeconds < 30)
            {
                var stream = await client.Containers.GetContainerLogsAsync(
                    container.ID, false, new ContainerLogsParameters() { ShowStdout = true, ShowStderr = true });
                (string stdout, string stderr) = await stream.ReadOutputToEndAsync(default);
                if (_readyMessage.IsMatch(stdout) || _readyMessage.IsMatch(stderr)) { ready = true; }
            }
            if (!ready) { throw new Exception($"Error starting {containerName}: container is not ready."); }
        }

        /// <summary>
        /// Remove a container.
        /// </summary>
        public static async Task DeleteContainer(string containerName, bool force = false)
        {
            using var client = new DockerClientConfiguration().CreateClient();
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters());
            var id = containers.FirstOrDefault(c => c.Names.Any(n => n.Contains(containerName)))?.ID;
            if (id is null) { return; }
            await client.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters() { Force = force });
        }
    }
}
