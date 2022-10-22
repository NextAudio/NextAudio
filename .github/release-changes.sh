#!/bin/bash
version=$1
version=${version/v/""}

body=${2//$'\n'/\\n}

echo "version=$version" >> $GITHUB_ENV

sed -i "4s/.*/    <VersionPrefix>$version<\/VersionPrefix>/" NextAudio.targets

sed -i "5i$body\n" CHANGELOG.md
