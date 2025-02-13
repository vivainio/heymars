# Heymars

![image](https://user-images.githubusercontent.com/557579/209970802-30852d6c-2418-4a44-88dd-ff8e113bab46.png)

Heymars is a small, configurable launcher GUI for technical users.

Commands can be listed in a .txt, .json or .jsonnet file.

# Installation

It's not published to a repository. Easiest installation is installing [uv](https://docs.astral.sh/uv/) and running:

```
uvx shimpan get https://github.com/vivainio/heymars/releases/download/v1.2.1/heymars-1.2.1.zip --to ~/.local/bin
```

You need .NET 8 to run it.

# Overview

Here's what it looks like:

![image](https://user-images.githubusercontent.com/557579/226138555-9c922cef-2132-4a67-8540-5d268ddd007b.png)

This was created from input json file:

```json
{
   "commands": [
      {
         "fgcolor": "Blue",
         "id": "",
         "runtags": [
            "build"
         ],
         "title": "Basic heymars commands"
      },
      {
         "c": "dotnet build -c release Heymars.sln",
         "matchers": [
            {
               "log": true,
               "patterns": [
                  "^.*\\(\\d+,\\d+\\): error [A-Z]+\\d+: .*"
               ],
               "say": "Build failed",
               "status": "Build error"
            }
         ],
         "shell": true,
         "tags": [
            "build"
         ],
         "title": "Release build"
      },
      {
         "c": "jsonnet heymars_sample.jsonnet -o heymars_sample.json",
         "cwd": "Examples",
         "shell": true,
         "tags": [
            "build"
         ]
      },
      {
         "c": "npx quicktype heymars_sample.json --just-types --out=heymars_schema.ts",
         "cwd": "Examples",
         "shell": true,
         "title": "Create type definition with quicktype"
      },
      {
         "bgcolor": "yellow",
         "c": "code Examples/heymars_schema.ts",
         "shell": true
      }
   ],
   "root": ".."
}

```

Heymars directly supports running [jsonnet](https://jsonnet.org/) files if you have jsonnet.exe installed. This json was generated with this jsonnet file:

```jsonnet
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

```

It is recommended to create your json files with jsonnet, to avoid repetition and e.g. reuse "C# error matchers" for different projects.

[Here](https://github.com/vivainio/heymars/blob/main/Examples/heymars_schema.ts) is the generated typescript schema you can use for quick reference.

For casual use, you can use plain text file as input. This file creates 4 commands:

```
echo hello
echo world
ls
pwd
```

You can gradually update to full blown json config by interleaving lines containing commands in json format:

```
echo hello
echo world
ls
{ "title": "More complex stuff you can fit on one line", "c": "echo foobar!", "fgcolor": "green", "id": "" }
pwd
```

# License

MIT
