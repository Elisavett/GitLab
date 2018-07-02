# GitLab.NET
A .NET library for interfacing with the GitLab API.
The main repository is hosted at [GitLab.com](https://gitlab.com/GitLab-dot-NET/GitLab-dot-NET).
Please submit issues via the issue tracker at GitLab.com

## Building

 * Building requires Visual Studio 2015
 * Allow NuGet to restore all packages
 * Currently Requires .NET 4.6.1

## Contributing

* Login in GitLab.com (you need an account)
* Fork the main repository from [GitLab.com](https://gitlab.com/GitLab-dot-NET/GitLab-dot-NET)
* Push your changes to your fork
* Send a merge request

## What works

  * Projects
    * Enumerate Projects
    * Manage Projects
    * Issue Management
    * Merge Request Management
    * Webhooks

  * Users & Groups
    * Session Login with Username / Password
    * Enumerate Groups / Users / Namespaces
    * Create / Update / Delete Groups & Users
    * Manage group membership
    * Transfer Projects
    * Block / Unblock Users
    * Manage user SSH keys

## What's not implemented yet

  * CI API Functions
  * System Hooks
  * System Settings
  * Project Events
  * Project Repository File operations
  * Project Deploy keys
  * Project Services
  * Administer Fork Relationship
