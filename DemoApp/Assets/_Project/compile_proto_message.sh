script_path=$(cd $(dirname $0) && pwd)
message_name=tracking_results.proto
cd $script_path
protoc --proto_path=../../../VirtavaArkitBlendshapes/ --csharp_out=. "../../../VirtavaArkitBlendshapes/$message_name"