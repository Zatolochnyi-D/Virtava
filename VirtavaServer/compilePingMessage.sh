message_path=$(cd $(dirname $0) && pwd)
message_name=ping.proto
cd $message_path
python -m grpc_tools.protoc --proto_path=. --python_out=src/virtava_server/ --pyi_out=src/virtava_server/ $message_name 