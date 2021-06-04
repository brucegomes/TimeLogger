# Time Logger

This is a simple CLI tool for managing [Toggl](https://toggl.com) time entries.

## Installation

```
dotnet tool install -g Cadence
```
You will need to generate a Toggl API to use this tool. The directions for this can be found [here](https://support.toggl.com/en/articles/3116844-where-is-my-api-token-located).

This tool also assumes that you have already setup appropriate projects within your Toggl account as well. 

You can either pass the API token into the tool on each invocation using the `--toggl-api-key` or by setting it as the "TogglApiKey" environment variable.

## Running the tool

Start a time for a given project with a description
```
cadence --description "<descripton>" --project "<project name>"
```

Stop the active time (omit the description; or use empty string)
```
cadence --description "" --project "<project name>"
```