# Heymars

![image](https://user-images.githubusercontent.com/557579/209970802-30852d6c-2418-4a44-88dd-ff8e113bab46.png)

Heymars is a small launcher GUI that allows you to select commands to run, and it, well, runs them.

Commands can be listed in a .txt or .json file.

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


# License

MIT

