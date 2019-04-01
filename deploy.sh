# Change into directory where deployscript resides in
BASEDIR=$(dirname "$0")
cd $BASEDIR

# Build the Angular app
read -p "Upload frontend? (y/n)" -n 1 -r
if [[ $REPLY =~ ^[Yy]$ ]]
    then
    echo

    echo "---------- Building frontend app ----------"
    cd Frontend
    ng build --prod --build-optimizer
    echo
    echo "----------  Built frontend app   ----------"
    echo
    echo "----------  Uploading frontend   ----------"
    cd dist

    # Sync all frontend files
    rsync -pruvz -e "ssh -i <key> -p <port>" ./ <user>@<ip>:<dir>
    echo
    echo "----------   Uploaded frontend   ----------"
    echo

    echo "----------      Tidying up       ----------"
    cd ../../
    rm -rf Frontend/dist
    echo
    echo "----------       COMPLETED       ----------"
fi

echo

read -p "Upload API? (y/n)" -n 1 -r
if [[ $REPLY =~ ^[Yy]$ ]]
    then
    echo

    # Publish the API
    echo "----------     Building API      ----------"
    cd API
    dotnet publish -c Release -r ubuntu.16.04-x64
    echo
    echo "----------       Built API       ----------"
    echo

    echo "----------     Uploading API     ----------"
    cd bin/Release/netcoreapp2.0/ubuntu.16.04-x64/publish

    # Sync all API files
    rsync -pruvz -e "ssh -i <key> -p <port>" ./ <user>@<ip>:<dir>
    echo
    echo "----------     Uploaded API      ----------"
    echo
    echo
    echo "----------      Tidying up       ----------"
    cd ../../../../../../
    rm -rf API/bin/Release
    echo
    echo "----------       COMPLETED       ----------"
fi

echo

read -p "Restart API? (y/n)" -n 1 -r
if [[ $REPLY =~ ^[Yy]$ ]]
    then
    echo

    echo "----------     Restart API       ----------"
    read -p "Sudo password:" -s password
    echo
    ssh -i <key> -p <port> <user>@<ip> "echo $password | sudo -S service api restart; echo; sudo service api status"
    echo
    echo "----------     Restarted API     ----------"
fi
