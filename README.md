# Bede.Lottery

## Prerequisites
1. .NET 9.0 SDK
2. VSCode with C# Dev Kit or Visual Studio 2022

## How it works in a nutshell
- Program.cs uses the framework's host application builder to setup DI, logging and configuration services as well as all program related dependencies.
- Services/LotteryApplicationService.cs handles lifecycle steps orchestration and program flow execution.
- Controller/LotteryController.cs binds program flow actions to views and uses the injected LotteryService to handle the concrete business logic.
- Services/ConfiguredLotteryService.cs sources and validates configurable application (using the default appsettings.json) and request model values. All configurable values are defined under the "Lottery" section.
- Services/LotteryDrawServiceBuilderProvider.cs detaches the source of the lottery configuration while allowing the rest of the dependencies to be injected through the builder to the LotteryDrawService.
- Services/LotteryDrawServiceBuilder.cs separates adding players to the lottery draw from the actual draw execution.
- Services/LotteryDrawService.cs executes the main lottery implementation and creates view models that can be used to render the results to the user.
- Views/* contains view classes which are abstract enough to be used in a context different than the console (i.e. writing results to a file, network stream etc.)
- Bede.Lottery.Console.Tests contains all test classes using MSTest and FluentAssertions to ensure all classes can be tested in isolation.
- Bede.Lottery.Contracts contains all interfaces and data/view models implemented as immutable records.
