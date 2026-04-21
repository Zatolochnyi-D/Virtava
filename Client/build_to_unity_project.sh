base_path=$(cd $(dirname $0) && pwd)
dll_location=$(cd $base_path && cd bin/Release/netstandard2.1/ && pwd)
dll_name=Client.dll
consumer_location=$(cd $base_path && cd ../../DemoApp/Assets/Plugins/Client/ && pwd)

cd $base_path
dotnet build -c "Release"
cp "$dll_location/$dll_name" "$consumer_location/"