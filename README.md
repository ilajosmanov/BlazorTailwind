# BlazorTailwind

## Motivation
Motivation here...

## Solving problem
1. Auto watch in dev mode
2. Auto build and minify when Release Stage has started

## How to use

1. Create Directory.Build.props

```xml
<Project>
    <PropertyGroup>
        <SolutionDir>$(MSBuildThisFileDirectory)</SolutionDir>
        <RunTailwindTasks>true</RunTailwindTasks>
        <TailwindCssInput>Path/to/main.css</TailwindCssInput>
        <TailwindCssOutput>Path/To/wwwroot/compiled.css</TailwindCssOutput>
        <TailwindCssConfig>Path/To/tailwind.config.js</TailwindCssConfig>
    </PropertyGroup>
</Project>
```

1. Create tailwind.config.js
```js
module.exports = {
    content: {
        relative: true,
        files: [
            "./**/Components/**/*.{html,razor,cshtml}",
        ]
    }
}
```

2. In your Startup.cs/Program.cs

```cs
if (app.Environment.IsDevelopment())
{
    // Watch and recompile css every time when .razor files changed
    _ = Tailwind
        .DevAsync(@"<ProjectName>/Path/To/main.css", @"<ProjectName>/Path/To/wwwroot/compiled.css")
        .ConfigureAwait(false);

    // Stop tailwind
    app.Lifetime.ApplicationStopping.Register(Tailwind.StopTailwindProcess);
}
```