<p align="center">
    <img src="Flow.Launcher.Plugin.QueryGroups\Assets\icon.png" alt="QueryGroups Plugin Icon" height="120">
</p>

# Query Groups for Flow Launcher

A plugin that lets you organize queries into groups for quick access within [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher).

## Features
- **Create named query groups** - Organize related search queries together
- **Easy management** - Intuitive interface for adding, editing, and removing groups and their queries.
- **Save complex queries** - Save long queries, under a group and a short display name.
- **Quick access** - Search groups by name or browse all available groups.


## Installation

### Plugin Store / Manager
1. Open Flow Launcher
2. Search for ``pm install Query Groups by DavidGBrett`` or use the plugin store   
3. Install the plugin

### Manual Debug Install
1. Download this repository
2. Run the file ``debug.ps1``

## Usage

Query Groups can be managed in two places:
- **The Settings page** (useful for bulk or structured edits)
- **The Flow Launcher search interface** (faster and more intuitive for day-to-day use)

### Managing via Search (recommended)

Typing ``qg`` lists your existing query groups and includes an option to add a new one.
Any groups you create will show up in the defualt search as well.

- Selecting a **group** shows the queries it contains, along with an option to add another.
- Selecting a **query** item replaces the current input with its stored query. 

- Right-click a **group** to rename or delete it.
- Right-click a **query** to rename it, change its query, or delete it.

### Managing via Settings

Groups and queries can also be created and edited from Flow Launcher's Settings under Plugins -> Query Groups.