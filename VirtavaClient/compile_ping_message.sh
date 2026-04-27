script_path=$(cd $(dirname $0) && pwd)
message_name=ping.proto
cd $script_path
protoc --proto_path=../ --csharp_out=Client/ "../$message_name"