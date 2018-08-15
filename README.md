<div align="center">
  <a href="https://spm.sirkadirov.com/">
    <img src="https://raw.githubusercontent.com/SirkadirovTeam/simplepm-resources/master/logo.png" alt="SimplePM logo">
  </a>
</div>
<div align="center">
  &nbsp;
  <a href="https://ci.appveyor.com/project/sirkadirov/simplepm-server">
    <img src="https://img.shields.io/appveyor/tests/sirkadirov/simplepm-server.png">
  </a>
  &nbsp;
  <a href="https://ci.appveyor.com/project/sirkadirov/simplepm-server">
    <img src="https://img.shields.io/appveyor/tests/sirkadirov/simplepm-server.png">
  </a>
  &nbsp;
  <a href="https://github.com/SirkadirovTeam/SimplePM_Server/releases">
    <img src="https://img.shields.io/github/release/SirkadirovTeam/SimplePM_Server/all.png" alt="GitHub (pre-)release version">
  </a>
  &nbsp;
  <a href="https://bestpractices.coreinfrastructure.org/projects/1230">
    <img src="https://bestpractices.coreinfrastructure.org/projects/1230/badge" alt="CII Best Practices">
  </a>
  &nbsp;
  <a href="https://www.codefactor.io/repository/github/sirkadirovteam/simplepm_server">
    <img src="https://www.codefactor.io/repository/github/sirkadirovteam/simplepm_server/badge" alt="Codefactor">
  </a>
  &nbsp;
  <a href="https://dependabot.com">
    <img src="https://api.dependabot.com/badges/status?host=github&identifier=86981262" alt="Dependabot Status">
  </a>
  &nbsp;
  <a href="https://app.fossa.io/projects/git%2Bgithub.com%2FSirkadirovTeam%2FSimplePM_Server?ref=badge_shield">
    <img src="https://app.fossa.io/api/projects/git%2Bgithub.com%2FSirkadirovTeam%2FSimplePM_Server.svg?type=shield" alt="FOSSA status">
  </a>
  &nbsp;
  <a>
    <img src="https://img.shields.io/github/license/SirkadirovTeam/SimplePM_Server.svg" alt="Project license">
  </a>
  &nbsp;
</div>
<div align="center">
  <a href="https://spm.sirkadirov.com/">Website</a> • <a href="https://simplepm.atlassian.net/projects/SERVER/">Bug tracker</a> • <a href="https://spm.sirkadirov.com/download.html">Download</a> • <a href="https://simplepm.atlassian.net/">Documentation</a>
</div>

*****

Official repository of SimplePM Server, submissions checking subsystem of software complex "Automated verification system for programming tasks "SimplePM".

Detailed information about the project you can get at project's official website: https://spm.sirkadirov.com/

## Requirements

### System requirements
- All supported by .NET Core platforms (AMD64 / Intel64 only)
- Minimum 2 GB RAM (DDR3) or greater
- Minimum 20 GB free space on HDD/SSD (SATA 3 / PCIe connection recommended)
- Stable LAN/WAN connection to MySQL Server (minimum 100 MBit/s for LAN and 50 MBit/s for WAN recommended)

### Runtime requirements
- .NET Core 2.1 or higher
- MySQL Server 5.8+ (latest)

### Recommendations
- Server execution scripts (`simplepm-server.bat` and `simplepm-server.sh`) before SimplePM Server launching love to kill all other .NET Core processes. If you need to continue running of all that apps, remove associated lines of code in that command files.
- After SimplePM Server installed, you are about to add installation path to `Path` environment variable to enable using server startup scripts and other useful tools, pre-installed with SimplePM Server.

### Notice
Requirements list declared on this page is not full. More information about dependencies and requirements you can get on [project's download page](https://spm.sirkadirov.com/download.html).

## How to build
1. Use `PowerShell` or install `Powershell Core`.
2. Execute script file `solution_release_build.ps1` located in the main repository folder.
3. **Horaaaaaaaaaay!!!**

## Used third-party projects
- Logging feature powered by **NLog**
- Database connection by **MySQL Connector/NET**
- JSON configuration files parsing by **Newtonsoft.JSON (Json.NET)**
- Other NuGet packages, see `packages` files for more

## Sponsors, partners, donors
Full list of SimplePM supporters available on [project's official website](https://spm.sirkadirov.com/). Anyway, you must to know that we love them :)

## Licensing
Source code of the project is licensed under *GNU AGPLv3 License*, official builds of this project licensed under SimplePM EULA, full text of it you can get at official website: https://spm.sirkadirov.com/

### Author rights and patents
Initial copyright and patents holder of this project - [Yurij Kadirov (Sirkadirov)](https://sirkadirov.com/).

- SimplePM v2.X.X "Moongirl" - Copyright (C) 2018, Yurij Kadirov and other contributors. All rights reserved.
- SimplePM v1.X.X "Alien Baroness" - Copyright (C) 2017, Yurij Kadirov and other contributors. All rights reserved.

### Open source license approval
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2FSirkadirovTeam%2FSimplePM_Server.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2FSirkadirovTeam%2FSimplePM_Server?ref=badge_large)
