using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Api;
using Toggl.Api.DataObjects;

namespace TimeLogger
{
    class Program
    {
        static async Task<int> Main(
            IConsole console,
            string? description = null, 
            string? togglApiKey = null, 
            string? project = null, 
            bool? isBillable = null, 
            string? workspace = null)
        {
            togglApiKey ??= Environment.GetEnvironmentVariable("TogglApiKey");
            if (string.IsNullOrWhiteSpace(togglApiKey))
            {
                console.Error.Write("A toggle api key is required");
                return 1;
            }
            TogglClient client = new(togglApiKey);
            if (await client.TimeEntries.GetCurrentAsync() is { } current &&
                current.Id != null)
            {
                if (current.Description == description)
                {
                    await client.TimeEntries.StopAsync(current);
                    return 0;
                }                
            }

            Workspace targetWorkspace = await GetWorkspace(client, workspace);
            
            Project? targetProject = await GetProject(client, project);

            await client.TimeEntries.StartAsync(new TimeEntry
            {
                Description = description,
                IsBillable = isBillable ?? targetProject?.IsBillable,
                CreatedWith = "TimeLogger Console Automation",
                ProjectId = targetProject?.Id,
                WorkspaceId = targetWorkspace.Id
            });

            return 0;
        }

        private static async Task<Workspace> GetWorkspace(TogglClient client, string? workspace)
        {
            List<Workspace> workspaces = await client.Workspaces.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(workspace))
            {
                if (int.TryParse(workspace, out int workspaceId) &&
                    workspaces.FirstOrDefault(x => x.Id == workspaceId) is { } workspaceById)
                {
                    return workspaceById;
                }

                if (workspaces.FirstOrDefault(x => string.Equals(x.Name, workspace, StringComparison.OrdinalIgnoreCase))
                    is { } workspaceByName)
                {
                    return workspaceByName;
                }
            }

            return workspaces.First();
        }

        private static async Task<Project?> GetProject(TogglClient client, string? project)
        {
            if (!string.IsNullOrWhiteSpace(project))
            {
                if (int.TryParse(project, out int projectId))
                {
                    return await client.Projects.GetAsync(projectId);
                }
                else
                {
                    List<Project> allProjects = await client.Projects.ListAsync();
                    return allProjects.FirstOrDefault(x =>
                        string.Equals(x.Name, project, StringComparison.OrdinalIgnoreCase));
                }
            }

            return null;
        }
    }
}
