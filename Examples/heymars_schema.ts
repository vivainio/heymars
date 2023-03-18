export interface HeymarsSchema {
    commands: Command[];
    root:     string;
}

export interface Command {
    fgcolor?:  string;
    id?:       string;
    runtags?:  string[];
    title?:    string;
    c?:        string;
    matchers?: Matcher[];
    shell?:    boolean;
    tags?:     string[];
    cwd?:      string;
    bgcolor?:  string;
}

export interface Matcher {
    log:      boolean;
    patterns: string[];
    say:      string;
    status:   string;
}
