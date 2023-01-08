from __future__ import print_function

import os, shutil, glob

prjdir = "Heymars"
version = "1.0"


def c(s):
    print(">", s)
    err = os.system(s)
    assert not err


def nuke(pth):
    if os.path.isdir(pth):
        shutil.rmtree(pth)


def rm_globs(*globs):
    for g in globs:
        files = glob.glob(g)
        for f in files:
            print("Del", f)
            os.remove(f)


nuke(prjdir + "/bin")
nuke(prjdir + "/obj")
nuke("deploy")

c("dotnet publish -c release --arch x64")
os.mkdir("deploy")
os.chdir("%s/bin/release" % prjdir)

rel_bin = "net6.0-windows/win-x64/publish"

rm_globs(f"{rel_bin}/*.pdb",f"{rel_bin}/*.xml")
os.rename(rel_bin, "Heymars")

c("7za a ../../../deploy/Heymars-%s.zip Heymars" % version)
