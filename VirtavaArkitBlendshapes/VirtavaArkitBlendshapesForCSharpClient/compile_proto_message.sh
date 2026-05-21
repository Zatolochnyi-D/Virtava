script_path=$(cd $(dirname $0) && pwd)
message_name=arkit_blendshapes.proto
cd $script_path
protoc --proto_path=../ --csharp_out=VirtavaArkitBlendshapesForCSharpClient "../$message_name"