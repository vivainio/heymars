{
    local Bat(cmd) = {
        c: cmd,
        shell: true
    },
    local Comm(title) = {
        title: title,
        fgcolor: "Blue",
        id: ""
    },

    local Msbuild(title, command) = {
        title: title,
        c: command,
        shell: true,
        matchers: [
            {

                patterns: [@"^.*\(\d+,\d+\): error [A-Z]+\d+: .*"],
                log: true,
                status: "Build error",
                say: "Build failed"
            }

        ]  
    },
    local sample = "heymars_sample",
    root: @"..",
    local tbuild = {
        tags: ["build"]
    },
    commands: [
        Comm("Basic heymars commands") + {runtags: ["build"]},
        Msbuild("Release build", "dotnet build -c release Heymars.sln") + tbuild,
        Bat("jsonnet %s.jsonnet -o %s.json" % [sample, sample]) + { cwd: "Examples" } + tbuild,
        Bat("npx quicktype heymars_sample.json --just-types --out=heymars_schema.ts") + {title: "Create type definition with quicktype", cwd: "Examples"},
        Bat("code Examples/heymars_schema.ts") + { bgcolor: "yellow"}
   ]
}
