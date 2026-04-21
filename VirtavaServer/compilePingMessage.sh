source '/Users/denys/Desktop/Diploma/Project/VirtavaServer/.venv/bin/activate'
script_path=$(cd $(dirname $0) && pwd)
message_name=ping.proto
cd $script_path
python -m grpc_tools.protoc --proto_path=../ --python_out=src/virtava_server/ --pyi_out=src/virtava_server/ "../$message_name"
deactivate