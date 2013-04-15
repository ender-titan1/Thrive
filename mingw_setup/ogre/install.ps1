param(
    [string]$MINGW_ENV
)

########
# Ogre #
########

Write-Output "--- Installing Ogre SDK ---"

$DIR = Split-Path $MyInvocation.MyCommand.Path

#################
# Include utils #
#################

. (Join-Path "$DIR\.." "utils.ps1")


############################
# Create working directory #
############################

$WORKING_DIR = Join-Path $MINGW_ENV temp\ogre

mkdir $WORKING_DIR -force | out-null


###################
# Check for 7-Zip #
###################

$7z = Join-Path $MINGW_ENV "temp\7zip\7za.exe"

if (-Not (Get-Command $7z -errorAction SilentlyContinue))
{
    return $false
}


####################
# Download archive #
####################

$REMOTE_DIR="http://downloads.sourceforge.net/project/ogre/ogre/1.8/1.8.1"

$ARCHIVE="OgreSDK_MinGW_v1-8-1.exe"

$DESTINATION = Join-Path $WORKING_DIR $ARCHIVE

if (-Not (Test-Path $DESTINATION)) {
    Write-Output "Downloading archive..."
    $CLIENT = New-Object System.Net.WebClient
    $CLIENT.DownloadFile("$REMOTE_DIR/$ARCHIVE", $DESTINATION)
}
else {
    Write-Output "Found archive file, skipping download."
}

##########
# Unpack #
##########

Write-Output "Unpacking archive..."

$ARGUMENTS = "x",
             "-y",
             "-o$WORKING_DIR",
             $DESTINATION
             
& $7z $ARGUMENTS


Write-Output "Installing..."

Copy-Item (Join-Path $WORKING_DIR "OgreSDK_MinGW_v1-8-1") -destination (Join-Path $MINGW_ENV "OgreSDK") -Recurse -Force
