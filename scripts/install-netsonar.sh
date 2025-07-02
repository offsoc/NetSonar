#!/bin/bash
#
# Script to download and install/upgrade NetSonar in current location
# Can be run outside NetSonar and as standalone script
# Then run this script
# usage 1: ./install-netsonar.sh
# usage 2: ./install-netsonar.sh 0.1.0
#

#cd "$(dirname "$0")"
arch="$(uname -m)" # x86_64 or arm64
archCode="${arch/86_/}"
osVariant=''       # osx, linux, arch, rhel
tag="${1#v}"       # Download specific version, passed as first argument to the script
owner="sn4k3"
software="NetSonar"
api_url="https://api.github.com/repos/$owner/$software/releases/latest"
macOS_least_version='13.0'

# Arch validation
if [ "$arch" != "x86_64" -a "$arch" != "arm64" ]; then
    echo "Error: Unsupported host arch $arch"
    exit -1
fi

# Tag validation
if [[ "$tag" =~ ^v[0-9]+[.][0-9]+[.][0-9]+$ ]]; then
    api_url="https://api.github.com/repos/$owner/$software/releases/tags/$tag"
elif [[ "$tag" =~ ^[0-9]+[.][0-9]+[.][0-9]+$ ]]; then
    api_url="https://api.github.com/repos/$owner/$software/releases/tags/v$tag"
elif [ "$tag" != "latest" -a -n "$tag" ]; then
    echo "Error: Invalid '$tag' tag/version was provided."
    exit -1
else
    tag='latest'
fi

version() { echo "$@" | awk -F. '{ printf("%d%03d%03d%03d\n", $1,$2,$3,$4); }'; }
testcmd() { command -v "$1" &> /dev/null; }
get_filesize() {
    (
      du --apparent-size --block-size=1 "$1" 2>/dev/null ||
      gdu --apparent-size --block-size=1 "$1" 2>/dev/null ||
      find "$1" -printf "%s" 2>/dev/null ||
      gfind "$1" -printf "%s" 2>/dev/null ||
      stat --printf="%s" "$1" 2>/dev/null ||
      stat -f%z "$1" 2>/dev/null ||
      wc -c <"$1" 2>/dev/null
    ) | awk '{print $1}'
}
download(){
    if [ -z "$1" ]; then
        echo 'Error: Download url was not specified!'
        exit -1
    fi

    filename="$(basename "${download_url}")"
    tmpfile="$(mktemp "${TMPDIR:-/tmp}"/${software}Update.XXXXXXXX)"

    echo "- Downloading: $download_url"
    curl -L --retry 4 $download_url -o "$tmpfile"

    download_size="$(curl -sLI $download_url | grep -i Content-Length | awk 'END{print $2}' | tr -d '\r')"
    if [ -n "$download_size" ]; then
        echo '- Validating file'
        local filesize="$(get_filesize "$tmpfile")"
        if [ "$download_size" -ne "$filesize" ]; then
            echo "Error: File verification failed, expecting $download_size bytes but downloaded $filesize bytes. Please re-run the script."
            rm -f "$tmpfile"
            exit -1
        fi
    fi

    echo '- Kill instances'
    kill -TERM "$software" 2> /dev/null
    sleep 2
    kill -KILL "$software" 2> /dev/null
    ps -ef | grep ".*dotnet.*${software}.dll" | grep -v grep | awk '{print $2}' | xargs kill 2> /dev/null
    sleep 0.5
}

cat << "EOF"
 _   _      _   ____                         
| \ | | ___| |_/ ___|  ___  _ __   __ _ _ __ 
|  \| |/ _ \ __\___ \ / _ \| '_ \ / _` | '__|
| |\  |  __/ |_ ___) | (_) | | | | (_| | |   
|_| \_|\___|\__|____/ \___/|_| |_|\__,_|_|   
    Auto download and installer script

- Detecting OS
EOF

if [ "${OSTYPE:0:6}" == "darwin" ]; then
    osVariant="osx"
elif testcmd apt-get; then
    osVariant="linux"
    ! testcmd curl && sudo apt-get install -y curl
elif testcmd pacman; then
    osVariant="arch"
    ! testcmd curl && sudo pacman -S curl
elif testcmd dnf; then
    osVariant="rhel"
    ! testcmd curl && sudo dnf install -y curl
elif testcmd zypper; then
    osVariant="suse"
    ! testcmd curl && sudo zypper install -y curl
fi

if [ -z "$osVariant" ]; then
    echo "Error: Unable to detect your Operative System."
    exit -1
fi

echo "- $osVariant $arch"

if [ "$osVariant" == "osx" ]; then
    #############
    #   macOS   #
    #############
    macOS_version="$(sw_vers -productVersion)"
    appPath="/Applications/${software}.app"

    if [ $(version $macOS_version) -lt $(version $macOS_least_version) ]; then
        echo "Error: Unable to install, $software requires at least macOS $macOS_least_version."
        exit -1
    fi

    if ! testcmd codesign; then
        echo '- Codesign required, installing...'
        xcode-select --install
    fi

    echo '- Detecting download'

    download_url="$(curl -s "$api_url" \
    | grep "browser_download_url.*_${osVariant}-${archCode}_.*[.]zip" \
    | head -1 \
    | cut -d : -f 2,3 \
    | tr -d \")"

    if [ -z "$download_url" ]; then
        echo "Error: Unable to detect the download url. Version '$tag' may not exist."
        exit -1
    fi

    download "$download_url"

    echo '- Removing old versions'
    rm -rf "$appPath"

    echo "- Inflating $filename to $appPath"
    unzip -q -o "$tmpfile" -d "/Applications"
    rm -f "$tmpfile"

    if [ -d "$appPath" ]; then
        echo '- Removing com.apple.quarantine security flag (gatekeeper)'
        find "$appPath" -print0 | xargs -0 xattr -d com.apple.quarantine &> /dev/null

        # Force codesign to allow the app to run directly
        echo '- Codesign app bundle'
        codesign --force --deep --sign - "$appPath"

        echo ''
        echo "Installation was successful. $software will now run."
        echo ''

        open -n "$appPath"
    else
        echo "Installation unsuccessful, unable to create '$appPath'."
        exit -1
    fi
else
    #############
    #   Linux   #
    #############
    echo '- Detecting download'
    response="$(curl -s "$api_url")"

    download_url="$(echo "$response" \
    | grep "browser_download_url.*_${osVariant}-x64_.*[.]AppImage" \
    | head -1 \
    | cut -d : -f 2,3 \
    | tr -d \")"

    if [ -z "$download_url" ]; then
        download_url="$(echo "$response" \
        | grep "browser_download_url.*_linux-x64_.*[.]AppImage" \
        | head -1 \
        | cut -d : -f 2,3 \
        | tr -d \")"
    fi

    if [ -z "$download_url" ]; then
        echo "Error: Unable to detect the download url. Version '$tag' may not exist."
        exit -1
    fi

    download "$download_url"

    targetDir="$PWD"
    [ -d "$HOME/Applications" ] && targetDir="$HOME/Applications"
    targetFilePath="$targetDir/${software}.AppImage"

    echo '- Removing old versions'
    rm -f "$targetDir/${software}_"*".AppImage"

    echo "- Moving $filename to $targetDir"
    mv -f "$tmpfile" "$targetFilePath"
    rm -f "$tmpfile"

    echo '- Setting permissions'
    chmod -fv 775 "$targetFilePath"

    "$targetFilePath" &

    echo ''
    echo "Installation was successful. $software will now run."
    echo 'If prompt for "Desktop integration", click "Integrate and run"'
    echo ''
fi
