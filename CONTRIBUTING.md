# Contributing

One of the easiest ways to contribute is to participate in discussions on GitHub issues. You can also contribute by submitting pull requests with code changes.
However, please conform to the following guidelines when possible:

## Bugs and feature requests?
Before reporting a new issue, try to find an existing issue if one already exists. If it already exists, upvote (👍) it. Also, consider adding a comment with your unique scenarios and requirements related to that issue.  Upvotes and clear details on the issue's impact help us prioritize the most important issues to be worked on sooner rather than later. If you can't find one, that's okay, we'd rather get a duplicate report than none.

## Pull Requests
Before opening a new pull request, discuss with the community on Github issues.
You just need to follow the instructions in the Pull Request Template for it to be valid.

If you wanna to deep contribute to this project see the above `Pull Requests` section inside the `Deep Developing` section.

## Semantic Versioning

This project follows [Semantic Versioning](http://semver.org/). When
writing changes to this project, it is recommended to write changes
that are SemVer compliant with the latest version of the library in
development.

The working release should be the latest build off of the `dev` branch,
but can also be found on the [development board](https://github.com/NextAudio/NextAudio/projects/1).

We follow the .NET Foundation's [Breaking Change Rules](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/breaking-change-rules.md)
when determining the SemVer compliance of a change.

Obsoleting a method is considered a **minor** increment.

## Coding Style

We attempt to conform to the .NET Foundation's [Coding Style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md)
where possible, with some exceptions:
- We use `var` in every place (we think inference types is just better 😉)
- We always use `.ConfigureAwait(false)` in all await-async calls because this project is a library and can be used in UI applications.

As a general rule, follow the coding style already set in the file you
are editing, or look at a similar file if you are adding a new one.

## Setup

### Cloning
Just clone the repo with the git tool

#### With ssh:
```bash
git clone git@github.com:NextAudio/NextAudio.git
```

#### With http:
```bash
git clone https://github.com/NextAudio/NextAudio.git
```

#### With github cli:
```bash
gh repo clone NextAudio/NextAudio
```

### Requirements
- .NET 6 SDK
- Any IDE with .NET Support (Visual Studio/VS Code/Rider/etc)

### Developing
If you using the Visual Studio IDE open the project by the [NextAudio.sln](https://github.com/NextAudio/NextAudio/blob/main/NextAudio.sln) file.
If you using VS Code you'll need the [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp), and then open the [NextAudio.code-workspace](https://github.com/NextAudio/NextAudio/blob/main/NextAudio.code-workspace) file.

This project require some additional .NET tools to develop you can restore them for the project scope running the command:
```bash
dotnet tool restore
```

### Build

First clean the project output files:
```bash
dotnet clean
```
Then restore all external dependencies
```bash
dotnet restore
```
Now you can finally build the project with the build command:
```bash
dotnet build --no-restore
```

### Linter/Formatter
This project uses the [dotnet-format](https://github.com/dotnet/format) tool that check if our code is correct according with the [.editorconfig](https://github.com/NextAudio/NextAudio/blob/main/.editorconfig) file.

Run the format with this command:
```bash
dotnet format
```

### Tests
We have two folders inside the `./tests` folder, they are: `UnitTests` and `FunctionalTests`, any project inside `./src` folder will have your respective [xunit](https://xunit.net/) test project inside each folder.

### Commiting
We use the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) convention, we also use [Husky.NET](https://alirezanet.github.io/Husky.Net/) to check if the commits are according with the convention.

## Deep Developing
Now we will explain how the project priorization and development works.

### Project structure
- `./src` folder: Here we have all libs in this project, we have a core lib named `NextAudio` and other specific components like `Matroska` named `NextAudio.Matroska`, if you creating a new component you'll need to create a new project too using `dotnet new library -o "Component.Name"` and then add to the solution using `dotnet sln add ./NewProjectPath`.
- `./test` folder: Here we all tests for all libs in this project we already explained this folder in the `Tests` section.


### Milestones
We will ever have a milestone to track all the next planned versions issues/PRs, all these milestones will in the `Version X.X.X` format like `Version 1.0.X`.

We also have another milestone to track all unplanned issues named `Backlog`.

### Labels
You can check all labels and your descriptions in the [label list](https://github.com/NextAudio/NextAudio/labels), here we will explain some of these.

#### Epic
This issue represents an epic change, the same as the [scrum](https://scrumguides.org/) epic.
#### User Story
This issue represents an user story change, the same as the [scrum](https://scrumguides.org/) user story.

#### Design
This issue/PR will change or proposal some design API, like interfaces, abstract classes or structure usage.

#### Good first issue
If you starting to contributing with this project issues tagged with this label are ideal for you now.

### Projects
We have some projects to help us to track, priorize and develop the next versions.

#### Area-Specific projects
We have a project for each project area like `Matroska` for the Matroska format area, and `Core` for the core NextAudio libray area.

Some projects like `Matroska` can have an `area` field inside to indicate nested areas, in this case: `demuxing` and `muxing`.

#### Development Board
In this project we have some views, let's explain all of them:
##### Backlog
Tracks all unplanned issues and PRs, but is in our "radar".

##### Current Sprint
In this view we have the current sprint same as [Scrum](https://scrumguides.org/) sprint format, we have some fields to help our development and categorization:
- Status:
  - Epic/User Story: All issues in this status is a `epic` or `user story`.
  - Backlog: All issues in this status are ready to develop but your development hasn't started yet.
  - In progress: All issues/PRs in this status are in development progress.
  - In review: All PRs in this status are in code review stage.
  - Done: All issues/PRs in this status are done.
- Priority:
  - Low: All issues in this priority are low prority.
  - Medium: All issues in this priority are medium prority.
  - High: All issues in this priority are High prority.
  - Urgent: All issues in this priority are Urgent prority.
- Size:
  - Tiny: all issues in this size are very easy to do.
  - Small: all issues in this size are easy to do.
  - Medium: all issues in this size has a medium difficult to do.
  - Large: all issues in this size has a large difficult to do.
  - X-Large: all issues in this size are extremally difficult to do.
- Sprint:
  - This is a github iteration field, basically github will create every time range a new value in this field to track new sprint (generally 2 weeks).

#### Previous Sprint
The latest sprint view equals the `Current Sprint`.

#### Next Sprint
The next sprint view equals the `Current Sprint`.

#### By Priority
Group all issues in the current sprint by `priority`.

#### By Size
Group all issues in the current sprint by `size`.

### Issues
We have some templates for some types of issues:

- Epic: This template is for an epic issue.
- User Story: This template is for an user story issue.
- Task/Feature: This template is for a task or feature planned for any sprint.
- Bug Report: This template is for a bug report, any user can use this template to report a new bug.
- Feature request (users): This template is for a feature request, any user can use this template to suggest a new feature.

### Pull Requests
Here some tips to make a correct PR:
- Always follow the code style, uses `dotnet format` to check if everything is correct.
- Always create unit/functional tests for your new feature, if you fixing a bug also fix or create a new test to cover this bug too.
- Always check if the project solution are building corretly with `dotnet build`.
- Always check if your PR has a breaking change, if yes describe the breaking change.
- If your PR aren't tracked by an issue describe the old and the new behavior.
- Your PR needs to pass all checks (build/test/lint) and have at least one contributor approved review to be merged.
- Always open PRs to the `main` branch, try always make the PR branch updated with the `main`.

### Final Coding Tips
- Always use `ValueTask` when you can, value tasks can represent synchronous and short asynchronous operations, this type is a `ValueType` and do less GC Pressure, you can read more [here](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/).
- Always use `Span<T>` and `Memory<T>` instead of `Array<T>` because these types don't alloc unnecessary memory in the heap and always will reflect the values by reference, even on slice operations, you can read more [here](https://learn.microsoft.com/en-us/dotnet/standard/memory-and-spans/memory-t-usage-guidelines).
- Always use `ArrayPool<T>` and `MemoryPool<T>` instead of `new Array<T>` when allocating temporary buffers, you can read more [here](https://stackoverflow.com/questions/61856306/difference-between-memorypoolt-and-arraypoolt).
