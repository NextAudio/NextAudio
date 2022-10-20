#!/bin/bash
version=$1
version=${version/v/""}

sed -i "4s/.*/    <VersionPrefix>$version<\/VersionPrefix>/" NextAudio.targets


sed -i "5i$2\n" CHANGELOG.md
